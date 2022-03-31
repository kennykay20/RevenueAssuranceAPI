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
using RevAssuranceApi.Helper;

namespace RevAssuranceApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [EnableCors("CorsPolicy")]
    //[Authorize]
    public class ConnectionTestController : ControllerBase
    {

        IConfiguration _configuration;
        ApiResponse ApiResponse = new ApiResponse();
        TokenGenerator TokenGenerator;
        AppSettingsPath AppSettingsPath;
        IDbConnection db = null;
        ApplicationDbContext _ApplicationDbContext;

        Formatter _Formatter = new Formatter();
        ChargeImplementation _ChargeImplementation;
        ServiceChargeImplementation _ServiceChargeImplementation;
        IRepository<oprServiceCharges> _repooprServiceCharge;
        IRepository<OprInstrument> _repoOprInstrument;
        IRepository<oprCollateral> _repooprCollateral;
        IRepository<admService> _repoadmService;

        IRepository<admCharge> _repoadmCharge;

        IRepository<admBankBranch> _repoadmBankBranch;
        AccountValidationImplementation _AccountValidationImplementation;
        ApplicationReturnMessageImplementation _ApplicationReturnMessageImplementation;
        ComputeChargesImplementation _ComputeChargesImplementation;
        UsersImplementation _UsersImplementation;
        HeaderLogin _HeaderLogin;
        RoleAssignImplementation _RoleAssignImplementation;
        IRepository<admDepartment> _repoadmDepartment;
        IRepository<admTemplate> _repoadmTemplate;
        IRepository<oprInstrmentTemp> _repooprInstrmentTem;
        IRepository<OprAmmendAndReprint> _repoOprAmmendAndReprint;
        AmmendReprintReasonImplementation _AmmendReprintReasonImplementation;
        IRepository<OprAmortizationSchedule> _repoOprAmortizationSchedule;
        LogManager _LogManager;
     
        public ConnectionTestController(IConfiguration configuration,
                                        ApplicationDbContext ApplicationDbContext,LogManager LogManager
                                          )
        {
            _configuration = configuration;
            AppSettingsPath = new AppSettingsPath(_configuration);
            TokenGenerator = new TokenGenerator(_configuration);
            db = new SqlConnection(AppSettingsPath.GetDefaultCon());
            _ApplicationDbContext = ApplicationDbContext;
            _LogManager = LogManager;
        }

        [HttpPost("TestCon")]
        public async Task<IActionResult> TestCon(ConnectDBParam q)
        {
            if (q != null)
            {
                try
                {
                    var ApiResponse = new ApiDBConResponse();
                    string Con = $"Data Source={q.IP};Initial Catalog={q.DBName};user id = {q.UserName}; password = {q.Password};";

                    using (var con = new SqlConnection(Con))
                    {
                        con.Open();

                        con.Close();
                        ApiResponse.ResponseCode = 0;
                        ApiResponse.ResponseMessage = "Connection Successful";
                        return Ok(ApiResponse);
                    }
                }
                catch (Exception ex)
                {
                    ApiResponse.ResponseMessage = ex.InnerException.Message ?? ex.Message;
                    ApiResponse.ResponseCode = -99;
                    ApiResponse.ResponseMessage = "Error Occured";
                    return BadRequest(ApiResponse);
                }
            }
            ApiResponse.ResponseMessage = "Enter Scripts to be Executed";
            return BadRequest(ApiResponse);

        }
    
    
    [HttpPost("TestConForEnc")]
        public async Task<IActionResult> TestConForEnc(ConnectDBParam q)
        {
            if (q != null)
            {
                try
                {
                    var ApiResponse = new ApiDBConResponse();
                    string Con =  AppSettingsPath.GetDefaultCon();

                    _LogManager.SaveLog($"Ful Con {Con}");
                    _LogManager.SaveLog($"TestConForEnc 1");
                    using (var con = new SqlConnection(Con))
                    {
                        _LogManager.SaveLog($"TestConForEnc 2");

                        con.Open();

                        con.Close();
                        ApiResponse.ResponseCode = 0;
                        ApiResponse.ResponseMessage = "Connection Successful";
                        
                        return Ok(ApiResponse);
                    }

                   
                }
                catch (Exception ex)
                {
                    var exM = ex == null ? ex.InnerException.Message : ex.Message;
                    // _LogManager.SaveLog($"Ful Con EX trace ConnectionTestController {ex.StackTrace}");

                     _LogManager.SaveLog($"Con exM trace inne {exM}");

                    ApiResponse.ResponseMessage = ex.InnerException.Message ?? ex.Message;
                    ApiResponse.ResponseCode = -99;
                    ApiResponse.ResponseMessage = "Error Occured";
                    
                    
                    
                    return BadRequest(ApiResponse);
                }
            }

             _LogManager.SaveLog($"No error");
            ApiResponse.ResponseMessage = "Enter Scripts to be Executed";
            return BadRequest(ApiResponse);

        }
    
    }
}