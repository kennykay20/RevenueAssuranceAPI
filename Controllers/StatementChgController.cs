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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RevAssuranceApi.Response;
using RevAssuranceApi.RevenueAssurance.DATA.Models;
using RevAssuranceApi.RevenueAssurance.Repository.DapperDAL;
using RevAssuranceApi.RevenueAssurance.Repository.Interface;
using RevAssuranceWebAPi.AnythingGood.DATA.Models;
using RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO;
using RevAssuranceApi.OperationImplemention;


namespace RevAssuranceApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [EnableCors("CorsPolicy")]
    public class StatementChgController : ControllerBase
    {
        IConfiguration _configuration;
        ApiResponse ApiResponse = new ApiResponse();
        TokenGenerator TokenGenerator;
        AppSettingsPath AppSettingsPath;
        IDbConnection db = null;
        ApplicationDbContext _ApplicationDbContext;
        IRepository<admStatementChg> _repoStatementChg;
        RoleAssignImplementation _RoleAssignImplementation;

        public StatementChgController(IConfiguration configuration,
                                   ApplicationDbContext ApplicationDbContext,
                                   IRepository<admStatementChg> repoStatementChg,
                                   RoleAssignImplementation RoleAssignImplementation)
        {
            _configuration = configuration;
            AppSettingsPath = new AppSettingsPath(_configuration);
            TokenGenerator = new TokenGenerator(_configuration);
            db = new SqlConnection(AppSettingsPath.GetDefaultCon());
            _ApplicationDbContext = ApplicationDbContext;
            _repoStatementChg = repoStatementChg;
            _RoleAssignImplementation = RoleAssignImplementation;
        }

        [HttpPost("GetAll")]
        public async Task<IActionResult> GetAll(ParamLoadPage AnyAuth)
        {
            try
            {
                var roleAssign = await _RoleAssignImplementation.GetRoleAssign(AnyAuth.MenuId, AnyAuth.RoleId);

                var rtn = new DapperDATAImplementation<admStatementChg>();

                string script = "Select * from admStatementChg";

                var _response = await rtn.LoadListNoParam(script, db);

                if (_response != null)
                {
                    var res = new
                    {
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
                ApiResponse.ResponseMessage = ex.InnerException.Message ?? ex.Message;

                ApiResponse.ResponseCode = -99;
                return BadRequest(ApiResponse);
            }


        }

        [HttpPost("Add")]
        public async Task<IActionResult> Add(admStatementChg p)
        {
            try
            {
                p.DateCreated = DateTime.Now;
                p.Status = _configuration["Statuses:ActiveStatus"];
                _ApplicationDbContext.admStatementChg.Add(p);
                int _response = await _ApplicationDbContext.SaveChanges(p.UserId);
                if (_response > 0)
                {
                    ApiResponse.ResponseMessage = _configuration["Message:AddedSuc"];
                    return Ok(ApiResponse);
                }
                else
                {
                    return BadRequest(new ApiResponse
                    {
                        ResponseMessage = "Could Not Save",
                        ResponseCode = -99
                    });
                }
            }
            catch (Exception ex)
            {
                ApiResponse.ResponseMessage = ex.InnerException.Message ?? ex.Message;
                ApiResponse.ResponseCode = -99;
                return BadRequest(ApiResponse);
            }
        }


        [HttpPost("Update")]
        public async Task<IActionResult> Update(admStatementChg p)
        {
            try
            {
                var get = _ApplicationDbContext.admStatementChg.FirstOrDefault(c => c.DocumentId == p.DocumentId);
                if (get != null)
                {
                    get.Description = p.Description;
                    get.StartingNo = p.StartingNo;
                    get.EndingNo = p.EndingNo;
                    get.ServiceId = p.ServiceId;
                    get.ChgMetrix =p.ChgMetrix;
                    get.ChgBasis = p.ChgBasis;
                    get.Status = p.Status;
                }
                _ApplicationDbContext.admStatementChg.Update(get);
                int _response = await _ApplicationDbContext.SaveChanges(p.UserId);
                if (_response > 0)
                {
                    ApiResponse.ResponseMessage = _configuration["Message:UpdateSuc"];
                    return Ok(ApiResponse);
                }
                else
                {
                    return BadRequest(_response);
                }


            }
            catch (Exception ex)
            {
                ApiResponse.ResponseMessage = ex == null ? ex.InnerException.Message : ex.Message;

                ApiResponse.ResponseCode = -99;
                return BadRequest(ApiResponse);
            }
        }
    }
}