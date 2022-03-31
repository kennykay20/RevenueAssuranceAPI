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
using RevAssuranceWebAPi.AnythingGood.DATA.Models;
using Microsoft.EntityFrameworkCore;
using RevAssuranceApi.RevenueAssurance.DATA.Models;
using RevAssuranceApi.RevenueAssurance.Repository.DapperDAL;
using RevAssuranceApi.Response;
using RevAssuranceApi.RevenueAssurance.Repository.Interface;
using RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO;
using RevAssuranceApi.OperationImplemention;


namespace RevAssuranceApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [EnableCors("CorsPolicy")]
    public class DocumentChgController : ControllerBase
    {
        IConfiguration _configuration;
        ApiResponse ApiResponse = new ApiResponse();
        TokenGenerator TokenGenerator;  
        AppSettingsPath AppSettingsPath ;
         IDbConnection db = null;
         ApplicationDbContext   _ApplicationDbContext;
         IRepository<admDocumentChg> _repoDocChg;
          RoleAssignImplementation _RoleAssignImplementation;

          public DocumentChgController(IConfiguration configuration,
                                     ApplicationDbContext   ApplicationDbContext,
                                     IRepository<admDocumentChg> repoDocChg,
                                     RoleAssignImplementation RoleAssignImplementation)
          {
                _configuration = configuration;
           AppSettingsPath = new AppSettingsPath(_configuration);
           TokenGenerator = new TokenGenerator(_configuration);
           db = new SqlConnection(AppSettingsPath.GetDefaultCon());
           _ApplicationDbContext =  ApplicationDbContext;
           _repoDocChg = repoDocChg;
           _RoleAssignImplementation = RoleAssignImplementation;
          }

        [HttpPost("GetAll")]
        public async Task<IActionResult> GetAll(ParamLoadPage AnyAuth)
        {
            try
            {
                 var roleAssign = await _RoleAssignImplementation.GetRoleAssign(AnyAuth.MenuId,AnyAuth.RoleId);
                
                var rtn = new DapperDATAImplementation<admDocumentChg>();

                string script = "Select * from admDocumentChg";
                
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

        [HttpPost("Update")]
        public async Task<IActionResult> Update(admDocumentChg p)
        {
            try
            {
                var get = _ApplicationDbContext.admDocumentChg.FirstOrDefault(c => c.DocumentId == p.DocumentId);
                if (get != null)
                {
                    if(get.Description == p.Description && get.PeriodStart == p.PeriodStart && get.PeriodEnd == p.PeriodEnd
                        && get.ServiceId == p.ServiceId && get.ChgMetrix == p.ChgMetrix && get.Status == p.Status
                        && get.ChgBasis == p.ChgBasis && get.ChgAmount == p.ChgAmount
                        )
                    {
                         ApiResponse.ResponseMessage = _configuration["Message:NoUpdate"];
                         ApiResponse.ResponseCode = -99;
                         return Ok(ApiResponse);
                    }
                    get.Description = p.Description;
                    get.PeriodStart = p.PeriodStart;
                    get.PeriodEnd = p.PeriodEnd;
                    get.ServiceId = p.ServiceId;
                    get.ChgMetrix =p.ChgMetrix;
                    get.ChgBasis = p.ChgBasis;
                    get.Status = p.Status;
                     get.ChgAmount = p.ChgAmount;
                }
                _ApplicationDbContext.admDocumentChg.Update(get);
                int _response = await _ApplicationDbContext.SaveChanges(p.UserId);
                if (_response > 0)
                {
                    ApiResponse.ResponseMessage = _configuration["Message:UpdateSuc"];
                    ApiResponse.ResponseCode = 2;
                    return Ok(ApiResponse);
                }
                 return BadRequest(new ApiResponse
                {
                    ResponseMessage = "Could Not Update",
                    ResponseCode = -99
                });

            }
            catch (Exception ex)
            {
                ApiResponse.ResponseMessage = ex == null ? ex.InnerException.Message : ex.Message;

                ApiResponse.ResponseCode = -99;
                return BadRequest(ApiResponse);
            }
        }

        [HttpPost("Add")]
        public async Task<IActionResult> Add(admDocumentChg p)
        {
            try
            {
                p.DateCreated = DateTime.Now;
                p.Status = _configuration["Statuses:ActiveStatus"];
                _ApplicationDbContext.admDocumentChg.Add(p);
                int _response = await _ApplicationDbContext.SaveChanges(p.UserId);
                if (_response > 0)
                {
                    ApiResponse.ResponseMessage = _configuration["Message:AddedSuc"];
                    return Ok(ApiResponse);
                }
               
                return BadRequest(new ApiResponse
                {
                    ResponseMessage = "Could Not Save",
                    ResponseCode = -99
                });
                
            }
            catch (Exception ex)
            {
                ApiResponse.ResponseMessage = ex.InnerException.Message ?? ex.Message;
                ApiResponse.ResponseCode = -99;
                return BadRequest(ApiResponse);
            }
        }

    }
}