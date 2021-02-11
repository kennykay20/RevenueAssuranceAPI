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
using RevAssuranceApi.AppSettings;

using RevAssuranceApi.TokenGen;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RevAssuranceApi.RevenueAssurance.Repository.DapperDAL;

namespace RevAssuranceApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [EnableCors("CorsPolicy")]
    [Authorize]
    public class MenuController : ControllerBase
    {
   
         IConfiguration _configuration;
        AppSettingsPath AppSettingsPath ;
         IDbConnection db = null;
          TokenGenerator TokenGenerator;  
  
        public MenuController(IConfiguration configuration) 
        {
          
           _configuration = configuration;
           AppSettingsPath = new AppSettingsPath(_configuration);
            TokenGenerator = new TokenGenerator(_configuration);
           db = new SqlConnection(AppSettingsPath.GetDefaultCon());
        }
   
        [HttpPost("Menu")]
        public async Task<IActionResult> Menu(Menu Postparam)
        {
            try
            {
                if(Postparam == null)
                {
                    return BadRequest("Specified Object is null or empty");
                }

                DynamicParameters param = new DynamicParameters();
                param.Add("@MenuId", null);
                var rtn = new DapperDATAImplementation<Menu>();
                
                var _response = await rtn.LoadData("sp_Menu", param, db);

                var getToken = TokenGenerator.GetToken();
                return Ok( new {
                            token = getToken.Token,
                            expiration = getToken.TokenExpireTime,
                            _response = _response,
                            
                        });
               
            }
            catch (Exception ex)
            {
                   var exM =  ex == null ? ex.InnerException.Message : ex.Message;
                
                 return BadRequest("Couldnot get menu"); 
            }   
        }
    }
}