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
using RevAssuranceWebAPi.AnythingGood.DATA.Models;
using Microsoft.EntityFrameworkCore;
using RevAssuranceApi.Response;
using RevAssuranceApi.TokenGen;
using RevAssuranceApi.AppSettings;
using RevAssuranceApi.RevenueAssurance.DATA.Models;
using RevAssuranceApi.RevenueAssurance.Repository.DapperDAL;
using RevAssuranceApi.OperationImplemention;
using RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO;

namespace RevAssuranceApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [EnableCors("CorsPolicy")]
    public class TransConfigController : ControllerBase
    {
        
         IConfiguration _configuration;
        ApiResponse ApiResponse = new ApiResponse();
        TokenGenerator TokenGenerator;  
        AppSettingsPath AppSettingsPath ;
         IDbConnection db = null;
         ApplicationDbContext   _ApplicationDbContext;
         RoleAssignImplementation _RoleAssignImplementation;

         public TransConfigController(IConfiguration configuration,
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

        [HttpPost("GetAll")]
        public async Task<IActionResult> GetAll(ParamLoadPage AnyAuth)
        {
            try
            {
                   var roleAssign = await _RoleAssignImplementation.GetRoleAssign(AnyAuth.MenuId,AnyAuth.RoleId);
                
                var rtn = new DapperDATAImplementation<admTransactionConfiguration>();

                string script = "Select * from admTransactionConfiguration";
                
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
        public async Task<IActionResult> Add(admTransactionConfiguration p)
        {
            try
            {
                p.DateCreated = DateTime.Now;
                p.Status =  _configuration["Statuses:ActiveStatus"];
               _ApplicationDbContext.admTransactionConfiguration.Add(p);
              int _response =  await _ApplicationDbContext.SaveChanges(p.UserId);
                if(_response > 0)
                {
                    ApiResponse.ResponseMessage =  _configuration["Message:AddedSuc"];
                    return Ok(ApiResponse);
                }
                else
                {
                    ApiResponse.ResponseMessage = "Record could'nt be Created";
                    return BadRequest(ApiResponse); 
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
        public async Task<IActionResult> Update(admTransactionConfiguration p)
        {
            try
            {
                var get =  _ApplicationDbContext.admTransactionConfiguration.FirstOrDefault(c=>c.ItbId  == p.ItbId);
                if(get != null)
                 {
                     get.ServiceId = p.ServiceId;
                     get.Direction = p.Direction;
                     get.CustomerAcctCrTC = p.CustomerAcctCrTC;
                     get.CustomerAcctDrTC = p.CustomerAcctDrTC;
                     get.DrNarration = p.DrNarration;
                    get.CrNarration = p.CrNarration;
                    get.GLAcctDrTC = p.GLAcctDrTC;
                    get.GLAcctCrTC = p.GLAcctCrTC;
                    get.DrChargeNarr = p.DrChargeNarr;
                    get.CrChargeNarr = p.CrChargeNarr;
                    get.CreditAcctTCRev = p.CreditAcctTCRev;
                    get.DebitAcctTCRev = p.DebitAcctTCRev;
                    get.GLAcctCrTCRev = p.GLAcctCrTCRev;
                    get.GLAcctDrTCRev = p.GLAcctDrTCRev;
                    get.ChargeType = p.ChargeType;
                    get.Status = p.Status;
                   
                    
        
                  }
               _ApplicationDbContext.admTransactionConfiguration.Update(get);
              int _response =  await _ApplicationDbContext.SaveChanges(p.UserId);
                if(_response > 0)
                {
                   ApiResponse.ResponseMessage=  _configuration["Message:UpdateSuc"];
                   return Ok(ApiResponse);
                }
                else
                { 
                    ApiResponse.ResponseMessage = "Record could'nt Update";
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