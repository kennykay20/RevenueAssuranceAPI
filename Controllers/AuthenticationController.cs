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
    public class AuthenticationController : ControllerBase
    {
        //   IAuditTrailService  _IAuditTrailService;
        IConfiguration _configuration;
        ApiResponse ApiResponse = new ApiResponse();
        TokenGenerator TokenGenerator;
        AppSettingsPath AppSettingsPath;
        IDbConnection db = null;
        DeserializeSerialize<UserReturnDetails> _DeserializeSerialize;
        IRepository<admLicenseSetUp> _repositoryLicense;
        IRepository<admClientProfile> _repoClientProfile;
        IRepository<admUserProfile> _repoadmUserProfile;
        ApplicationDbContext _ApplicationDbContext;
        IRepository<admUserLogin> _repoadmUserLogin;
        Formatter _Formatter = new Formatter();
        LoginImplementation _LoginImplementation;
        LogManager _LogManager;
        public AuthenticationController(
                                          IConfiguration configuration,
                                          DeserializeSerialize<UserReturnDetails> DeserializeSerialize,
                                          IRepository<admLicenseSetUp> repositoryLicense,
                                          IRepository<admClientProfile> repoClientProfile,
                                          IRepository<admUserProfile> repoadmUserProfile,
                                          ApplicationDbContext ApplicationDbContext,
                                          IRepository<admUserLogin> repoadmUserLogin,
                                          LoginImplementation LoginImplementation,
                                          LogManager LogManager
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
            _LoginImplementation = LoginImplementation;
            _DeserializeSerialize = DeserializeSerialize;
            _LogManager = LogManager;
        }

        [HttpPost("AAuth")]
        public async Task<IActionResult> AAuth(LoginDTO Auth)
        {
            _LogManager.SaveLog("AAuth Start");
            try
            {
                if (Auth == null)
                {
                    return BadRequest("Specified Object is null or empty");
                }

                Auth.UserName = Auth.UserName == null ? "" : HttpUtility.HtmlEncode(Auth.UserName);
                Auth.Password = HttpUtility.HtmlEncode(Auth.Password);
                Auth.staffId = Auth.staffId == null ? "" : HttpUtility.HtmlEncode(Auth.staffId);

                var _response = await _LoginImplementation.AuthenticateUser(Auth.UserName, Auth.Password, Auth.staffId, Auth.loginType);

                /*Note: from above response code below means:
                     1. -1000 is Lincense has Expires
                     2. 2000 Lincense about to Expire 
                */
                if (_response.ErrorCode == 0 || _response.ErrorCode == 9)
                {

                    //here we need to check and send two factor code to the user 
                    //var response2 = await _LoginImplementation.GenerateFactorCode((int)_response.UserId, _response.FullName);

                    var claim = new[]
                    {
                     new Claim(JwtRegisteredClaimNames.Sub, Auth.UserName)
                    };
                    
                    var getToken = TokenGenerator.GetToken();

                    var lastLoginTime = TokenGenerator.GetLastLogin();

                    var userDetails = new UserReturnDetails
                    {
                        UserName = Auth.UserName,
                        RoleId = _response.RoleId,
                        UserId = _response.UserId,
                        FullName = _response.FullName,
                        LoginId = Auth.UserName,
                        DeptId = _response.deptId,
                        BranchNo = _response.branchNo,
                        lastLoginTime = lastLoginTime,
                        BankingDate = _response.BankingDate,
                        RoleName = _response.RoleName,
                        BranchName = _response.BranchName,
                        CBSStatus = _response.Status,
                        loginType = _response.loginType,
                        staffId = _response.staffId,
                        userLoginType = _response.userLoginType
                    };
                    

                    var getMenu = await _LoginImplementation.getUserMenu(userDetails.RoleId);

                    return Ok(new
                    {
                        token = getToken.Token,
                        expiration = getToken.TokenExpireTime,
                        response = _response,
                        lastLoginTime = lastLoginTime,
                        userDetails = userDetails,
                        BankingDate = _response.BankingDate,
                        Menu = getMenu
                    });
                }
                
                else
                {
                    return BadRequest(_response);
                }
            }
            catch (Exception ex)
            {
                var exM = ex == null ? ex.InnerException.Message : ex.Message;
                _LogManager.SaveLog($"Error Message: { exM } in login controler. StackTrace: {ex.StackTrace}");
                ApiResponse.ResponseMessage = ex == null ? ex.InnerException.Message : ex.Message;
                ApiResponse.ResponseCode = -99;
                return BadRequest(ApiResponse);
            }
        
        }
    

        [HttpPost("AuthGenerateCode")]
        public async Task<IActionResult> AuthGenerateCode(LoginDTO Auth)
        {
            _LogManager.SaveLog("AuthGenerateCode Start");
            try
            {
                if (Auth == null)
                {
                    return BadRequest("Specified Object is null or empty");
                }

                Auth.UserName = HttpUtility.HtmlEncode(Auth.UserName);
                Auth.Password = HttpUtility.HtmlEncode(Auth.Password);

                var _response = await _LoginImplementation.AuthenticateUser(Auth.UserName, Auth.Password, Auth.staffId, Auth.loginType);

                /*Note: from above response code below means:
                     1. -1000 is Lincense has Expires
                     2. 2000 Lincense about to Expire 
                */
                if (_response.ErrorCode == 0)
                {
                    //here we need to check and send two factor code to the user 
                    //var response2 = await _LoginImplementation.GenerateFactorCode((int)_response.UserId, _response.FullName);

                    return Ok(new
                    {
                        //response = response2,
                        userId = _response.UserId
                    });
                    
                }
                else
                {
                    return BadRequest(_response);
                }
            }
            catch (Exception ex)
            {
                var exM = ex == null ? ex.InnerException.Message : ex.Message;
                _LogManager.SaveLog($"Error Message: { exM } in AuthGenerate Code controler. StackTrace: {ex.StackTrace}");
                ApiResponse.ResponseMessage = ex == null ? ex.InnerException.Message : ex.Message;
                ApiResponse.ResponseCode = -99;
                return BadRequest(ApiResponse);
            }
        
        }


        [HttpPost("VerifyOtp")]
        public async Task<IActionResult> VerifyOtp(LoginDTO Auth)
        {
            try
            {
                if (Auth == null)
                {
                    return BadRequest("Specified Object is null or empty");
                }

                Auth.UserName = Auth.UserName == null ? "" : HttpUtility.HtmlEncode(Auth.UserName);
                Auth.Password = HttpUtility.HtmlEncode(Auth.Password);
                var _response = await _LoginImplementation.CheckPasswordOtp(Auth.Password, Auth.UserName);

                return Ok(new
                {
                        response = _response,
                        userId = _response.UserId
                });
            }
            catch(Exception ex)
            {
                var exM = ex == null ? ex.InnerException.Message : ex.Message;
                _LogManager.SaveLog($"Error Message: { exM } in login controler. StackTrace: {ex.StackTrace}");
                ApiResponse.ResponseMessage = ex == null ? ex.InnerException.Message : ex.Message;
                ApiResponse.ResponseCode = -99;
                return BadRequest(ApiResponse);
            }
            //return BadRequest("error");
        }


        [HttpPost("ValidateTwoFactorCode")]
        public async Task<IActionResult> ValidateTwoFactorCode(LoginDTO auth, int userId)
        {
            _LogManager.SaveLog("ValidateTwoFactor Start");
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            try
            {
                // var result = await _LoginImplementation.ValidateTwoFactorCodeAsync(auth.Code, userId);
                // if(!result.IsValid)
                // {
                //     //MOVE on 
                // }
                return Ok(new {
                    //response = result
                });
            }
            catch(Exception ex)
            {

            }
            return Ok();
            //return Unauthorized(new { LoginError = "You request cannot be completed" });
        }
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassord(LoginDTO profile)
        {
            _LogManager.SaveLog("Resetpassword Start");
            
            try
            {
                if (profile == null)
                {
                    return BadRequest("Specified Object is null or empty");
                }

                profile.UserName = HttpUtility.HtmlEncode(profile.UserName);
                profile.Password = HttpUtility.HtmlEncode(profile.Password);

                var _response = await _LoginImplementation.PasswordReset(profile.UserName, profile.Password);

                if(_response.ErrorCode == 0)
                {
                    return Ok(_response);
                }
                else{
                    return BadRequest(_response);
                }
            }
            catch(Exception ex){
                var exM = ex == null ? ex.InnerException.Message : ex.Message;
                _LogManager.SaveLog($"Error Message: { exM } in ResetPassword controler. StackTrace: {ex.StackTrace}");
                ApiResponse.ResponseMessage = ex == null ? ex.InnerException.Message : ex.Message;
                ApiResponse.ResponseCode = -99;
                return BadRequest(ApiResponse);
            }
        }

        [HttpPost("TestPinToken")]
        public async Task<IActionResult> TestPinToken(StaffTest value)
        {
            if(value == null)
            {
                return BadRequest(new{
                    Status = false,
                    Message = "your value is null"
                });
            }
            if(value.Password == "12345678" && value.staffId != "")
            {
                return Ok(
                        new{
                            Status = true,
                            Message = "success"
                        }
                    );
            }
            return BadRequest(new{ Status = false,
                                Message = "error"});
        }

        
    
    }

    public class StaffTest
        {
            public string staffId { get; set; }
            public string Password { get; set; }
        }
}