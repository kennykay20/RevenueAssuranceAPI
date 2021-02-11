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
using Dapper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RevAssuranceApi.AppSettings;
using RevAssuranceApi.OperationImplemention;
using RevAssuranceApi.Response;
using RevAssuranceApi.RevenueAssurance.DATA.Models;
using RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO;
using RevAssuranceApi.RevenueAssurance.Repository.DapperDAL;
using RevAssuranceApi.RevenueAssurance.Repository.Interface;
using RevAssuranceApi.TokenGen;
using RevAssuranceWebAPi.AnythingGood.DATA.Models;

namespace RevAssuranceApi.Controllers
{
      [Route("api/v1/[controller]")]
    [ApiController]
    [EnableCors("CorsPolicy")]
    public class BroadCastMessageController : ControllerBase
    {
        IConfiguration _configuration;
        ApiResponse ApiResponse = new ApiResponse();
        TokenGenerator TokenGenerator;  
        AppSettingsPath AppSettingsPath ;
         IDbConnection db = null;
         ApplicationDbContext   _ApplicationDbContext;
         RoleAssignImplementation _RoleAssignImplementation;
         IRepository<admBroadCast> _repoadmBroadCast;
         IRepository<admUserProfile> _repoadmUserProfile;
        public BroadCastMessageController(IConfiguration configuration,
                        ApplicationDbContext   ApplicationDbContext,  
                        RoleAssignImplementation RoleAssignImplementation,
                        IRepository<admBroadCast> repoadmBroadCast,
                        IRepository<admUserProfile> repoadmUserProfile)
        {
          
           _configuration = configuration;
           AppSettingsPath = new AppSettingsPath(_configuration);
           TokenGenerator = new TokenGenerator(_configuration);
           db = new SqlConnection(AppSettingsPath.GetDefaultCon());
           _ApplicationDbContext =  ApplicationDbContext;
           _RoleAssignImplementation = RoleAssignImplementation;
           _repoadmBroadCast = repoadmBroadCast;
           _repoadmUserProfile = repoadmUserProfile;
        }


        
         [HttpPost("GetAll")]
        public async Task<IActionResult> GetAll(ParamLoadPage AnyAuth)
        {
            try
            {
                 var roleAssign = await _RoleAssignImplementation.GetRoleAssign(AnyAuth.MenuId,AnyAuth.RoleId);
                
                var rtn = new DapperDATAImplementation<admBroadCastDTO>();

                string script = @"Select 
                                  (Select DeptName  
                                   from admDepartment where DeptId = a.DeptId) DeptName,
                                   * 
                            from admBroadCast a";
                
                var _response = await rtn.LoadListNoParam(script, db);
            
                if(_response != null)
                {
                    var res = new 
                    { roleAssign = roleAssign,
                    _response = _response
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
                 ApiResponse.ResponseMessage =  ex.InnerException.Message ?? ex.Message;
                
                 ApiResponse.ResponseCode = -99;
                 return BadRequest(ApiResponse); 
            }
        }

 
         [HttpPost("GetAllByDept")]
        public async Task<IActionResult> GetAllByDept(ParamLoadPage AnyAuth)
        {
            try
            {
                 var roleAssign = await _RoleAssignImplementation.GetRoleAssign(AnyAuth.MenuId,AnyAuth.RoleId);
                
                var rtn = new DapperDATAImplementation<admBroadCastDTO>();
                var dt = DateTime.Now;
                string script = $"Select  * from admBroadCast a where deptid = {AnyAuth.pnDeptId} or TargetAudience = 'ALL'  and startdate = '{dt.ToString()}'";
                   
                var _response = await rtn.LoadListNoParam(script, db);
            
                if(_response != null)
                {
                    var res = new 
                    { roleAssign = roleAssign,
                    _response = _response
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
                 ApiResponse.ResponseMessage =  ex.InnerException.Message ?? ex.Message;
                
                 ApiResponse.ResponseCode = -99;
                 return BadRequest(ApiResponse); 
            }
        }

      
        [HttpPost("Add")]
        public async Task<IActionResult> Add(admBroadCast p)
        {
            try
            {
                p.DateCreated = DateTime.Now;
                p.Status = "Active";
                await _repoadmBroadCast.AddAsync(p);
              int save =  await _ApplicationDbContext.SaveChanges(p.UserId);
                if(save > 0)
                {
                   ApiResponse.ResponseMessage=  _configuration["Message:AddedSuc"];
                   return Ok(ApiResponse);
                }
                else
                {
                     ApiResponse.ResponseMessage = "Error Occured";
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
   

        [HttpPost("Update")]
        public async Task<IActionResult> Update(admBroadCast p)
        {
            try
            {
                _ApplicationDbContext.admBroadCast.Update(p);
                int _response =  await _ApplicationDbContext.SaveChanges(p.UserId);
                if(_response > 0)
                {
                    ApiResponse.ResponseMessage=  _configuration["Message:UpdateSuc"];
                    return Ok(ApiResponse);
                }    

                   ApiResponse.ResponseMessage =  "Error Occured"  ;
                  return BadRequest(ApiResponse);  
            
            }
            catch (Exception ex)
            {
                 ApiResponse.ResponseMessage =  ex == null ? ex.InnerException.Message : ex.Message;
                
                 ApiResponse.ResponseCode = -99;
                 return BadRequest(ApiResponse); 
            }
        }
      

        [HttpPost("GetById")]
        public async Task<IActionResult> GetById(admBroadCast p)
        {
            try
            {
                var get = await _repoadmBroadCast.GetAsync(c=>c.ItbId  == p.ItbId);
                var getUser = await _repoadmUserProfile.GetAsync(c=> c.UserId == get.UserId);
                if(get != null)
                 {
                     string startDate =  get.StartDate != null ? get.StartDate.ToString() : string.Empty;  
                   string endDate =  get.EndDate != null ? get.EndDate.ToString() : string.Empty;  
                   
                     var res = new {
                         get = get,
                         getUser = getUser,
                        startDate = startDate,
                        endDate = endDate

                     };

                    return Ok(res);
                  }
                else
                {
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
   
        
    }
}