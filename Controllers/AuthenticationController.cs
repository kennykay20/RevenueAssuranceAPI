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

                Auth.UserName = HttpUtility.HtmlEncode(Auth.UserName);
                Auth.Password = HttpUtility.HtmlEncode(Auth.Password);

                var _response = await _LoginImplementation.AutheticateUser(Auth.UserName, Auth.Password);

                /*Note: from above response code below means:
                     1. -1000 is Lincense has Expires
                     2. 2000 Lincense about to Expire 
                */
                if (_response.ErrorCode == 0)
                {
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
                    };
                    /*  
                      var ser = await _DeserializeSerialize.Serialize(userDetails);

                      var enCriptUserDetails = UserEncrypt.Encrypt(ser);

                      var DeCriptUserDetails = UserEncrypt.Decrypt(enCriptUserDetails);

                      var Deser = await _DeserializeSerialize.DeSerialize(DeCriptUserDetails);
                  */

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
    }
}