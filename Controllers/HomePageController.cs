using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using anythingGoodApi.AnythingGood.DATA.Models;
using anythingGoodApi.AnythingGood.DATA.ModelsDTO;
using RevAssuranceApi.AppSettings;

using RevAssuranceApi.TokenGen;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RevAssuranceApi.EmailSettings;
using RevAssuranceApi.Response;
using RevAssuranceApi.RevenueAssurance.Repository.DapperDAL;

namespace RevAssuranceApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [EnableCors("CorsPolicy")]
  //  [Authorize]
    public class HomePageController : ControllerBase
    {
  
         IConfiguration _configuration;
        ApiResponse ApiResponse = new ApiResponse();
        AppSettingsPath AppSettingsPath ;
        EmailConfig _EmailConfig;
         IDbConnection db = null;
         TokenGenerator TokenGenerator;  
        public HomePageController(IConfiguration configuration) 
        {
           _configuration = configuration;
           AppSettingsPath = new AppSettingsPath(_configuration);
            TokenGenerator = new TokenGenerator(_configuration);
           _EmailConfig = new EmailConfig(_configuration);
           db = new SqlConnection(AppSettingsPath.GetDefaultCon());
        }
   
        [HttpPost("Products")]
        public async Task<IActionResult> Products(ProductInfo Postparam)
        {
            try
            {
                if(Postparam == null)
                {
                    return BadRequest("Specified Object is null or empty");
                }

                DynamicParameters param = new DynamicParameters();
                
                param.Add("@SkipNoOfRec", Postparam.SkipNoOfRec);
                param.Add("@fechNoOfRec", Postparam.FechNoOfRec);

                var rtn = new DapperDATAImplementation<ProductInfo>();
                
                var response = await rtn.LoadData("sp_GetHomePageProduct", param, db);
 
                var getToken = TokenGenerator.GetToken();
                    return Ok( new {
                    token = getToken.Token,
                    expiration = getToken.TokenExpireTime,
                    response = response,
                    
                }); 
            
            }
            catch (Exception ex)
            {
                 ApiResponse.ResponseMessage =  ex == null ? ex.InnerException.Message : ex.Message;
                
                 ApiResponse.ResponseCode = -99;
                 return BadRequest(ApiResponse); 
            }          
        }
   
    }
}