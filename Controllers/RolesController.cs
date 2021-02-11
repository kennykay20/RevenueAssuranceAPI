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
    public class RolesController : ControllerBase
    {
         IConfiguration _configuration;
         ApiResponse ApiResponse = new ApiResponse();
         TokenGenerator TokenGenerator;  
         AppSettingsPath AppSettingsPath ;
         IDbConnection db = null;
         ApplicationDbContext   _ApplicationDbContext;
         Cryptors Cryptors = new Cryptors();
         IRepository<admRole> _repoRoles;
         IRepository<admCurrency> _repoadmCurrency; 
         RoleAssignImplementation _RoleAssignImplementation; 
         IRepository<admRoleLimit> _repoadmRoleLimit; 
         IRepository<admBankBranch> _repoadmBankBranch;
 
        public RolesController(IConfiguration configuration,
                                ApplicationDbContext   ApplicationDbContext,
                                IRepository<admRole> repoRoles,
                                 RoleAssignImplementation RoleAssignImplementation,
                                  IRepository<admCurrency> repoadmCurrency,
                                  IRepository<admRoleLimit> repoadmRoleLimit,
                                  IRepository<admBankBranch> repoadmBankBranch) 
        {
       
           _configuration = configuration;
           AppSettingsPath = new AppSettingsPath(_configuration);
           TokenGenerator = new TokenGenerator(_configuration);
           db = new SqlConnection(AppSettingsPath.GetDefaultCon());
           _ApplicationDbContext =  ApplicationDbContext;
           _repoRoles  = repoRoles;
           _RoleAssignImplementation = RoleAssignImplementation;
           _repoadmCurrency = repoadmCurrency;
           _repoadmRoleLimit = repoadmRoleLimit;
           _repoadmBankBranch = repoadmBankBranch;
        }
   
        [HttpPost("GetAll")]
        public async Task<IActionResult> GetAll(ParamLoadPage AnyAuth)
        {
            try
            {
                var rtn = new DapperDATAImplementation<AdmRoleDTO>();

                string script = @"Select * from admRole";
                
                var _response = await rtn.LoadListNoParam(script, db);

                 var roleAssign = await _RoleAssignImplementation.GetRoleAssign(AnyAuth.MenuId,AnyAuth.RoleId);
                
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

        
        [HttpPost("GetRoleLimit")]
        public async Task<IActionResult> GetRoleLimit(ParamLoadPage AnyAuth)
        {
            try
            {
                var rtn = new DapperDATAImplementation<admRoleLimitDTO>();

                string script = @"select * from admRoleLimits
                                where RoleId = " + AnyAuth.RoleId;
                
                var _response = await rtn.LoadListNoParam(script, db);

                 var roleAssign = await _RoleAssignImplementation.GetRoleAssign(AnyAuth.MenuId, AnyAuth.LoginUserRoleId);
                
                var Currencies = await _repoadmCurrency.GetManyAsync(c=> c.IsoCode != null);

                var branches = await _repoadmBankBranch.GetManyAsync(c=> c.Itbid > 0);
                if(_response != null)
                {
                    var res = new {
                        _response = _response,
                        roleAssign = roleAssign,
                        Currencies = Currencies,
                        branches = branches
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

         [HttpPost("AddRoleLimit")]
        public async Task<IActionResult> AddRoleLimit(admRoleLimitDTO param)
        {
            try
            {
                var admRoleLimit = await _repoadmRoleLimit.GetAsync(c=> c.CurrencyIso == param.CurrencyIso && c.RoleId
                  == param.RoleId );
                if(admRoleLimit != null)
                {
                    admRoleLimit.CurrencyIso = param.CurrencyIso;
                    admRoleLimit.CreditLimit = param.CreditLimit;
                    admRoleLimit.DebitLimit = param.DebitLimit;
                    admRoleLimit.GLDebitLimit = param.GLDebitLimit;
                    admRoleLimit.GLCreditLimit = param.GLCreditLimit;
                    admRoleLimit.Status = param.Status;

                    
                    _repoadmRoleLimit.Update(admRoleLimit);

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
                    var admRoleLimitSave = new admRoleLimit(); 

                     admRoleLimitSave.RoleId = param.RoleId;
                    admRoleLimitSave.CurrencyIso = param.CurrencyIso;
                    admRoleLimitSave.CreditLimit = param.CreditLimit;
                    admRoleLimitSave.DebitLimit = param.DebitLimit;
                     admRoleLimitSave.GLDebitLimit = param.GLDebitLimit;
                    admRoleLimitSave.GLCreditLimit = param.GLDebitLimit;
                    admRoleLimitSave.Status = param.Status;
                    admRoleLimitSave.DateCreated = DateTime.Now;
                    admRoleLimitSave.UserId = param.LoginUserId;


                    await _repoadmRoleLimit.AddAsync(admRoleLimitSave);

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

        
        
        
        [HttpPost("Update")]
        public async Task<IActionResult> Update(AdmRoleDTO p)
        {
            try
            {
                var get = await _repoRoles.GetAsync(c=> c.RoleId == p.admRole.RoleId);
                if(get != null)
                {
                    get.RoleName = p.admRole.RoleName;
                    get.Description = p.admRole.Description ;
                    get.UserId = p.LoginUserId ;
                    get.Status = p.admRole.Status ;
                }
               _repoRoles.Update(get);
              int _response =  await _ApplicationDbContext.SaveChanges(p.LoginUserId);
                if(_response > 0)
                {
                   ApiResponse.ResponseMessage=  _configuration["Message:UpdateSuc"];
                   return Ok(ApiResponse);
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
    
         [HttpPost("Add")]
        public async Task<IActionResult> Add(AdmRoleDTO p)
        {
            try
            {
                var ApiResponse = new ApiResponse();
                var getExist = await _repoRoles.GetAsync(c=> c.RoleName == p.admRole.RoleName);
                if(getExist != null)
                {
                  ApiResponse.ResponseCode = -99;
                  ApiResponse.ResponseMessage = "Role Already Exist!";
                  return BadRequest(ApiResponse);
                }
                else
                {

                
                  var get = new admRole();
                    get.RoleName = p.admRole.RoleName;
                    get.Description = p.admRole.Description ;
                    get.UserId = p.LoginUserId;
                    get.Status = "Active";
                    get.DateCreated = DateTime.Now ;
                    await _repoRoles.AddAsync(get);
                    int _response =  await _ApplicationDbContext.SaveChanges(p.LoginUserId);
                    if(_response > 0)
                    {
                    ApiResponse.ResponseMessage=  _configuration["Message:AddedSuc"];
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
                var exM = ex == null ? ex.InnerException.Message : ex.Message;
                 ApiResponse.ResponseMessage = "Error Occured";//
                 ApiResponse.ResponseCode = -99;
                 return BadRequest(ApiResponse); 
            }

           
        }
    
    
    }
}