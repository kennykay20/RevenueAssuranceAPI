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
using RevAssuranceApi.Helper;

namespace RevAssuranceApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [EnableCors("CorsPolicy")]
    //[Authorize]
    public class ReverseTransController : ControllerBase
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
        static CbsTransaction cbsStatic;
         LogManager _LogManager;
        public ReverseTransController(IConfiguration configuration,
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
                                         IRepository<CbsTransaction> repoCbsTransaction, LogManager LogManager)
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
            _LogManager = LogManager;
        }

        [HttpPost("GetAll")]
        public async Task<IActionResult> GetAll(ParamLoadPage AnyAuth)
        {
            try
            {
                var RoleAssign = await _RoleAssignImplementation.GetRoleAssign(AnyAuth.MenuId, AnyAuth.RoleId);

                DynamicParameters param = new DynamicParameters();
                param.Add("@pnUserId", AnyAuth.UserId);
                param.Add("@pnDeptId", AnyAuth.pnDeptId);
                if (!string.IsNullOrEmpty(AnyAuth.psBranchNo))
                {
                    param.Add("@psbranchCode", AnyAuth.psBranchNo);
                }
                if (!string.IsNullOrEmpty(AnyAuth.psBranchNo))
                {
                    param.Add("@psBranches", AnyAuth.psBranchNo);
                }
                if (!string.IsNullOrEmpty(AnyAuth.FromDate))
                {
                    param.Add("@pdtStartDate", AnyAuth.FromDate);
                }
                if (!string.IsNullOrEmpty(AnyAuth.ToDate))
                {
                    param.Add("@pdtEndDate", AnyAuth.ToDate);
                }
                if (!string.IsNullOrEmpty(AnyAuth.Amount))
                {
                    param.Add("@pnAmnt", Convert.ToDecimal(AnyAuth.Amount));
                }
                param.Add("@psIsGlobalSupervisor", RoleAssign.IsGlobalSupervisor);

                var rtn = new DapperDATAImplementation<CbsTransactionDTO>();
                var _response = await rtn.LoadData("Isp_ReversalPostingList", param, db);
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

        [HttpPost("ReverseTxn")]
        public async Task<IActionResult> ReverseTxn(PostTransDTO p)
        {
            StringBuilder sb = new StringBuilder("<ul>");
             var SubtringClass = new List<SubtringClassDTO>();
            int postErrorCode = -999999;
            string postErrorText = string.Empty;
            string TransReference = string.Empty;
            int success = 0;
            int failure = 0;
            int UpdateOld = 0;

            try
            {
                string PostedStatus = _configuration["Statuses:PostedStatus"];
                string SeekAuthorizeStatus = _configuration["Statuses:SeekAuthorizeStatus"];
                string UnPostedStatus = _configuration["Statuses:UnPostedStatus"];

                string Ids = string.Empty, tranIds = string.Empty;

            foreach (var i in p.ListCbsTransactionDTO)
            {
                var cbsReversalLeg = new CbsTransaction();
               
                var cbsOld = await _repoCbsTransaction.GetAsync(c => c.ItbId == i.ItbId);

                cbsStatic = cbsOld;

                var newRev = _CBSTransImplementation.passCbsToNewCBS(cbsStatic);
               
                newRev.Reversal = 1;
               
                var serviceTransRef = await _ComputeChargesImplementation.GenTranRef(cbsStatic.ServiceId);
               
                newRev.ParentTransactionId = cbsStatic.TransReference;

                newRev.UserId = p.LoginUserId;
                newRev.TransReference = serviceTransRef.nReference;
                newRev.Status = UnPostedStatus;

                await _repoCbsTransaction.AddAsync(newRev);

                int save = await _ApplicationDbContext.SaveChanges(p.LoginUserId);
                if (save > 0)
                {
                    cbsReversalLeg = newRev;
                }
                else
                {
                    continue;
                }
                var checkTransExit = await _CBSTransImplementation.ChectTransRefExist((decimal)cbsReversalLeg.ItbId, cbsReversalLeg.TransReference);
                
                if (checkTransExit.nReturnCode == -1)
                {
                    sb.Append($"<li> Failed: Trans. Ref: {i.TransReference} Failed. Reason: { checkTransExit.sReturnMsg } </li>");

                    failure++;
                    ApiResponse.ResponseCode = checkTransExit.nReturnCode;
                    ApiResponse.ResponseMessage = checkTransExit.sReturnMsg;
                    continue;
                }

                var postResp = await _CBSTransImplementation.PostTransactionsEthix(cbsReversalLeg, p.LoginUserName);

                if (postResp != null)
                {
                    if (postResp.nErrorCode == 0)
                    {
                        cbsReversalLeg.PostingErrorCode = postResp.nErrorCode;
                        cbsReversalLeg.PostingErrorDescr = postResp.sErrorText;
                        cbsReversalLeg.UserId = p.UserId;
                        cbsReversalLeg.CbsUserId = p.LoginUserName;
                        cbsReversalLeg.DrAcctCashAmt = !string.IsNullOrEmpty(postResp.drCashAmt) ? decimal.Parse(postResp.drCashAmt) : 0.00m;
                        cbsReversalLeg.CrAcctCbsTranId = _Formatter.ValDecimal(postResp.nCbsTranId);
                        cbsReversalLeg.DrAcctBalAfterPost = !string.IsNullOrEmpty(postResp.nBalance) ? decimal.Parse(postResp.nBalance) : 0.00m;
                        cbsReversalLeg.Status = PostedStatus;
                        cbsReversalLeg.PostingDate = DateTime.Now;
                        cbsReversalLeg.UserId = p.UserId;

                        _repoCbsTransaction.Update(cbsReversalLeg);

                        var updateReversalLeg = await _ApplicationDbContext.SaveChanges(p.LoginUserId);

                        cbsOld.Reversal = 2;

                        _repoCbsTransaction.Update(cbsOld);

                        UpdateOld = await _ApplicationDbContext.SaveChanges(p.LoginUserId);

                        if (UpdateOld > 0 && updateReversalLeg > 0)
                        {
                            success++;
                            ApiResponse.ResponseCode = postResp.nErrorCode;
                            ApiResponse.ResponseMessage = postResp.sErrorText;

                             SubtringClass.Add(new SubtringClassDTO { TransTracer = i.TransTracer, PostErrorCode =  postResp.nErrorCode, PostErrorText = postResp.sErrorText, TransReference = postResp.TransReference });
                            continue;
                        }
                    }
                    else
                    {
                        var deleteTransRef = await _CBSTransImplementation.ChectTransRefExist(cbsReversalLeg.ItbId, cbsReversalLeg.TransReference);

                        cbsReversalLeg.PostingErrorCode = postResp.nErrorCode;
                        cbsReversalLeg.PostingErrorDescr = postResp.sErrorText;
                        ApiResponse.ResponseCode = postResp.nErrorCode;
                        ApiResponse.ResponseMessage = postResp.sErrorText;
                        cbsReversalLeg.UserId = p.UserId;
                        var update = await _ApplicationDbContext.SaveChanges(p.LoginUserId);
                        sb.Append($"<li> Failed: Tracer Reference: { i.TransTracer }  Post Trans Message: {postResp.sErrorText}</li>");
                        failure++;
                          SubtringClass.Add(new SubtringClassDTO { TransTracer = i.TransTracer, PostErrorCode =  postResp.nErrorCode, PostErrorText = postResp.sErrorText, TransReference = postResp.TransReference });
                        continue;
                    }
                }
            }

            
                var getTransPostAttemp = new List<SubtringClassDTO>();

                int TotalPostedSucess = 0, TotalPostedFailed = 0;
               
                foreach(var res in SubtringClass)
                {
                    TotalPostedSucess = SubtringClass.Count(c=> c.PostErrorCode == 0 && c.TransTracer == res.TransTracer);
                    TotalPostedFailed = SubtringClass.Count(c=> c.PostErrorCode != 0 && c.TransTracer == res.TransTracer);
                    var ToTalTransTracerCount = SubtringClass.Count(c=> c.TransTracer  == res.TransTracer);

                     var getSubtringClass = getTransPostAttemp.FirstOrDefault(c => c.TransTracer == res.TransTracer);
                     string UnSuccesfulTrans = TotalPostedFailed > 0 ? $"{TotalPostedFailed} UnSuccessful transaction(s) Reason: {res.PostErrorText}" : string.Empty;

                    
                    
                     getTransPostAttemp.Add(new SubtringClassDTO { TransTracer = res.TransTracer, PostErrorCode =  postErrorCode, PostErrorText = postErrorText, TransReference = TransReference });
                }
                 
               

                TotalPostedSucess = SubtringClass.Count(c=> c.PostErrorCode == 0);
                TotalPostedFailed = SubtringClass.Count(c=> c.PostErrorCode != 0);

                var result = new {
                    SubtringClass = SubtringClass,
                    TotalPostedSucess = TotalPostedSucess,
                    TotalPostedFailed = TotalPostedFailed,
                    successMsg = "Successful transaction(s)",
                    errorMsg = "Failed transaction(s)"

                };
                        
            return Ok(result);

            }
            catch(Exception ex){
                var exM = ex == null ? ex.InnerException.Message : ex.Message;
                _LogManager.SaveLog($"Error Message: { exM } in Post Trans controler. StackTrace: {ex.StackTrace}");
                ApiResponse.ResponseMessage = ex == null ? ex.InnerException.Message : ex.Message;
                ApiResponse.ResponseCode = -99;
                return BadRequest(ApiResponse);
            }

            ApiResponse.ResponseMessage = "Reversal Not  Successful!";
            return BadRequest(ApiResponse);
        }


        [HttpPost("Update")]
        public async Task<IActionResult> Update(StatementReqDTO p)
        {
            try
            {
                int revToken = -1;
                int itbId = p.oprStatementReq.ItbId;

                var get = await _repoprStatementReq.GetAsync(c => c.ItbId == itbId);

                if (get != null)
                {
                    get.AcctNo = p.oprStatementReq.AcctNo;
                    //Do for others
                    _repoprStatementReq.Update(get);

                    revToken = await _ApplicationDbContext.SaveChanges(p.UserId);
                }


                int upServiceCharge = -1;

                foreach (var b in p.ListoprServiceCharge)
                {
                    var getSevice = await _repooprServiceCharge.GetAsync(c => c.ServiceId == p.oprStatementReq.ServiceId
                   && c.ServiceItbId == p.oprStatementReq.ItbId);

                    _repooprServiceCharge.Update(getSevice);
                    upServiceCharge = await _ApplicationDbContext.SaveChanges(p.UserId);

                }
                if (revToken > 0)
                {

                    if (revToken > 0)
                    {

                        ApiResponse.ResponseCode = 0;
                        ApiResponse.sErrorText = "Record Updated Successfully";

                        return Ok(ApiResponse);
                    }
                }
            }
            catch (Exception ex)
            {
                ApiResponse.ResponseMessage = ex == null ? ex.InnerException.Message : ex.Message;

                ApiResponse.ResponseCode = -99;
                return BadRequest(ApiResponse);
            }

            return BadRequest(ApiResponse);
        }

        [HttpPost("GetById")]
        public async Task<IActionResult> GetById(PostTransDTO p)
        {
            try
            {
                var get = await _repoCbsTransaction.GetAsync(c => c.ItbId == p.ItbId);
                if (get != null)
                {

                    var allUsers = await _UsersImplementation.GetAllUser(get.UserId, get.RejectedBy, 0);
                    var res = new
                    {
                        get = get,
                        allUsers = allUsers,
                    };

                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                var exM = ex;

            }

            return BadRequest();
        }

        [HttpPost("Retrieve")]
        public async Task<IActionResult> Retrieve(ParamLoadPage AnyAuth)
        {
            try{
                var RoleAssign = await _RoleAssignImplementation.GetRoleAssign(AnyAuth.MenuId, AnyAuth.RoleId);
                DynamicParameters param = new DynamicParameters();
                param.Add("@pnUserId", AnyAuth.UserId);
                param.Add("@pnDeptId", AnyAuth.pnDeptId);
                if (!string.IsNullOrEmpty(AnyAuth.psBranchNo))
                {
                    param.Add("@psbranchCode", AnyAuth.psBranchNo);
                }
                if (!string.IsNullOrEmpty(AnyAuth.psBranchNo))
                {
                    param.Add("@psBranches", AnyAuth.psBranchNo);
                }
                if (!string.IsNullOrEmpty(AnyAuth.FromDate))
                {
                    param.Add("@pdtStartDate", AnyAuth.FromDate);
                }
                if (!string.IsNullOrEmpty(AnyAuth.ToDate))
                {
                    param.Add("@pdtEndDate", AnyAuth.ToDate);
                }
                if (!string.IsNullOrEmpty(AnyAuth.Amount))
                {
                    param.Add("@pnAmnt", Convert.ToDecimal(AnyAuth.Amount.Replace(",", "")));
                }
                param.Add("@psIsGlobalSupervisor", RoleAssign.IsGlobalSupervisor);

                var rtn = new DapperDATAImplementation<CbsTransactionDTO>();
                var _response = await rtn.LoadData("Isp_ReversalPostingList", param, db);
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
    
    }
}