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
using RevAssuranceApi.Helper;

namespace RevAssuranceApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [EnableCors("CorsPolicy")]
    //[Authorize]
    public class PostTransController : ControllerBase
    {
        IConfiguration _configuration;
        ApiResponse ApiResponse = new ApiResponse();
        TokenGenerator TokenGenerator;
        AppSettingsPath AppSettingsPath;
        IDbConnection db = null;
        ApplicationDbContext _ApplicationDbContext;
        TransactionLogger _TransactionLogger;
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
        LogManager _LogManager;
        public PostTransController(IConfiguration configuration,
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
                                         TransactionLogger TransactionLogger, LogManager LogManager)
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
            _TransactionLogger = TransactionLogger;
            _LogManager = LogManager;
        }

        [HttpPost("GetAll")]
        public async Task<IActionResult> GetAll(ParamLoadPage AnyAuth)
        {
            try
            {
                var RoleAssign = await _RoleAssignImplementation.GetRoleAssign(AnyAuth.MenuId, AnyAuth.RoleId);
                bool IsGlobalView = false;
                if (RoleAssign != null)
                {
                    IsGlobalView = RoleAssign.IsGlobalSupervisor == true ? true : false;
                }

                DynamicParameters param = new DynamicParameters();

                param.Add("@pnUserId", AnyAuth.UserId);
                param.Add("@psBranchCode", AnyAuth.psBranchNo);
                param.Add("@pnDeptId", AnyAuth.pnDeptId);
                param.Add("@psGlobalView", IsGlobalView);

                var rtn = new DapperDATAImplementation<CbsTransactionDTO>();
                var _response = await rtn.LoadData("Isp_FetchPostingList", param, db);
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

        [HttpPost("PostTrans")]
        public async Task<IActionResult> PostTrans(PostTransDTO p)
        {
            ApiResponse rtv = new ApiResponse();
            try
            {
                _LogManager.SaveLog("Call post Transaction");
                GetIpForPostingTrans GetIpForPostingTrans = new GetIpForPostingTrans(_configuration, _LogManager);
                if(!GetIpForPostingTrans.GetIp()) 
                {

                    _LogManager.SaveLog("Invalid server ");
                    ApiResponse.ResponseCode =  99;
                    ApiResponse.ResponseMessage = "Invalid Server to Post Transaction, Kindly contact the System Administrator";
                    return BadRequest(ApiResponse);
                }

                _LogManager.SaveLog("Valid ips server");

                StringBuilder sb = new StringBuilder("<ul>");
                int success = 0;
                int failure = 0;
                int supervision = 0;

            
                var transactions = new List<CbsTransaction>();

                string PostedStatus = _configuration["Statuses:PostedStatus"];
                string SeekAuthorizeStatus = _configuration["Statuses:SeekAuthorizeStatus"];

                int postErrorCode = -999999;
                string postErrorText = string.Empty;
                string TransReference = string.Empty;

                string Ids = string.Empty, tranIds = string.Empty;

                foreach (var b in p.ListCbsTransactionDTO)
                {
                    _LogManager.SaveLog("add itbids ");
                    Ids += b.ItbId + ",";
                }

                tranIds = _Formatter.RemoveLast(Ids);
                _LogManager.SaveLog("remove last comma " + tranIds);

                var SubtringClass = new List<SubtringClassDTO>();

                _LogManager.SaveLog("call if tranIds ");
                if(!string.IsNullOrEmpty(tranIds))
                {
                    _LogManager.SaveLog("inside if tranIds is not empty ");
                    try
                    {
                        _LogManager.SaveLog("call data from CbsTransa where itbid in transIds");
                        var rtn = new DapperDATAImplementation<CbsTransaction>();
                        string script = $"select * from CbsTransaction where itbid in({tranIds})";
                        transactions = await rtn.LoadListNoParam(script, db);

                        _LogManager.SaveLog("transactions data " + transactions);
                        foreach (var eachTrans in transactions)
                        {
                            _LogManager.SaveLog("call eachtrans");
                            var i = await _repoCbsTransaction.GetAsync(c => c.ItbId == eachTrans.ItbId);
                            
                            _LogManager.SaveLog("call repoCbsTrans " + i);
                            var get = await _CBSTransImplementation.GetTransTrancer(i.TransTracer, p.UserId);

                            _LogManager.SaveLog("get data for GetTransTrancer call " + get);
                            int getCountTransTracer = get.Count;
                            _LogManager.SaveLog("getCountTransTracer counts " + getCountTransTracer);
                            int? NotChkFound = null;
                            string NotSelectString = string.Empty;
                            int numberSelected = 0;

                            foreach (var b in get)
                            {
                                if (tranIds.Contains(b.ItbId.ToString()))
                                {
                                    numberSelected += 1;
                                    _LogManager.SaveLog("call numberSelected " + numberSelected);
                                    continue;
                                }
                                else
                                {
                                    NotChkFound = 1;
                                    _LogManager.SaveLog("call NotChkFound " + NotChkFound);
                                    continue;
                                }
                            }
                            
                            var getUserProfile = await _UsersImplementation.ViewDetails(p.UserId);

                            _LogManager.SaveLog("call getUserProfile " + getUserProfile);
                            if (getUserProfile != null)
                            {
                                if (getUserProfile.UseCbsAuth == true)
                                {
                                    _LogManager.SaveLog("call getUserProfile.UseCbsAuth == true ");
                                    var CoreBnkLimit = await _CoreBankingImplementation.CoreBankingLimit(p.LoginUserName, i.CcyCode);
                                    
                                    _LogManager.SaveLog("call CoreBnkLimit " + CoreBnkLimit);
                                    var ValidateAmountLimit = _ApprovalValidation.ValidateAmountLimitCoreBnking(i, CoreBnkLimit);
                                    _LogManager.SaveLog("call ValidateAmountLimit ");

                                    if (ValidateAmountLimit.DebitLimit == true || ValidateAmountLimit.CreditLimit == true || ValidateAmountLimit.GLDebitLimit == true || ValidateAmountLimit.GLCreditLimit == true)                                         
                                    {
                                        _LogManager.SaveLog("call ValidateAmountLimit for all is true");
                                        var resApp = await _ApplicationReturnMessage.returnMessage(20005);

                                        sb.Append($"<li> Authorized: Tracer Reference: { i.TransTracer } {resApp.ErrorText} </li>");

                                        i.Status = SeekAuthorizeStatus;
                                        i.UserId = p.UserId;
                                        var update = await _CBSTransImplementation.CbsTransUpdate(i, p.UserId);

                                            //Update Primary Table here
                                        var updatePrimaryVal = await _CBSTransImplementation.UpdatePrimaryTBL((int)i.ServiceId, SeekAuthorizeStatus, i.PrimaryId, p.UserId);
                                        _LogManager.SaveLog("call updatePrimaryVal");
                                        var UpdateServiceChargeVal = await _CBSTransImplementation.UpdateServiceCharge(i.ServiceChargeId, SeekAuthorizeStatus, p.UserId, _Formatter.ValLong(i.PrimaryId.ToString()), p.LoginUserName);
                                        _LogManager.SaveLog("call UpdateServiceChargeVal");
                                        supervision++;
                                        continue;
                                    
                                    }
                                    else
                                    {
                                        _LogManager.SaveLog("call else ValidateAmountLimit not true ");
                                        var getUserLimit = await _ApprovalValidation.ValidateLimitWithCurrency(p.UserId, i.CcyCode);

                                        var ValidateAmountLimitVal = _ApprovalValidation.ValidateAmountLimit(i, getUserLimit);
                                        _LogManager.SaveLog("call  ValidateAmountLimitVal inside else " + ValidateAmountLimitVal);
                                        if (ValidateAmountLimitVal.DebitLimit == true || ValidateAmountLimitVal.CreditLimit == true || ValidateAmountLimitVal.GLDebitLimit == true || ValidateAmountLimitVal.GLCreditLimit == true)
                                        {
                                            rtv.ResponseCode = -1;
                                            rtv.ResponseMessage = "Limit Exceeded";
                                            _LogManager.SaveLog("set rtv.ResponseMessage = Limit exceeded");
                                            var resApp = await _ApplicationReturnMessage.returnMessage(20005);
                                            sb.Append($"<li> Authorized: Tracer Reference: { i.TransTracer } {resApp.ErrorText}</li>");

                                            i.Status = SeekAuthorizeStatus;
                                            i.UserId = p.UserId;
                                            var update = await _CBSTransImplementation.CbsTransUpdate(i, p.UserId);

                                            //Update Primary Table here

                                            var b = await _CBSTransImplementation.UpdatePrimaryTBL((int)i.ServiceId, SeekAuthorizeStatus, i.PrimaryId, p.UserId);
                                            var c = await _CBSTransImplementation.UpdateServiceCharge(i.ServiceChargeId, SeekAuthorizeStatus, p.UserId, _Formatter.ValLong(i.PrimaryId.ToString()), p.LoginUserName);


                                            supervision++;
                                            continue;
                                        }
                                    }
                                }
                                _LogManager.SaveLog("call getUserProfile.UseCbsAuth is not true");
                                try
                                {
                                    if (i.DrAcctType == "GL" && i.CrAcctType == "GL")
                                    {
                                        // Do not Check for Bal if DR and CR are  GL
                                        _LogManager.SaveLog("call Do not Check for Bal if DR and CR are  GL");
                                    }
                                    else
                                    {
                                        if (i.Amount > i.DrAcctCashBalance)
                                        {
                                            _LogManager.SaveLog("CALL insufficient balance " + i.Amount);
                                            if (i.Amount > i.DrAcctBalance)
                                            {
                                                _LogManager.SaveLog("CALL i.Amount > i.DrAcctBalance " + i.DrAcctBalance);
                                                sb.Append($"<li> Authorized: Tracer Reference: { i.TransTracer } transaction Could't be Processed. Insufficient Balance</li>");
                                                
                                                failure++;
                                                i.Status = SeekAuthorizeStatus;
                                                i.UserId = p.UserId;
                                                var update = await _CBSTransImplementation.CbsTransUpdate(i, p.UserId);

                                                var d = await _CBSTransImplementation.UpdatePrimaryTBL((int)i.ServiceId, SeekAuthorizeStatus, i.PrimaryId, p.UserId);
                                                var e = await _CBSTransImplementation.UpdateServiceCharge(i.ServiceChargeId, SeekAuthorizeStatus, p.UserId, _Formatter.ValLong(i.PrimaryId.ToString()), p.LoginUserName);

                                                continue;
                                            }
                                        }

                                        var CalAllChg = i.Amount + i.DrAcctChargeAmt + i.DrAcctTaxAmt;
                                        _LogManager.SaveLog("call CalAllChg" + CalAllChg);
                                        if (i.DrAcctBalance < CalAllChg) 
                                        {
                                            sb.Append($"<li>  TransTracer: { i.TransTracer } Insufficient Balance</li>");
                                            _LogManager.SaveLog("call TransTracer Insufficient Balance " + i.TransTracer);
                                            i.Status = SeekAuthorizeStatus;
                                            i.UserId = p.UserId;                                       
                                            SubtringClass.Add(new SubtringClassDTO { TransTracer = i.TransTracer, PostErrorCode = -999, PostErrorText = $"Insufficient Balance", TransReference = i.TransReference });

                                            failure++;
                                            continue;
                                        }
                                    
                                    }
                                    var checkTransExit = await _CBSTransImplementation.ChectTransRefExist(i.ItbId, i.TransReference);
                                    _LogManager.SaveLog("call checkTransExit again " + checkTransExit);

                                    if (checkTransExit.nReturnCode == -1)
                                    {
                                        sb.Append($"<li> Failed: Trans. Ref: {i.TransReference} Failed. Reason: { checkTransExit.sReturnMsg } </li>");

                                        failure++;
                                        rtv.ResponseCode = checkTransExit.nReturnCode;
                                        rtv.ResponseMessage = checkTransExit.sReturnMsg;
                                        SubtringClass.Add(new SubtringClassDTO { TransTracer = i.TransTracer, PostErrorCode = -1, PostErrorText = $"Trans. Ref. {i.TransReference } {checkTransExit.sReturnMsg}" , TransReference = i.TransReference});
                                        continue;
                                    }

                                    var postResp = await _CBSTransImplementation.PostTransactionsEthix(i, p.LoginUserName);
                                    _LogManager.SaveLog("call postResp " + postResp);
                                    if (postResp != null)
                                    {
                                        postErrorCode = postResp.nErrorCode;
                                        postErrorText = postResp.sErrorText;
                                        TransReference =  postResp.TransReference;
                                        _LogManager.SaveLog("call inside postResp != null " + postResp);
                                        if (postResp.nErrorCode == 0)
                                        {
                                            _LogManager.SaveLog("call postResp.nErrorCode == 0 ");
                                            i.PostingErrorCode = postResp.nErrorCode;
                                            i.PostingErrorDescr = postResp.sErrorText;
                                            i.UserId = p.UserId;
                                            i.CbsUserId = p.LoginUserName;
                                            i.DrAcctCashAmt = !string.IsNullOrEmpty(postResp.drCashAmt) ? decimal.Parse(postResp.drCashAmt) : 0.00m;
                                            i.CrAcctCbsTranId = _Formatter.ValDecimal(postResp.nCbsTranId);
                                            i.DrAcctBalAfterPost = !string.IsNullOrEmpty(postResp.nBalance) ? decimal.Parse(postResp.nBalance) : 0.00m;
                                            i.Status = PostedStatus;
                                            i.PostingDate = DateTime.Now;
                                            i.UserId = p.UserId;

                                            var update = await _CBSTransImplementation.CbsTransUpdate(i, p.UserId);
                                            success++;

                                            _LogManager.SaveLog("call update " + update);
                                            var returnUpdate = await _CBSTransImplementation.UpdateServiceCharge(i.ServiceChargeId, PostedStatus, p.UserId, _Formatter.ValLong(i.PrimaryId.ToString()), p.LoginUserName);

                                            _LogManager.SaveLog("call UpdateServiceCharge call after " + returnUpdate);

                                            var getSubtringClass = SubtringClass.FirstOrDefault(c => c.TransTracer == i.TransTracer);
                                            _LogManager.SaveLog("call UpdateServiceCharge call after getSubtringClass " + getSubtringClass);
                                            if (getSubtringClass == null)
                                            {
                                                _LogManager.SaveLog("call getSubtringClass == null");
                                                if (postResp.sErrorText != "Success")
                                                {
                                                    _LogManager.SaveLog("call postResp.sErrorText != success");
                                                    sb.Append($"<li> Error: { postResp.sErrorText } Trans Count: {getCountTransTracer}</li>");
                                                }
                                                else
                                                {
                                                    _LogManager.SaveLog("call postResp.sErrorText == success");
                                                    sb.Append($"<li> { success } Transaction(s) was Successful With transer No { i.TransTracer }. TransTracer Count: {getCountTransTracer}</li>");
                                                }

                                            }
                                            rtv.ResponseCode = postResp.nErrorCode;
                                            rtv.ResponseMessage = postResp.sErrorText;
                                            _LogManager.SaveLog("call postResp.sErrorText == " + postResp.sErrorText);
                                            SubtringClass.Add(new SubtringClassDTO { TransTracer = i.TransTracer, PostErrorCode =  postResp.nErrorCode, PostErrorText = postResp.sErrorText, TransReference = postResp.TransReference });

                                            continue;

                                        }
                                        else
                                        {

                                            var deleteTransRef = await _CBSTransImplementation.ChectTransRefExist(i.ItbId, i.TransReference);
                                            _LogManager.SaveLog("call deleteTransRef == " + deleteTransRef);
                                            i.PostingErrorCode = postResp.nErrorCode;
                                            i.PostingErrorDescr = postResp.sErrorText;
                                            rtv.ResponseCode = postResp.nErrorCode;
                                            rtv.ResponseMessage = postResp.sErrorText;
                                            i.UserId = p.UserId;
                                            var update = await _CBSTransImplementation.CbsTransUpdate(i, p.UserId);
                                            sb.Append($"<li> Failed: Tracer Reference: { i.TransTracer }  Post Trans Message: {postResp.sErrorText}</li>");
                                            failure++;

                                            rtv.ResponseCode = postResp.nErrorCode;
                                            rtv.ResponseMessage = postResp.sErrorText;
                                            SubtringClass.Add(new SubtringClassDTO { TransTracer = i.TransTracer, PostErrorCode =  postResp.nErrorCode, PostErrorText = postResp.sErrorText,TransReference = postResp.TransReference  });

                                            continue;
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    var exM = ex;
                                    failure++;
                                    var exMVa = ex.Message == null ? ex.InnerException.Message : ex.Message.ToString();
                                    _TransactionLogger.SaveTransLog($"Error While Posting Error Message:  {exMVa} ", i.TransReference);

                                    rtv.ResponseCode = postErrorCode;
                                    rtv.ResponseMessage = postErrorText;
                                    SubtringClass.Add(new SubtringClassDTO { TransTracer = i.TransTracer, PostErrorCode =  postErrorCode, PostErrorText = postErrorText, TransReference = TransReference });
                                    continue;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        var exM = ex == null ? ex.InnerException.Message : ex.Message;
                    }

                    StringBuilder sb2 = new StringBuilder("<ul>");

                    var getTransPostAttemp = new List<SubtringClassDTO>();

                    int TotalPostedSucess = 0, TotalPostedFailed = 0;
                
                    foreach(var res in SubtringClass)
                    {
                        _LogManager.SaveLog("call res in SubtringClass" + res);

                        TotalPostedSucess = SubtringClass.Count(c=> c.PostErrorCode == 0 && c.TransTracer == res.TransTracer);
                        TotalPostedFailed = SubtringClass.Count(c=> c.PostErrorCode != 0 && c.TransTracer == res.TransTracer);
                        var ToTalTransTracerCount = SubtringClass.Count(c=> c.TransTracer  == res.TransTracer);

                         _LogManager.SaveLog("call ToTalTransTracerCount " + ToTalTransTracerCount);
                        var getSubtringClass = getTransPostAttemp.FirstOrDefault(c => c.TransTracer == res.TransTracer);
                        string UnSuccesfulTrans = TotalPostedFailed > 0 ? $"{TotalPostedFailed} UnSuccessful transaction(s) Reason: {res.PostErrorText}" : string.Empty;

                        if(getSubtringClass == null)
                        {
                           _LogManager.SaveLog("call getSubtringClass == null");
                           sb2.Append($"<li> TransTracer: { res.TransTracer }- { TotalPostedSucess } Successful transaction(s). {UnSuccesfulTrans} TransTracer Count: {ToTalTransTracerCount}</li>");
                           getTransPostAttemp.Add(new SubtringClassDTO { TransTracer = res.TransTracer, PostErrorCode =  postErrorCode, PostErrorText = postErrorText, TransReference = TransReference });     
                        }
                        
                    }
                    
                    sb2.Append("</ul>");

                    rtv.StringbuilderMessage = sb2.ToString();
                    rtv.ResponseMessage = sb2.ToString();

                    TotalPostedSucess = SubtringClass.Count(c=> c.PostErrorCode == 0);
                    TotalPostedFailed = SubtringClass.Count(c=> c.PostErrorCode != 0);

                    _LogManager.SaveLog("call last stage to return result ");
                    var result = new {
                        SubtringClass = SubtringClass,
                        TotalPostedSucess = TotalPostedSucess,
                        TotalPostedFailed = TotalPostedFailed,
                        successMsg = "Successful transaction(s)",
                        errorMsg = "Failed transaction(s)"
                    };
                            
                    return Ok(result);

                }
                
                rtv.ResponseCode = -2;
                rtv.ResponseMessage = "You didn't Select Transaction(s)";
                sb.Append($"<li>{rtv.ResponseMessage}</li>");
                sb.Append("</ul>");
                
                rtv.StringbuilderMessage = sb.ToString();
                rtv.ResponseMessage = sb.ToString();
                return BadRequest(rtv);

            }
            catch(Exception ex)
            {
                 var exM = ex == null ? ex.InnerException.Message : ex.Message;
                _LogManager.SaveLog($"Error Message: { exM } in Post Trans controler. StackTrace: {ex.StackTrace}");
                ApiResponse.ResponseMessage = ex == null ? ex.InnerException.Message : ex.Message;
                ApiResponse.ResponseCode = -99;
                return BadRequest(ApiResponse);
            }


            //return BadRequest(rtv);
        }

        [HttpPost("SeekApproval")]
        public async Task<IActionResult> SeekApproval(PostTransDTO p)
        {
            StringBuilder sb = new StringBuilder("<ul>");
            sb.Append("<ul>");

            var SubtringClass = new List<SubtringClassDTO>();

            ApiResponse apiResponse = new ApiResponse();
            string SeekAuthorizeStatus = _configuration["Statuses:SeekAuthorizeStatus"];

            string Ids = string.Empty, tranIds = string.Empty;
            int tracerCountSucc = 0, Update = 0;
            foreach (var b in p.ListCbsTransactionDTO)
            {
                var get = await _repoCbsTransaction.GetAsync(c => c.ItbId == b.ItbId);
                if (get != null)
                {
                    get.Status = SeekAuthorizeStatus;

                    _repoCbsTransaction.Update(get);
                    Update = await _ApplicationDbContext.SaveChanges(p.LoginUserId);
                    if (Update > 0)
                    {
                        tracerCountSucc += 1;
                        SubtringClass.Add(new SubtringClassDTO { TransTracer = b.TransTracer });
                    }
                    else
                    {
                    }


                }
            }

            if (Update > 0 && p.ListCbsTransactionDTO.Count() == tracerCountSucc)
            {
                var SubtringClass1 = new List<SubtringClassDTO>();
                foreach (var b in SubtringClass)
                {

                    var getDefault = SubtringClass1.FirstOrDefault(c => c.TransTracer == b.TransTracer);
                    if (getDefault == null)
                    {
                        var getAllDefault = SubtringClass.Where(c => c.TransTracer == b.TransTracer);

                        sb.Append($"<li> Success: Trans Tracer: { b.TransTracer } was Successful! Trans Count: { getAllDefault.Count() } </li>");

                        SubtringClass1.Add(new SubtringClassDTO { TransTracer = b.TransTracer });
                    }
                }


                sb.Append("</ul>");
                apiResponse.ResponseMessage = sb.ToString();

                return Ok(apiResponse);
            }

            return BadRequest(apiResponse);
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



    }
}