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

using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RevAssuranceWebAPi.AnythingGood.DATA.Models;
using Microsoft.EntityFrameworkCore;
using RevAssuranceApi.RevenueAssurance.DATA.Models;
using RevAssuranceApi.Response;
using RevAssuranceApi.TokenGen;
using RevAssuranceApi.AppSettings;
using RevAssuranceApi.RevenueAssurance.Repository.DapperDAL;
using RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO;
using RevAssuranceApi.RevenueAssurance.Repository.Interface;
using RevAssuranceApi.OperationImplemention;
using RevAssuranceApi.Helper;

namespace RevAssuranceApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [EnableCors("CorsPolicy")]
    public class LicenceController : ControllerBase
    {
         IConfiguration _configuration;
        ApiResponse ApiResponse = new ApiResponse();
        TokenGenerator TokenGenerator;  
        AppSettingsPath AppSettingsPath ;
         IDbConnection db = null;
         ApplicationDbContext   _ApplicationDbContext;
         IRepository<admClientProfile> _repoClientProfile;
        LoginImplementation _LoginImplementation; 

         public LicenceController(IConfiguration configuration,
                                ApplicationDbContext   ApplicationDbContext,
                                 IRepository<admClientProfile> repoClientProfile,
                                 LoginImplementation LoginImplementation)
         {
            ;
           _configuration = configuration;
           AppSettingsPath = new AppSettingsPath(_configuration);
           TokenGenerator = new TokenGenerator(_configuration);
           db = new SqlConnection(AppSettingsPath.GetDefaultCon());
           _ApplicationDbContext =  ApplicationDbContext;
            _repoClientProfile = repoClientProfile;
              _LoginImplementation = LoginImplementation;
         }

         [HttpPost("GetAll")]
        public async Task<IActionResult> GetAll(AAuth AnyAuth)
        {
            try
            {
                var rtn = new DapperDATAImplementation<admLicenseSetUpHistory>();

                string script = "Select * from admLicenseSetUpHistory";
                
                var _response = await rtn.LoadListNoParam(script, db);
            
                if(_response != null)
                {
                   return Ok(_response);
                }
                else
                {
                    return BadRequest(_response); 
                }
                  
            
            }
            catch (Exception ex)
            {
                 ApiResponse.ResponseMessage =  ex.InnerException.Message ?? ex.Message;
                
                 ApiResponse.ResponseCode = -99;
                 return BadRequest(ApiResponse); 
            }

           
        }
    

        [HttpPost("License")]
        public async Task<IActionResult> License(LoginDTO Auth)
        {
          
            try
            {

         
               
            var clientProfile = await _repoClientProfile.GetAsync(null);  
               var _response = await _LoginImplementation.getLicense(clientProfile);

               /*Note: from above response code below means:
                    1. -1000 is Lincense has Expires
                    2. 2000 Lincense about to Expire 
               */
                    return Ok( new {
                              _response =  _response
                          });
              
            }
            catch (Exception ex)
            {
                var exM =  ex == null ? ex.InnerException.Message : ex.Message;
                //_LogManager.SaveLog($"Error Message: { exM } in login controler. StackTrace: {ex.StackTrace}");
                 ApiResponse.ResponseMessage =  ex == null ? ex.InnerException.Message : ex.Message;
                 ApiResponse.ResponseCode = -99;
                 return BadRequest(ApiResponse); 
            }
        }
   
        [HttpPost("NewLicense")]
        public async Task<IActionResult> NewLicense(License Auth)
        {
          
            try
            {
                try
                {
                    string dt = Cryptors.DecryptNoKey(Auth.LicenseKey);
                }
                catch(Exception ex1){

                       ApiResponse.ResponseMessage = "Invalid License Key";
                   return BadRequest(ApiResponse); 

                }
                  
               
            var clientProfile = await _repoClientProfile.GetAsync(null);  
                clientProfile.LicenceKey = Auth.LicenseKey;

                 _repoClientProfile.Update(clientProfile);

                var SaveTem = await _ApplicationDbContext.SaveChanges(0); //hard code will later confirm to know the user that does it
                if(SaveTem > 0){
                     ApiResponse.ResponseCode = 0;
                        ApiResponse.ResponseMessage = "License Updated  Successfully!";

                        return Ok(ApiResponse);
                }

                ApiResponse.ResponseMessage = "Could'nt  Update  License!";
                return BadRequest(ApiResponse); 
              
            }
            catch (Exception ex)
            {
                var exM =  ex == null ? ex.InnerException.Message : ex.Message;
             
                 ApiResponse.ResponseMessage =  ex == null ? ex.InnerException.Message : ex.Message;
                 ApiResponse.ResponseCode = -99;
                 return BadRequest(ApiResponse); 
            }
        }
   
    
    }
}