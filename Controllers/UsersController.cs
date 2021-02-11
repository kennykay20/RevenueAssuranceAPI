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
using RevAssuranceApi.Helper;
using RevAssuranceApi.Response;
using RevAssuranceApi.RevenueAssurance.DATA.Models;
using RevAssuranceApi.RevenueAssurance.Repository.DapperDAL;
using RevAssuranceWebAPi.AnythingGood.DATA.Models;
using RevAssuranceApi.RevenueAssurance.Repository.Interface;
using RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO;
using RevAssuranceApi.OperationImplemention;

namespace RevAssuranceApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [EnableCors("CorsPolicy")]
    //[Authorize]
    public class UsersController : ControllerBase
    {
         IConfiguration _configuration;
        ApiResponse ApiResponse = new ApiResponse();
        TokenGenerator TokenGenerator;  
        AppSettingsPath AppSettingsPath ;
         IDbConnection db = null;
         ApplicationDbContext   _ApplicationDbContext;
         Cryptors Cryptors = new Cryptors();
         IRepository<admRole> _repoRoles;
         IRepository<admUserProfile> _repoadmUserProfile;
         UsersImplementation _UsersImplementation;
         RoleAssignImplementation _RoleAssignImplementation;
         IRepository<admUserLimit> _repoadmUserLimit;
         IRepository<admCurrency> _repoadmCurrency; 
        public UsersController(IConfiguration configuration,
                                ApplicationDbContext   ApplicationDbContext,
                                IRepository<admRole> repoRoles,
                                IRepository<admUserProfile> repoadmUserProfile,
                                 UsersImplementation UsersImplementation,
                                 RoleAssignImplementation RoleAssignImplementation,
                                 IRepository<admUserLimit> repoadmUserLimit,
                                 IRepository<admCurrency> repoadmCurrency) 
        {
       
           _configuration = configuration;
           AppSettingsPath = new AppSettingsPath(_configuration);
           TokenGenerator = new TokenGenerator(_configuration);
           db = new SqlConnection(AppSettingsPath.GetDefaultCon());
           _ApplicationDbContext =  ApplicationDbContext;
           _repoRoles  = repoRoles;
           _repoadmUserProfile = repoadmUserProfile;
           _UsersImplementation = UsersImplementation;
           _RoleAssignImplementation = RoleAssignImplementation;
           _repoadmUserLimit = repoadmUserLimit;
           _repoadmCurrency = repoadmCurrency;
        }
   

        [HttpPost("GetAll")]
        public async Task<IActionResult> GetAll(ParamLoadPage AnyAuth)
        {
            try
            {
                var roleAssign = await _RoleAssignImplementation.GetRoleAssign(AnyAuth.MenuId,AnyAuth.RoleId);
                
                var rtn = new DapperDATAImplementation<admUserProfileDTO>();

                string script = @"		Select
                                * 
                                ,(Select RoleName from  admRole where RoleId = a.Roleid) RoleName
                                ,(Select BranchName from  admBankBranch where BranchNo = a.BranchNo) BranchName
								,(Select DeptName from  admDepartment where DeptId = a.DeptId) DeptName
                                from admUserProfile a
                               ";
                
                var _response = await rtn.LoadListNoParam(script, db);
            
               if(_response != null)
                {
                     var res = new {
                        _response = _response,
                        roleAssign = roleAssign
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
        public async Task<IActionResult> Update(admUserProfileDTO p)
        {
            try
            {
                var ApiResponse = new ApiResponse();
                var get = await _repoadmUserProfile.GetAsync(c=> c.LoginId == p.LoginId);
                if(get != null)
                {
                    //get.UserId
                    get.FullName = p.FullName;
                    get.LoginId = p.LoginId.ToUpper();
                    get.IsFirstLogin = false;
                    get.IsSupervisor = p.IsSupervisor;
                    get.LoggedOn = false;
                    get.RoleId = p.RoleId;
                    get.DeptId = p.DeptId;
                    get.BranchNo = p.BranchNo;
                    get.PasswordExpiryDate = DateTime.Now;
                    get.Status = p.Status;
                    get.EmailAddress = p.EmailAddress;
                    get.MobileNo  = p.MobileNo;
                    get.EnforcePswdChange = "Y";
                    get.CreatedBy  = p.CreatedBy;
                    get.DateCreated = DateTime.Now;
                    get.Password = Cryptors.Encrypt(p.LoginId.ToUpper());
                    get.loginstatus = 0;
                    get.lockcount = 0;
                    get.logincount = 0;

                    get.UseCbsAuth = p.UseCbsAuth;
                    get.CanPrintStatement = p.CanPrintStatement;
                    get.CanPrintBidSecurity = p.CanPrintBidSecurity ;
                    get.CanPrintRefLetter = p.CanPrintRefLetter;
                    get.CanPrintBond = p.CanPrintBond;
                    get.CanPrintTradeRef = p.CanPrintTradeRef;
                    get.CanPrintOD = p.CanPrintOD ;
                    get.CanReverse = p.CanReverse ;
                    get.DataSources = p.DataSources;
                    get.AuthUser = p.AuthUser;
                    get.CanApprove = p.CanApprove;
                    get.RsmId = p.RsmId;
                    get.Status = p.Status.Trim();

                    _repoadmUserProfile.Update(get);
                    int _response =  await _ApplicationDbContext.SaveChanges(p.LoginUserId);
                    if(_response > 0)
                    {
                      ApiResponse.ResponseMessage =  _configuration["Message:UpdateSuc"];
                      return Ok(ApiResponse);
                    }
                }

            }
            catch (Exception ex)
            {
                 ApiResponse.ResponseMessage =  ex == null ? ex.InnerException.Message : ex.Message;
                
                 ApiResponse.ResponseCode = -99;
                 return BadRequest(ApiResponse); 
            }
            ApiResponse.ResponseMessage  ="Record not Uploaded";
                
                 ApiResponse.ResponseCode = -99;
                 return BadRequest(ApiResponse); 
           
        }

       
        [HttpPost("Add")]
        public async Task<IActionResult> Add(admUserProfileDTO p)
        {
            try
            {
                var ApiResponse = new ApiResponse();
                var getExist = await _repoadmUserProfile.GetAsync(c=> c.LoginId == p.LoginId);
                if(getExist != null)
                {
                  ApiResponse.ResponseCode = -99;
                  ApiResponse.ResponseMessage = "Users Already Exist!";
                  return BadRequest(ApiResponse);
                }
                else
                {
                  var get = new admUserProfile();
                    
                    //get.UserId
                    get.FullName = p.FullName;
                    get.LoginId = p.LoginId.ToUpper();
                    get.IsFirstLogin = false;
                    get.IsSupervisor = p.IsSupervisor;
                    get.LoggedOn = false;
                    get.RoleId = p.RoleId;
                    get.DeptId = p.DeptId;
                    get.BranchNo = p.BranchNo;
                    get.PasswordExpiryDate = DateTime.Now;
                    get.Status = p.Status;
                    get.EmailAddress = p.EmailAddress;
                    get.MobileNo  = p.MobileNo;
                    get.EnforcePswdChange = "Y";
                    get.CreatedBy  = p.CreatedBy;
                    get.DateCreated = DateTime.Now;
                    get.Password = Cryptors.Encrypt(p.LoginId.ToUpper());
                    get.loginstatus = 0;
                    get.lockcount = 0;
                    get.logincount = 0;

                    get.UseCbsAuth = p.UseCbsAuth;
                    get.CanPrintStatement = p.CanPrintStatement;
                    get.CanPrintBidSecurity = p.CanPrintBidSecurity ;
                    get.CanPrintRefLetter = p.CanPrintRefLetter;
                    get.CanPrintBond = p.CanPrintBond;
                    get.CanPrintTradeRef = p.CanPrintTradeRef;
                    get.CanPrintOD = p.CanPrintOD ;
                    get.CanReverse = p.CanReverse ;
                    get.DataSources = p.DataSources;
                    get.AuthUser = p.AuthUser;
                    get.CanApprove = p.CanApprove;
                    get.RsmId = p.RsmId;
                    get.Status = p.Status.Trim();

                    await _repoadmUserProfile.AddAsync(get);
                    int _response =  await _ApplicationDbContext.SaveChanges(p.LoginUserId);
                    if(_response > 0)
                    {
                      ApiResponse.ResponseMessage =  _configuration["Message:AddedSuc"];
                      return Ok(ApiResponse);
                    }
                    else
                    {
                        ApiResponse.ResponseCode = -99;
                        ApiResponse.ResponseMessage = "Error Occured, Try later or Contact System Admin";
                        return BadRequest(ApiResponse); 
                    }
              }
            
            }
            catch (Exception ex)
            {
                 ApiResponse.ResponseMessage =  ex == null ? ex.InnerException.Message : ex.Message;
                
                 ApiResponse.ResponseCode = -99;
                 return BadRequest(ApiResponse); 
            }

           
        }

        [HttpPost("GetUser")]
        public async Task<IActionResult> GetUser(UsersDTO p)
        {
            try
            {
                var ApiResponse = new ApiResponse();
                var getExist = await _repoadmUserProfile.GetAsync(c=> c.UserId == p.CreatedBy);
                if(getExist != null)
                {
                  return Ok(getExist);
                }
                else
                {
                        ApiResponse.ResponseCode = -99;
                        ApiResponse.ResponseMessage = "No User Found";
                        return BadRequest(ApiResponse); 
                
                 }
             
            }
            catch (Exception ex)
            {
                 ApiResponse.ResponseMessage =  ex == null ? ex.InnerException.Message : ex.Message;
                
                 ApiResponse.ResponseCode = -99;
                 return BadRequest(ApiResponse); 
            }

           
        }

        [HttpPost("GetUserById")]
        public async Task<IActionResult> GetUserById(AAuth AnyAuth)
        {
            try
            {
                var usrname = await _repoadmUserProfile.GetAsync(p=>p.UserId == AnyAuth.UserId);
            
                if(usrname != null)
                {
                   return Ok(usrname);
                }
                else
                {
                    var api = new ApiResponse(); 
                    api.ResponseMessage = "No Users!";
                    return BadRequest(api); 
                }
                  
            
            }
            catch (Exception ex)
            {
                 ApiResponse.ResponseMessage =  ex == null ? ex.InnerException.Message : ex.Message;
                
                 ApiResponse.ResponseCode = -99;
                 return BadRequest(ApiResponse); 
            }
        }


        [HttpPost("FetchUserDetailsCoreBanking")]
        public async Task<IActionResult> FetchUserDetailsCoreBanking(GetUserDetailsParamCoreBnkingDTO AnyAuth)
        {
            try
            {
                var details = await _UsersImplementation.GetUserDetailsCore(AnyAuth);
            
                if(details != null)
                {
                    if(details.nErrorCode == 0)
                    {
                        return Ok(details);
                    }
                }
                else
                {
                    var api = new ApiResponse(); 
                    api.ResponseMessage = "No Users!";
                    return BadRequest(api); 
                }
            }
            catch (Exception ex)
            {
                 ApiResponse.ResponseMessage =  ex == null ? ex.InnerException.Message : ex.Message;
                
                 ApiResponse.ResponseCode = -99;
                 return BadRequest(ApiResponse); 
            }

             ApiResponse.ResponseMessage = "User not found";
                
                 ApiResponse.ResponseCode = -99;
            return  BadRequest(ApiResponse); 
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetUserDTO p)
        {
            try
            {
                var ApiResponse = new ApiResponse();

                foreach(var b in p.admUserProfileDTO)
                {
                var get = await _repoadmUserProfile.GetAsync(c=> c.LoginId == b.LoginId);
                if(get != null)
                {
                   
                    get.Password = Cryptors.Encrypt(b.LoginId.ToUpper());
                    _repoadmUserProfile.Update(get);
                    int _response =  await _ApplicationDbContext.SaveChanges(p.LoginUserId);
                    if(_response > 0)
                    {
                      ApiResponse.ResponseMessage =  "Reset Password Successfully!";;
                      return Ok(ApiResponse);
                    }
                  }
                }
            }
            catch (Exception ex)
            {
                 ApiResponse.ResponseMessage =  ex == null ? ex.InnerException.Message : ex.Message;
                
                 ApiResponse.ResponseCode = -99;
                 return BadRequest(ApiResponse); 
            }
            ApiResponse.ResponseMessage  ="Reset Password not Successful!";
                
                 ApiResponse.ResponseCode = -99;
                 return BadRequest(ApiResponse); 
           
        }

        [HttpPost("ResetLocked")]
        public async Task<IActionResult> ResetLocked(ResetUserDTO p)
        {
            try
            {
                var ApiResponse = new ApiResponse();

                foreach(var b in p.admUserProfileDTO)
                {
                var get = await _repoadmUserProfile.GetAsync(c=> c.LoginId == b.LoginId);
                if(get != null)
                {
                   
                    get.lockcount = 0;
                    _repoadmUserProfile.Update(get);
                    int _response =  await _ApplicationDbContext.SaveChanges(p.LoginUserId);
                    if(_response > 0)
                    {
                      ApiResponse.ResponseMessage =  "Unlock User Successfully!";
                      return Ok(ApiResponse);
                    }
                  }
                }
            }
            catch (Exception ex)
            {
                 ApiResponse.ResponseMessage =  ex == null ? ex.InnerException.Message : ex.Message;
                
                 ApiResponse.ResponseCode = -99;
                 return BadRequest(ApiResponse); 
            }
            ApiResponse.ResponseMessage  ="Record not Uploaded";
                
                 ApiResponse.ResponseCode = -99;
                 return BadRequest(ApiResponse); 
           
        }

         [HttpPost("GetUserLimit")]
        public async Task<IActionResult> GetUserLimit(ParamLoadPage AnyAuth)
        {
            try
            {
                var _response = await _repoadmUserLimit.GetAsync(c=> c.UserItbId == AnyAuth.UserId);

               if(_response != null)
                {
                     
                   return Ok(_response);
                }
                else
                {
                    var admUserLimit = new admUserLimit();
                    admUserLimit.CreditLimit = Convert.ToDecimal("0.00");
                    admUserLimit.DebitLimit = 0;
                    admUserLimit.GLDebitLimit = 0;
                    admUserLimit.GLCreditLimit = 0;

                    return Ok(admUserLimit); 
                }
            }
            catch (Exception ex)
            {
                 ApiResponse.ResponseMessage =  ex == null ? ex.InnerException.Message : ex.Message;
                
                 ApiResponse.ResponseCode = -99;
                 return BadRequest(ApiResponse); 
            }
        }

        [HttpPost("UserLimit")]
        public async Task<IActionResult> UserLimit(UserLimitsParamDTO p)
        {
            try
            {
                var ApiResponse = new ApiResponse();
                var get = await _repoadmUserProfile.GetAsync(c=> c.LoginId == p.admUserProfileDTO.LoginId);
                if(get != null)
                {
                    //get.UserId
                    get.FullName = p.admUserProfileDTO.FullName;
                    get.LoginId = p.admUserProfileDTO.LoginId.ToUpper();
                    get.IsFirstLogin = false;
                    get.IsSupervisor = p.admUserProfileDTO.IsSupervisor;
                    get.LoggedOn = false;
                    get.RoleId = p.admUserProfileDTO.RoleId;
                    get.DeptId = p.admUserProfileDTO.DeptId;
                    get.BranchNo = p.admUserProfileDTO.BranchNo;
                    get.PasswordExpiryDate = DateTime.Now;
                    get.Status = p.admUserProfileDTO.Status;
                    get.EmailAddress = p.admUserProfileDTO.EmailAddress;
                    get.MobileNo  = p.admUserProfileDTO.MobileNo;
                    get.EnforcePswdChange = "Y";
                    get.DateCreated = DateTime.Now;
                    get.loginstatus = 0;
                    get.lockcount = 0;
                    get.logincount = 0;

                    get.UseCbsAuth = p.admUserProfileDTO.UseCbsAuth;
                    get.CanPrintStatement = p.admUserProfileDTO.CanPrintStatement;
                    get.CanPrintBidSecurity = p.admUserProfileDTO.CanPrintBidSecurity ;
                    get.CanPrintRefLetter = p.admUserProfileDTO.CanPrintRefLetter;
                    get.CanPrintBond = p.admUserProfileDTO.CanPrintBond;
                    get.CanPrintTradeRef = p.admUserProfileDTO.CanPrintTradeRef;
                    get.CanPrintOD = p.admUserProfileDTO.CanPrintOD ;
                    get.CanReverse = p.admUserProfileDTO.CanReverse ;
                    get.DataSources = p.admUserProfileDTO.DataSources;
                    get.AuthUser = p.admUserProfileDTO.AuthUser;
                    get.CanApprove = p.admUserProfileDTO.CanApprove;
                    //get.RsmId = p.RsmId;
                    get.Status = p.admUserProfileDTO.Status.Trim();

                    _repoadmUserProfile.Update(get);
                    int _response =  await _ApplicationDbContext.SaveChanges(p.admUserProfileDTO.LoginUserId);
                    
                    var _getUserLim = await _repoadmUserLimit.GetAsync(c=> c.UserItbId == get.UserId);
                    int saveUserLim = -1;
                    if(_getUserLim != null)
                    {

                        _getUserLim.DebitLimit = p.UserLimitDTO.DebitLimit;
                        _getUserLim.CreditLimit = p.UserLimitDTO.CreditLimit;
                        _getUserLim.GLDebitLimit = p.UserLimitDTO.GLDebitLimit;
                        _getUserLim.GLCreditLimit = p.UserLimitDTO.GLCreditLimit;

                        _repoadmUserLimit.Update(_getUserLim);
                        saveUserLim = await _ApplicationDbContext.SaveChanges(p.admUserProfileDTO.LoginUserId);
                    }
                    
                    
                    if(_response > 0 && saveUserLim > 0)
                    {
                      ApiResponse.ResponseMessage =  "Transaction Limit Updated Successfully!";
                      return Ok(ApiResponse);
                    }
                }

            }
            catch (Exception ex)
            {
                 ApiResponse.ResponseMessage =  ex == null ? ex.InnerException.Message : ex.Message;
                
                 ApiResponse.ResponseCode = -99;
                 return BadRequest(ApiResponse); 
            }
            ApiResponse.ResponseMessage  ="Record not Uploaded";
                
                 ApiResponse.ResponseCode = -99;
                 return BadRequest(ApiResponse); 
           
        }


        [HttpPost("GetUserLimitNew")]
        public async Task<IActionResult> GetUserLimitNew(ParamLoadPage AnyAuth)
        {
            try
            {
                var rtn = new DapperDATAImplementation<UserLimitDTO>();

                string script = "select * from admUserLimits where UserItbId  = " + AnyAuth.UserItbId;
                
                var _response = await rtn.LoadListNoParam(script, db);

                 var roleAssign = await _RoleAssignImplementation.GetRoleAssign(AnyAuth.MenuId,AnyAuth.RoleId);
                
                var Currencies = await _repoadmCurrency.GetManyAsync(c=> c.IsoCode != null);


                if(_response != null)
                {
                    var res = new {
                        _response = _response,
                        roleAssign = roleAssign,
                        Currencies = Currencies

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

         [HttpPost("AddUserLimit")]
        public async Task<IActionResult> AddUserLimit(UserLimitsParamDTO param)
        {
            try
            {
                var userLimit = await _repoadmUserLimit.GetAsync(c=> c.CurrencyIso == param.UserLimitDTO.CurrencyIso && c.RoleId
                  == param.UserLimitDTO.RoleId && c.UserItbId == param.UserLimitDTO.UserItbId );
                if(userLimit != null)
                {

                    userLimit.CurrencyIso = param.UserLimitDTO.CurrencyIso;
                    userLimit.CreditLimit = param.UserLimitDTO.CreditLimit;
                    userLimit.DebitLimit = param.UserLimitDTO.DebitLimit;
                    userLimit.GLDebitLimit = param.UserLimitDTO.GLDebitLimit;
                    userLimit.GLCreditLimit = param.UserLimitDTO.GLCreditLimit;
                    userLimit.Status = param.UserLimitDTO.Status;

                    _repoadmUserLimit.Update(userLimit);

                    int Save = await _ApplicationDbContext.SaveChanges(param.LoginUserId);
                    if(Save > 0)
                    {
                        ApiResponse.ResponseMessage = "Your Request was Successful!" ;
                        return Ok(ApiResponse);
                    }
                    else
                    {
                        ApiResponse.ResponseMessage = "Error Occured!" ;
                        return BadRequest(ApiResponse); 
                    }
                }
                else
                {
                    var admUserLimitSave = new admUserLimit(); 

                    
                        admUserLimitSave.UserItbId = param.UserLimitDTO.UserItbId;
                        admUserLimitSave.RoleId = param.UserLimitDTO.RoleId;
                        admUserLimitSave.CurrencyIso = param.UserLimitDTO.CurrencyIso;
                        admUserLimitSave.CreditLimit = param.UserLimitDTO.CreditLimit;
                        admUserLimitSave.DebitLimit = param.UserLimitDTO.DebitLimit;
                        admUserLimitSave.GLDebitLimit = param.UserLimitDTO.GLDebitLimit;
                        admUserLimitSave.GLCreditLimit = param.UserLimitDTO.GLCreditLimit;
                        admUserLimitSave.UserId = param.LoginUserId;
                        admUserLimitSave.DateCreated = DateTime.Now;
                        admUserLimitSave.Status = "Active";
                        


                    await _repoadmUserLimit.AddAsync(admUserLimitSave);

                    int Save = await _ApplicationDbContext.SaveChanges(param.LoginUserId);
                    if(Save > 0)
                    {
                        ApiResponse.ResponseMessage = "Your Request was Successful!" ;
                        return Ok(ApiResponse);
                    }
                    else
                    {
                        ApiResponse.ResponseMessage = "Error Occured!" ;
                        return BadRequest(ApiResponse); 
                    }
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