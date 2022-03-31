using System.Net;
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
using RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO;
using RevAssuranceWebAPi.AnythingGood.DATA.Models;
using RevAssuranceApi.OperationImplemention;
using RevAssuranceApi.RevenueAssurance.DATA.Models;
using RevAssuranceApi.RevenueAssurance.Repository.DapperDAL;
using RevAssuranceApi.RevenueAssurance.Repository.Interface;
using RevAssuranceApi.Response;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http;
using Newtonsoft.Json;

namespace RevAssuranceApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [EnableCors("CorsPolicy")]
    //[Authorize]
    public class TransEnquiryController : ControllerBase
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
        IRepository<oprStatementReq> _repoprStatementReq;
        IRepository<admClientProfile> _repoadmClientProfile;
        IRepository<admService> _repoadmService;
        IRepository<admCharge> _repoadmCharge;
        IRepository<admBankBranch> _repoadmBankBranch;
        IRepository<CbsTransaction> _repoCbsTransaction;
        IRepository<admBankServiceSetup> _repoadmBankServiceSetup;
        AccountValidationImplementation _AccountValidationImplementation;
        ApplicationReturnMessageImplementation _ApplicationReturnMessageImplementation;
        ComputeChargesImplementation _ComputeChargesImplementation;
        UsersImplementation _UsersImplementation;
        HeaderLogin _HeaderLogin;
        RoleAssignImplementation _RoleAssignImplementation;
        CBSTransImplementation _CBSTransImplementation;
        CoreBankingImplementation _CoreBankingImplementation;

        ApprovalValidation _ApprovalValidation;
        ApplicationReturnMessage _ApplicationReturnMessage;
        public TransEnquiryController(IConfiguration configuration,
                                        ApplicationDbContext ApplicationDbContext,
                                        ChargeImplementation ChargeImplementation,
                                        ServiceChargeImplementation ServiceChargeImplementation,
                                         IRepository<oprServiceCharges> repooprServiceCharge,
                                         IRepository<admCharge> repoadmCharge,
                                         IRepository<admBankBranch> repoadmBankBranch,
                                         AccountValidationImplementation AccountValidationImplementation,
                                         ApplicationReturnMessageImplementation ApplicationReturnMessageImplementation,
                                         ComputeChargesImplementation ComputeChargesImplementation,
                                         UsersImplementation UsersImplementation,
                                         HeaderLogin HeaderLogin,
                                         IRepository<OprInstrument> repoOprInstrument,
                                         RoleAssignImplementation RoleAssignImplementation,
                                         IRepository<admService> repoadmService,
                                         IRepository<oprStatementReq> repoprStatementReq,
                                         CBSTransImplementation CBSTransImplementation,
                                         CoreBankingImplementation CoreBankingImplementation,
                                         ApprovalValidation ApprovalValidation,
                                         ApplicationReturnMessage ApplicationReturnMessage,
                                         IRepository<admClientProfile> repoadmClientProfile,
                                         IRepository<admBankServiceSetup> repoadmBankServiceSetup,
                                         IRepository<CbsTransaction> repoCbsTransaction)
        {
            _configuration = configuration;
            AppSettingsPath = new AppSettingsPath(_configuration);
            TokenGenerator = new TokenGenerator(_configuration);
            db = new SqlConnection(AppSettingsPath.GetDefaultCon());
            _ApplicationDbContext = ApplicationDbContext;
            _ChargeImplementation = ChargeImplementation;
            _ServiceChargeImplementation = ServiceChargeImplementation;
            _repooprServiceCharge = repooprServiceCharge;
            _repoprStatementReq = repoprStatementReq;
            _repoadmCharge = repoadmCharge;
            _repoadmBankBranch = repoadmBankBranch;
            _AccountValidationImplementation = AccountValidationImplementation;
            _ApplicationReturnMessageImplementation = ApplicationReturnMessageImplementation;
            _ComputeChargesImplementation = ComputeChargesImplementation;
            _UsersImplementation = UsersImplementation;
            _HeaderLogin = HeaderLogin;
            _RoleAssignImplementation = RoleAssignImplementation;
            _repoadmService = repoadmService;
            _CBSTransImplementation = CBSTransImplementation;
            _CoreBankingImplementation = CoreBankingImplementation;
            _ApprovalValidation = ApprovalValidation;
            _ApplicationReturnMessage = ApplicationReturnMessage;
            _repoadmClientProfile = repoadmClientProfile;
            _repoadmBankServiceSetup = repoadmBankServiceSetup;
            _repoCbsTransaction = repoCbsTransaction;
        }

        [HttpPost("GetAll")]
        public async Task<IActionResult> GetAll(TranEnqParam AnyAuth)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();

                AnyAuth.pdtStartDate = _Formatter.FormatToDateYearMonthDay(AnyAuth.pdtStartDate);
                AnyAuth.pdtEndDate = _Formatter.FormatToDateYearMonthDay(AnyAuth.pdtEndDate);

                param.Add("@pnUserId", AnyAuth.pnUserId);
                param.Add("@psBranchCode", AnyAuth.psBranchCode);
                param.Add("@psCurrencyIso", AnyAuth.psCurrencyIso);
                param.Add("@psDrAcctNo", AnyAuth.psDrAcctNo);
                param.Add("@psCrAcctNo", AnyAuth.psCrAcctNo);
                param.Add("@pdtStartDate", AnyAuth.pdtStartDate);
                param.Add("@pdtEndDate", AnyAuth.pdtEndDate);
                param.Add("@psTranRef", AnyAuth.psTranRef);
                param.Add("@psStatus", AnyAuth.psStatus);
                param.Add("@pnAmnt", AnyAuth.pnAmnt == null ? null : AnyAuth.pnAmnt.Replace(",", ""));
                param.Add("@pnServiceId", AnyAuth.pnServiceId);
                param.Add("@pnDeptId", AnyAuth.pnDeptId);
                param.Add("@pnbatchId", AnyAuth.pnbatchId);
                
                var rtn = new DapperDATAImplementation<CbsTransactionDTO>();
                var _response = await rtn.LoadData("Isp_TransEnquiryList", param, db);
                if (_response != null)
                {
                    var res = new
                    {
                        _response = _response,
                      
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
                ApiResponse.ResponseMessage = ex == null ? ex.InnerException.Message : ex.Message;

                ApiResponse.ResponseCode = -99;
                return BadRequest(ApiResponse);
            }
        }


        [HttpPost("GetAllStatus")]
        public async Task<IActionResult> GetAllStatus()
        {
            try
            {
                DynamicParameters param = new DynamicParameters();

                var sql = "select Distinct Status from CbsTransaction";
                
                var rtn = new DapperDATAImplementation<CbsStatusDTO>();
                var _response = await rtn.LoadListNoParam(sql, db);
                if (_response != null)
                {
                    var res = new
                    {
                        _response = _response,
                      
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
                ApiResponse.ResponseMessage = ex == null ? ex.InnerException.Message : ex.Message;

                ApiResponse.ResponseCode = -99;
                return BadRequest(ApiResponse);
            }
        }

   

     



    }
}