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
    public class AmmendAndReprintController : ControllerBase
    {
        IConfiguration _configuration;
        ApiResponse ApiResponse = new ApiResponse();
        ReturnValues ReturnValues = new ReturnValues();
        TokenGenerator TokenGenerator;
        AppSettingsPath AppSettingsPath;
        IDbConnection db = null;
        ApplicationDbContext _ApplicationDbContext;

        Formatter _Formatter = new Formatter();
        ChargeImplementation _ChargeImplementation;
        ServiceChargeImplementation _ServiceChargeImplementation;
        IRepository<oprServiceCharges> _repooprServiceCharge;
        IRepository<OprAmmendAndReprint> _repoOprAmmendAndReprint;
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
        public AmmendAndReprintController(IConfiguration configuration,
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
                                         IRepository<CbsTransaction> repoCbsTransaction,
                                         IRepository<OprAmortizationSchedule> repoOprAmortizationSchedule,
                                         IRepository<OprAmmendAndReprint> repoOprAmmendAndReprint)
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
            _repoOprAmmendAndReprint = repoOprAmmendAndReprint;
        }

        [HttpPost("GetAll")]
        public async Task<IActionResult> GetAll(AmmendReprintParam AnyAuth)
        {
            try
            {
                var RoleAssign = await _RoleAssignImplementation.GetRoleAssign(AnyAuth.MenuId,AnyAuth.RoleId);
                string IsGlobalView = "N";
                if(RoleAssign != null)
                {
                    IsGlobalView = RoleAssign.IsGlobalSupervisor == true ?  "Y" : "N"; 
                }

                DynamicParameters param = new DynamicParameters();

                AnyAuth.psBranchCode = AnyAuth.psBranchCode  == string.Empty ? null : AnyAuth.psBranchCode ;
                AnyAuth.psCurrencyIso  = AnyAuth.psCurrencyIso == string.Empty ? null : AnyAuth.psCurrencyIso;
                AnyAuth.psAcctNo = AnyAuth.psAcctNo == string.Empty ? null : AnyAuth.psAcctNo;
                AnyAuth.pdtStartDate = AnyAuth.pdtStartDate == string.Empty ? null : AnyAuth.pdtStartDate;
                AnyAuth.pdtEndDate = AnyAuth.pdtEndDate == string.Empty ? null : AnyAuth.pdtEndDate;
                AnyAuth.psTranRef =  AnyAuth.psTranRef == string.Empty ? null :  AnyAuth.psTranRef;
                AnyAuth.pnAmnt = AnyAuth.pnAmnt  == string.Empty ? null : AnyAuth.pnAmnt ;
                AnyAuth.pnServiceId = AnyAuth.pnServiceId == string.Empty ? null : AnyAuth.pnServiceId;

                DateTime startdt, enddt;
                if (AnyAuth.pdtStartDate != null)
                {
                    startdt = Convert.ToDateTime(AnyAuth.pdtStartDate);
                    AnyAuth.pdtStartDate = string.Format("{0:yyyyMMdd}", startdt);
                }
                if (AnyAuth.pdtStartDate != null)
                {
                    enddt = Convert.ToDateTime(AnyAuth.pdtEndDate);
                    AnyAuth.pdtEndDate = string.Format("{0:yyyyMMdd}", enddt);
                }

                param.Add("@psBranchCode", AnyAuth.psBranchCode);
                param.Add("@psCurrencyIso", AnyAuth.psCurrencyIso);
                param.Add("@psAcctNo", AnyAuth.psAcctNo);
                param.Add("@pdtStartDate", AnyAuth.pdtStartDate);
                param.Add("@pdtEndDate", AnyAuth.pdtEndDate);
                param.Add("@psTranRef", AnyAuth.psTranRef);
                param.Add("@pnAmnt", AnyAuth.pnAmnt);
                param.Add("@pnServiceId", AnyAuth.pnServiceId);
                
                var rtn = new DapperDATAImplementation<CbsTransactionDTO>();
                var _response = await rtn.LoadData("Isp_AmmendmentReprintList", param, db);
                if (_response != null)
                {
                    var res = new
                    {
                        _response = _response,
                        RoleAssign = RoleAssign
                      
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

        [HttpPost("Add")]
        public async Task<IActionResult> Add(OprAmmendAndReprintDTO p)
        {
            try
            {
                var oprAmmendAndReprint = new OprAmmendAndReprint();

                oprAmmendAndReprint = p.OprAmmendAndReprint;

                var serviceRef = await _ComputeChargesImplementation.GenServiceRef(p.ServiceId);
          
                oprAmmendAndReprint.DateCreated = DateTime.Now;
                oprAmmendAndReprint.UserId = p.LoginUserId;
                await _repoOprAmmendAndReprint.AddAsync(oprAmmendAndReprint);
                var retV1 = await _ApplicationDbContext.SaveChanges(p.LoginUserId);
                if (retV1 > 0)
                {  
                    if(oprAmmendAndReprint.RequireCharge == true)
                    {
                        // cal charge procedure
                    }
                    ApiResponse.ResponseMessage = "Record Saved Successfully and Forwarded for Authorization";
                    ApiResponse.ResponseCode = 0;
                    return Ok(ApiResponse);
                }
            }
            catch (Exception ex)
            {
                ApiResponse.ResponseMessage = ex == null ? ex.InnerException.Message : ex.Message;

                ApiResponse.ResponseCode = -99;
                return BadRequest(ApiResponse);
            }
            ApiResponse.ResponseMessage = "Record Not Saved";
            return BadRequest(ApiResponse);
        }
 
    }
}