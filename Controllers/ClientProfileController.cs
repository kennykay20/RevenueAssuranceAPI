using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using anythingGoodApi.AnythingGood.DATA;
using anythingGoodApi.AnythingGood.DATA.Models;
using RevAssuranceApi.AppSettings;

using RevAssuranceApi.TokenGen;
using Dapper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RevAssuranceApi.Response;
using RevAssuranceApi.RevenueAssurance.DATA.Models;
using RevAssuranceApi.RevenueAssurance.Repository.DapperDAL;
using RevAssuranceWebAPi.AnythingGood.DATA.Models;
using RevAssuranceApi.RevenueAssurance.Repository.Interface;
using RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO;

namespace RevAssuranceApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [EnableCors("CorsPolicy")]
    //[Authorize]
    public class ClientProfileController : ControllerBase
    {
         IConfiguration _configuration;
        ApiResponse ApiResponse = new ApiResponse();
        TokenGenerator TokenGenerator;  
        AppSettingsPath AppSettingsPath ;
         IDbConnection db = null;
         ApplicationDbContext   _ApplicationDbContext;
         IRepository<admUserProfile> _repoadmUserProfile;
        public ClientProfileController(IConfiguration configuration,
                                        ApplicationDbContext   ApplicationDbContext,
                                         IRepository<admUserProfile> repoadmUserProfile) 
        {
           
           _configuration = configuration;
           AppSettingsPath = new AppSettingsPath(_configuration);
           TokenGenerator = new TokenGenerator(_configuration);
           db = new SqlConnection(AppSettingsPath.GetDefaultCon());
           _ApplicationDbContext =  ApplicationDbContext;
           _repoadmUserProfile = repoadmUserProfile;
        }
   
   

        [HttpPost("GetAll")]
        public async Task<IActionResult> GetAll(AAuth AnyAuth)
        {
            try
            {
                var rtn = new DapperDATAImplementation<admClientProfile>();

                string script = "Select * from admClientProfile";
                
                var _response = await rtn.LoadSingle(script, db);
                var getUSer = await _repoadmUserProfile.GetAsync(c => c.UserId == _response.UserId);


            
                if(_response != null)
                {
                    var res = new {
                        _response = _response,
                        getUSer = getUSer
                    };
                   return Ok(res);
                }
                else
                {
                    return BadRequest(_response); 
                }
                  
            
            }
            catch (Exception ex)
            {
                 ApiResponse.ResponseMessage =  ex == null ? ex.InnerException.Message : ex.Message;
                
                 ApiResponse.ResponseCode = -99;
                 return BadRequest(ApiResponse); 
            }

           
        }

        [HttpPost("Update")]
        public async Task<IActionResult> Update(admClientProfileDTO p)
        {
            try
            {
                var get =  _ApplicationDbContext.admClientProfile.FirstOrDefault(c=> c.BankCode == p.admClientProfile.BankCode);
                if(get != null)
                {

                    get.BankCode  = p.admClientProfile.BankCode ;
                    get.BankName = p.admClientProfile.BankName ;
                    get.BankAddress = p.admClientProfile.BankAddress ;
                    get.ChannelId = p.admClientProfile.ChannelId ;
                    get.CurrentProcessingDate = p.admClientProfile.CurrentProcessingDate ;
                    get.EnforcePasswordChangeDays = p.admClientProfile.EnforcePasswordChangeDays ;
                    get.EnforceStrngPwd = p.admClientProfile.EnforceStrngPwd ;
                    get.BankingSystem = p.admClientProfile.BankingSystem ;
                    get.LoginIdEncryption = p.admClientProfile.LoginIdEncryption ;
                    get.SystemIdleTimeout = p.admClientProfile.SystemIdleTimeout ;
                    get.CountryCode = p.admClientProfile.CountryCode ;
                    get.CurrencyCode = p.admClientProfile.CurrencyCode ;
                    get.Status = p.admClientProfile.Status ;
                    get.LoginCount = p.admClientProfile.LoginCount ;
                    get.LicenceKey = p.admClientProfile.LicenceKey ;
                    get.UseCBSAuth = p.admClientProfile.UseCBSAuth ;
                }
               _ApplicationDbContext.admClientProfile.Update(get);
              int _response =  await _ApplicationDbContext.SaveChanges(p.LoginUserId);
                if(_response > 0)
                {

                    ApiResponse.ResponseMessage =  _configuration["Message:UpdateSuc"];
                
                   return Ok(ApiResponse);
                }
                else
                {
                     ApiResponse.ResponseMessage  = "Record Could'nt be Updated";
                    return BadRequest(_response); 
                }
                  
            
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