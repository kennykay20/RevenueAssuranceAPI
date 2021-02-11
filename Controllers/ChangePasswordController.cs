using System.ComponentModel;
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
using System.Web;
using RevAssuranceApi.AppSettings;

using RevAssuranceApi.TokenGen;
using Dapper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RevAssuranceApi.Helper;
using RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO;
using RevAssuranceWebAPi.AnythingGood.DATA.Models;
using anythingGoodApi.AnythingGood.DATA;
using RevAssuranceApi.OperationImplemention;
using RevAssuranceApi.WebServices;
using RevAssuranceApi.RevenueAssurance.DATA.Models;
using RevAssuranceApi.RevenueAssurance.Repository.Interface;
using RevAssuranceApi.Response;

namespace RevAssuranceApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [EnableCors("CorsPolicy")]
     //[Authorize]
    public class ChangePasswordController : ControllerBase
    {
      //   IAuditTrailService  _IAuditTrailService;
         IConfiguration _configuration;
        ApiResponse ApiResponse = new ApiResponse();
        TokenGenerator TokenGenerator;  
        AppSettingsPath AppSettingsPath ;
         IDbConnection db = null;
        DeserializeSerialize<UserReturnDetails> _DeserializeSerialize;
        IRepository<admLicenseSetUp> _repositoryLicense;
        IRepository<admClientProfile> _repoClientProfile;
        IRepository<admUserProfile> _repoadmUserProfile;
        ApplicationDbContext _ApplicationDbContext;
        IRepository<admUserLogin> _repoadmUserLogin;
        Formatter _Formatter =  new Formatter();
        ChangePwdImplementation _ChangePwdImplementation; 
        LogManager _LogManager;
        public ChangePasswordController(  
                                        IConfiguration configuration,
                                          DeserializeSerialize<UserReturnDetails> DeserializeSerialize,
                                          IRepository<admLicenseSetUp> repositoryLicense,
                                          IRepository<admClientProfile> repoClientProfile,
                                          IRepository<admUserProfile> repoadmUserProfile,
                                          ApplicationDbContext ApplicationDbContext,
                                          IRepository<admUserLogin> repoadmUserLogin,
                                          LoginImplementation LoginImplementation,
                                          LogManager LogManager,
                                          ChangePwdImplementation ChangePwdImplementation
                                         ) 
        {
           _configuration = configuration;
           AppSettingsPath = new AppSettingsPath(_configuration);
           TokenGenerator = new TokenGenerator(_configuration);
            db = new SqlConnection(AppSettingsPath.GetDefaultCon());
            _repositoryLicense = repositoryLicense;
           _repoClientProfile = repoClientProfile;
           _repoadmUserProfile = repoadmUserProfile;
            _ApplicationDbContext = ApplicationDbContext;
            _repoadmUserLogin = repoadmUserLogin;
            _ChangePwdImplementation  = ChangePwdImplementation;
            _DeserializeSerialize = DeserializeSerialize;
            _LogManager = LogManager;
        }
   
         [HttpPost("ChgPwd")] 
        public async Task<IActionResult> ChgPwd(ChangePwdDTO ChangePwdDTO)
        {
            try
            {
                if (ChangePwdDTO == null)
                {
                    return BadRequest("Specified Object is null or empty");
                } 
     
                var _response = await _ChangePwdImplementation.ChangePassword(ChangePwdDTO);

                if(_response.ResponseCode == 0)
                {
                    return Ok(_response);
                }
       
               return BadRequest(_response); 
               
            }
            catch (Exception ex)
            {
                var exM =  ex == null ? ex.InnerException.Message : ex.Message;
                _LogManager.SaveLog($"Error Message: { exM } in login controler. StackTrace: {ex.StackTrace}");
                 ApiResponse.ResponseMessage =  ex == null ? ex.InnerException.Message : ex.Message;
                 ApiResponse.ResponseCode = -99;
                 return BadRequest(ApiResponse); 
            }

        }
    
    
    }
}