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
                                         TransactionLogger TransactionLogger)
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
            StringBuilder sb = new StringBuilder("<ul>");
            int success = 0;
            int failure = 0;
            int supervision = 0;

            ApiResponse rtv = new ApiResponse();
            var transactions = new List<CbsTransaction>();

            string PostedStatus = _configuration["Statuses:PostedStatus"];
            string SeekAuthorizeStatus = _configuration["Statuses:SeekAuthorizeStatus"];

            int postErrorCode = -999999;
            string postErrorText = string.Empty;

            string Ids = string.Empty, tranIds = string.Empty;

            foreach (var b in p.ListCbsTransactionDTO)
            {
                Ids += b.ItbId + ",";
            }

            tranIds = _Formatter.RemoveLast(Ids);


            var SubtringClass = new List<SubtringClassDTO>();

            if (!string.IsNullOrEmpty(tranIds))
            {
                try
                {
                    var rtn = new DapperDATAImplementation<CbsTransaction>();
                    string script = $"select * from CbsTransaction where itbid in({tranIds})";
                     transactions = await rtn.LoadListNoParam(script, db);

                    foreach (var eachTrans in transactions)
                    {
                        var i = await _repoCbsTransaction.GetAsync(c => c.ItbId == eachTrans.ItbId);

                        var get = await _CBSTransImplementation.GetTransTrancer(i.TransTracer, p.UserId);

                        int getCountTransTracer = get.Count;
                        int? NotChkFound = null;
                        string NotSelectString = string.Empty;
                        int numberSelected = 0;
                        foreach (var b in get)
                        {
                            if (tranIds.Contains(b.ItbId.ToString()))
                            {
                                numberSelected += 1;
                                continue;
                            }
                            else
                            {
                                NotChkFound = 1;
                                continue;
                            }
                        }
                        //No need to check below, because a leg could be posted
                       // if (!string.IsNullOrWhiteSpace(i.DrAcctNo)) 
                        {
                            var getUserProfile = await _UsersImplementation.ViewDetails(p.UserId);
                            if (getUserProfile != null)
                            {
                                if (getUserProfile.UseCbsAuth == true)
                                {
                                    var CoreBnkLimit = await _CoreBankingImplementation.CoreBankingLimit(p.LoginUserName, i.CcyCode);

                                    var ValidateAmountLimit = _ApprovalValidation.ValidateAmountLimitCoreBnking(i, CoreBnkLimit);

                                    if (ValidateAmountLimit.DebitLimit == true || ValidateAmountLimit.CreditLimit == true
                                                                               || ValidateAmountLimit.GLDebitLimit == true
                                                                               || ValidateAmountLimit.GLCreditLimit == true)
                                    {
                                        //rtv.nErrorCode = -1;
                                        //rtv.sErrorText = "Limit Exceeded";
                                        var resApp = await _ApplicationReturnMessage.returnMessage(20005);

                                        sb.Append($"<li> Authorized: Tracer Reference: { i.TransTracer } {resApp.ErrorText} </li>");

                                        i.Status = SeekAuthorizeStatus;
                                        i.UserId = p.UserId;
                                        var update = await _CBSTransImplementation.CbsTransUpdate(i, p.UserId);

                                        //Update Primary Table here
                                        await _CBSTransImplementation.UpdatePrimaryTBL((int)i.ServiceId, SeekAuthorizeStatus, i.PrimaryId, p.UserId);

                                        await _CBSTransImplementation.UpdateServiceCharge(i.ServiceChargeId, SeekAuthorizeStatus, p.UserId, _Formatter.ValLong(i.PrimaryId.ToString()), p.LoginUserName);

                                        supervision++;
                                        continue;
                                    }
                                }
                                else
                                {
                                    var getUserLimit = await _ApprovalValidation.ValidateLimitWithCurrency(p.UserId, i.CcyCode);

                                    var ValidateAmountLimit = _ApprovalValidation.ValidateAmountLimit(i, getUserLimit);

                                    if (ValidateAmountLimit.DebitLimit == true || ValidateAmountLimit.CreditLimit == true || ValidateAmountLimit.GLDebitLimit == true || ValidateAmountLimit.GLCreditLimit == true)
                                    {
                                        rtv.ResponseCode = -1;
                                        rtv.ResponseMessage = "Limit Exceeded";

                                        var resApp = await _ApplicationReturnMessage.returnMessage(20005);
                                        sb.Append($"<li> Authorized: Tracer Reference: { i.TransTracer } {resApp.ErrorText}</li>");

                                        i.Status = SeekAuthorizeStatus;
                                        i.UserId = p.UserId;
                                        var update = await _CBSTransImplementation.CbsTransUpdate(i, p.UserId);

                                        //Update Primary Table here

                                        await _CBSTransImplementation.UpdatePrimaryTBL((int)i.ServiceId, SeekAuthorizeStatus, i.PrimaryId, p.UserId);
                                        await _CBSTransImplementation.UpdateServiceCharge(i.ServiceChargeId, SeekAuthorizeStatus, p.UserId, _Formatter.ValLong(i.PrimaryId.ToString()), p.LoginUserName);


                                        supervision++;
                                        continue;
                                    }
                                }
                            }
                            try
                            {
                                if (i.DrAcctType == "GL" && i.CrAcctType == "GL")
                                {
                                    // Do not Check for Bal if DR and CR are  GL
                                }
                                else
                                {
                                    if (i.Amount > i.DrAcctCashBalance)
                                    {
                                        if (i.Amount > i.DrAcctBalance)
                                        {
                                            sb.Append($"<li> Authorized: Tracer Reference: { i.TransTracer } transaction Could't be Processed. Insufficient Balance</li>");

                                            failure++;
                                            i.Status = SeekAuthorizeStatus;
                                            i.UserId = p.UserId;
                                            var update = await _CBSTransImplementation.CbsTransUpdate(i, p.UserId);

                                            await _CBSTransImplementation.UpdatePrimaryTBL((int)i.ServiceId, SeekAuthorizeStatus, i.PrimaryId, p.UserId);
                                            await _CBSTransImplementation.UpdateServiceCharge(i.ServiceChargeId, SeekAuthorizeStatus, p.UserId, _Formatter.ValLong(i.PrimaryId.ToString()), p.LoginUserName);

                                            continue;
                                        }
                                    }

                                    var CalAllChg = i.Amount + i.DrAcctChargeAmt + i.DrAcctTaxAmt;

                                     if (i.DrAcctBalance < CalAllChg) 
                                    {
                                        sb.Append($"<li>  TransTracer: { i.TransTracer } Insufficient Balance</li>");

                                        i.Status = SeekAuthorizeStatus;
                                        i.UserId = p.UserId;                                       
                                        SubtringClass.Add(new SubtringClassDTO { TransTracer = i.TransTracer, PostErrorCode = -999, PostErrorText = $"Insufficient Balance" });

                                        failure++;
                                        continue;
                                    }
                                
                                }
                                var checkTransExit = await _CBSTransImplementation.ChectTransRefExist(i.ItbId, i.TransReference);
                                if (checkTransExit.nReturnCode == -1)
                                {
                                    sb.Append($"<li> Failed: Trans. Ref: {i.TransReference} Failed. Reason: { checkTransExit.sReturnMsg } </li>");

                                    failure++;
                                    rtv.ResponseCode = checkTransExit.nReturnCode;
                                    rtv.ResponseMessage = checkTransExit.sReturnMsg;
                                    continue;
                                }


                                AccountValParam DrAccountValParam = new AccountValParam();

                                DrAccountValParam.AcctType = i.DrAcctType;
                                DrAccountValParam.AcctNo = i.DrAcctNo;
                                DrAccountValParam.CrncyCode = i.CcyCode;
                                DrAccountValParam.Username = p.LoginUserName;

                                var ValDrAcct = await _AccountValidationImplementation.ValidateAccountCall(DrAccountValParam);

                                i.DrAcctAddress = ValDrAcct.sAddress;
                                i.DrAcctClassCode = ValDrAcct.ProductCode;
                                //i.DrAcctChequeNo				= ValDrAcct. ;
                                //i.DrAcctChargeCode			= ValDrAcct. ;
                                //i.DrAcctChargeAmt				= ValDrAcct ;
                                //i.DrAcctTaxAmt				= ValDrAcct. ;
                                //i.DrAcctChargeRate			= ValDrAcct. ;
                                //i.DrAcctChargeNarr			= ValDrAcct. ;
                                //i.DrAcctBalAfterPost			= ValDrAcct. ;
                                //i.CrAcctBalAfterPost			= ValDrAcct. ;
                                i.DrAcctOpeningDate = _Formatter.ValidateDate(ValDrAcct.sAcctOpenDate);
                                i.DrAcctIndusSector = ValDrAcct.sSector;
                                //i.DrAcctCbsTranId				= ValDrAcct. ;
                                i.DrAcctCustNo = _Formatter.ValIntergers(ValDrAcct.CustId);
                                i.DrAcctCashBalance = _Formatter.ValDecimal(ValDrAcct.nCashBalance);
                                //i.DrAcctCashAmt				= ValDrAcct. ;
                                i.DrAcctCity = ValDrAcct.sCity;
                                i.DrAcctValUserId = p.UserId;
                                i.DrAcctValErrorCode = ValDrAcct.nErrorCode;
                                i.DrAcctValErrorMsg = ValDrAcct.sErrorText;


                                AccountValParam CrAccountValParam = new AccountValParam();

                                CrAccountValParam.AcctType = i.CrAcctType;
                                CrAccountValParam.AcctNo = i.CrAcctNo;
                                CrAccountValParam.CrncyCode = i.CcyCode;
                                CrAccountValParam.Username = p.LoginUserName;

                                var ValCrAcct = await _AccountValidationImplementation.ValidateAccountCall(CrAccountValParam);

                                i.CrAcctAddress = ValCrAcct.sAddress;
                                //i.Cr				= ValCrAcct.ProductCode;     
                                //i.CrAcctChequeNo				= ValCrAcct. ;
                                //i.CrAcctChargeCode			= ValCrAcct.  ;
                                //i.CrAcctChargeAmt				= ValCrAcct. ;
                                //i.CrAcctTaxAmt				= ValCrAcct.  ;
                                //i.CrAcctChargeRate			= ValCrAcct.  ;
                                //i.CrAcctChargeNarr			= ValCrAcct.  ;
                                //i.CrAcctBalAfterPost			= ValCrAcct.  ;
                                //i.CrAcctBalAfterPost			= ValCrAcct.  ;
                                i.CrAcctOpeningDate = _Formatter.ValidateDate(ValCrAcct.sAcctOpenDate);
                                i.CrAcctIndusSector = ValCrAcct.sSector;
                                //i.CrAcctCbsTranId				= ValCrAcct. ;
                                i.CrAcctCustNo = _Formatter.ValIntergers(ValCrAcct.CustId);
                                i.CrAcctCashBalance = _Formatter.ValDecimal(ValCrAcct.nCashBalance);
                                //i.DrAcctCashAmt				= ValCrAcct. ;
                                i.CrAcctCity = ValCrAcct.sCity;
                                i.CrAcctValUserId = p.UserId;
                                i.CrAcctValErrorCode = ValCrAcct.nErrorCode;
                                i.CrAcctValErrorMsg = ValCrAcct.sErrorText;

                                var updateVal = await _CBSTransImplementation.CbsTransUpdate(i, p.UserId);

                                var postResp = await _CBSTransImplementation.PostTransactionsEthix(i, p.LoginUserName);
                                if (postResp != null)
                                {
                                    postErrorCode = postResp.nErrorCode;
                                    postErrorText = postResp.sErrorText;

                                    if (postResp.nErrorCode == 0)
                                    {
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

                                        await _CBSTransImplementation.UpdateServiceCharge(i.ServiceChargeId, PostedStatus, p.UserId, _Formatter.ValLong(i.PrimaryId.ToString()), p.LoginUserName);

                                        var getSubtringClass = SubtringClass.FirstOrDefault(c => c.TransTracer == i.TransTracer);
                                        if (getSubtringClass == null)
                                        {
                                            if (postResp.sErrorText != "Success")
                                            {
                                                sb.Append($"<li> Error: { postResp.sErrorText } Trans Count: {getCountTransTracer}</li>");
                                            }
                                            else
                                            {
                                                sb.Append($"<li> { success } Transaction(s) was Successful With transer No { i.TransTracer }. TransTracer Count: {getCountTransTracer}</li>");
                                            }

                                        }
                                        rtv.ResponseCode = postResp.nErrorCode;
                                        rtv.ResponseMessage = postResp.sErrorText;
                                         SubtringClass.Add(new SubtringClassDTO { TransTracer = i.TransTracer, PostErrorCode =  postResp.nErrorCode, PostErrorText = postResp.sErrorText });

                                        continue;

                                    }
                                    else
                                    {

                                        var deleteTransRef = await _CBSTransImplementation.ChectTransRefExist(i.ItbId, i.TransReference);

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
                                        SubtringClass.Add(new SubtringClassDTO { TransTracer = i.TransTracer, PostErrorCode =  postResp.nErrorCode, PostErrorText = postResp.sErrorText });

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
                                SubtringClass.Add(new SubtringClassDTO { TransTracer = i.TransTracer, PostErrorCode =  postErrorCode, PostErrorText = postErrorText});
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
               

                foreach(var res in SubtringClass)
                {
                    var getToTalPostedSuccess = SubtringClass.Count(c=> c.PostErrorCode == 0 && c.TransTracer == res.TransTracer);
                    var getToTalPostedError = SubtringClass.Count(c=> c.PostErrorCode != 0 && c.TransTracer == res.TransTracer);
                    var ToTalTransTracerCount = SubtringClass.Count(c=> c.TransTracer  == res.TransTracer);

                     var getSubtringClass = getTransPostAttemp.FirstOrDefault(c => c.TransTracer == res.TransTracer);
                     string UnSuccesfulTrans = getToTalPostedError > 0 ? $"{getToTalPostedError} UnSuccessful transaction(s) Reason: {res.PostErrorText}" : string.Empty;

                     if(getSubtringClass == null)
                     sb2.Append($"<li> TransTracer: { res.TransTracer }- { getToTalPostedSuccess } Successful transaction(s). {UnSuccesfulTrans} TransTracer Count: {ToTalTransTracerCount}</li>");
                    
                     getTransPostAttemp.Add(new SubtringClassDTO { TransTracer = res.TransTracer, PostErrorCode =  postErrorCode, PostErrorText = postErrorText});
                }
                sb2.Append("</ul>");

                rtv.StringbuilderMessage = sb2.ToString();
                rtv.ResponseMessage = sb2.ToString();

                return Ok(rtv);

            }
            else
            {
                rtv.ResponseCode = -2;
                rtv.ResponseMessage = "You didn't Select Transaction(s)";
                sb.Append($"<li>{rtv.ResponseMessage}</li>");
                sb.Append("</ul>");
                return BadRequest(rtv);
            }

            sb.Append("</ul>");
            rtv.StringbuilderMessage = sb.ToString();
            rtv.ResponseMessage = sb.ToString();

            return BadRequest(rtv);
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