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
using RevAssuranceApi.AppSettings;

using RevAssuranceApi.TokenGen;
using Dapper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO;
using RevAssuranceWebAPi.AnythingGood.DATA.Models;
using RevAssuranceApi.OperationImplemention;
using RevAssuranceApi.RevenueAssurance.DATA.Models;
using RevAssuranceApi.RevenueAssurance.Repository.DapperDAL;
using RevAssuranceApi.RevenueAssurance.Repository.Interface;
using RevAssuranceApi.Response;
using Microsoft.AspNetCore.Authorization;

namespace RevAssuranceApi.Controllers
{
  [Route("api/v1/[controller]")]
    [ApiController]
     [EnableCors("CorsPolicy")]
     // [Authorize]
    public class ServiceController : ControllerBase
    {
          
         IConfiguration _configuration;
        ApiResponse ApiResponse = new ApiResponse();
        TokenGenerator TokenGenerator;  
        AppSettingsPath AppSettingsPath ;
         IDbConnection db = null;
         ApplicationDbContext   _ApplicationDbContext;
         RoleAssignImplementation _RoleAssignImplementation;
        public ServiceController(IConfiguration configuration,
                                ApplicationDbContext   ApplicationDbContext,
                                 RoleAssignImplementation RoleAssignImplementation) 
        {
           _configuration = configuration;
           AppSettingsPath = new AppSettingsPath(_configuration);
           TokenGenerator = new TokenGenerator(_configuration);
           db = new SqlConnection(AppSettingsPath.GetDefaultCon());
           _ApplicationDbContext =  ApplicationDbContext;
           _RoleAssignImplementation = RoleAssignImplementation;
        }
   
    
        [HttpPost("GetAllServices")]
        public async Task<IActionResult> GetAllServices(ParamLoadPage AnyAuth)
        {
            try
            {
                  var roleAssign = await _RoleAssignImplementation.GetRoleAssign(AnyAuth.MenuId,AnyAuth.RoleId);
                
                var rtn = new DapperDATAImplementation<admServiceRead>();

                string script = @"Select 
                        *,
                        (Select DeptName from admDepartment b where b.DeptId =  a.DefaultDept) DeptName
                        from admService a";
                
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
                 ApiResponse.ResponseMessage =  ex.InnerException.Message ?? ex.Message;
                
                 ApiResponse.ResponseCode = -99;
                 return BadRequest(ApiResponse); 
            }
        }
    
        [HttpPost("Add")]
        public async Task<IActionResult> Add(admService p)
        {
            try
            {
                p.DateCreated = DateTime.Now;
                p.Status =  _configuration["Statuses:ActiveStatus"];
               _ApplicationDbContext.admService.Add(p);
              int _response =  await _ApplicationDbContext.SaveChanges(p.UserId);
                if(_response > 0)
                {
                    ApiResponse.ResponseMessage =  _configuration["Message:AddedSuc"];
                    return Ok(ApiResponse);
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
    
         [HttpPost("Update")]
        public async Task<IActionResult> Update(admService p)
        {
            try
            {
                var get =  _ApplicationDbContext.admService.FirstOrDefault(c=>c.ServiceId  == p.ServiceId);
                if(get != null)
                 {

                    get.ServiceDescription = p.ServiceDescription;
                    get.UserId =  p.UserId ;
                    get.ContDrAcctNo  =  p.ContDrAcctNo ;
                    get.ContDrAcctType  =  p.ContDrAcctType ;
                    get.ContDrAcctNarr  =  p.ContDrAcctNarr ;
                    get.ContCrAcctNo  =  p.ContCrAcctNo ;
                    get.ContCrAcctType  =  p.ContCrAcctType ;
                    get.ContCrAcctNarr  =  p.ContCrAcctNarr ;
                    get.DefaultDept  =  p.DefaultDept ;
                    get.Status  =  p.Status ;
                    get.IncomeAcctNo  =  p.IncomeAcctNo ;
                    get.IncomeAcctType  =  p.IncomeAcctType ;
                    get.Channel  =  p.Channel ;
                    get.Frequency  =  p.Frequency ;
                    get.Sequence  =  p.Sequence ;
                    get.RefPrefix  =  p.RefPrefix ;
                    get.TransactionDate  =  p.TransactionDate ;
                    get.CustAcctDrTC  =  p.CustAcctDrTC ;
                    get.GLAcctDrTC  =  p.GLAcctDrTC ;
                    get.CustAcctCrTC  =  p.CustAcctCrTC ;
                    get.GLAcctCrTC  =  p.GLAcctCrTC ;
                    get.ReqAmortSched  =  p.ReqAmortSched ;

                 }
               _ApplicationDbContext.admService.Update(get);
              int _response =  await _ApplicationDbContext.SaveChanges(p.UserId);
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
    }

}