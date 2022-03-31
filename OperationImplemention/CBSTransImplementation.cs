using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using RevAssuranceApi.RevenueAssurance.Repository.Interface;
using RevAssuranceWebAPi.AnythingGood.DATA.Models;
using RevAssuranceApi.Response;
using RevAssuranceApi.RevenueAssurance.DATA.Models;
using System;
using RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO;
using Dapper;
using RevAssuranceApi.RevenueAssurance.Repository.DapperDAL;
using Microsoft.Extensions.Configuration;
using System.Data;
using RevAssuranceApi.AppSettings;
using System.Data.SqlClient;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using RevAssuranceApi.Helper;

namespace RevAssuranceApi.OperationImplemention
{
    public class CBSTransImplementation
    {
        IRepository<CbsTransaction> _repoCbsTransaction;
        ApplicationDbContext _ApplicationDbContext;
        IRepository<OprInstrument> _repoOprInstrument;
        IRepository<OprOverDraft> _repoOprOverDraft;
        IRepository<admClientProfile> _repoadmClientProfile;
        IRepository<admBankServiceSetup> _repoadmBankServiceSetup;
        IRepository<oprServiceCharges> _repooprServiceCharges;
        IConfiguration _configuration;
        IDbConnection db = null;
        AppSettingsPath AppSettingsPath;
        Formatter _Formatter = new Formatter();
        LogManager _LogManager;
        TransactionLogger _TransactionLogger;
        FunctionApiSetUpImplementation _FunctionApiSetUpImplementation;

        IRepository<OprToken> _repoOprToken;
        IRepository<OprChqBookRequest> _repoOprChqBookRequest;
        IRepository<oprCounterChq> _repooprCounterChq;
        IRepository<OprStopChqRequest> _repoOprStopChqRequest;
        IRepository<OprBusinessSearch> _repoOprBusinessSearch;
        IRepository<OprAcctClosure> _repoOprAcctClosure;
        IRepository<OprCard> _repoOprCard;
        IRepository<oprStatementReq> _repooprStatementReq;
        IRepository<OprTradeReference> _repoOprTradeReference;
        IRepository<OprReferenceLetter> _repoOprReferenceLetter;



        public CBSTransImplementation(IRepository<CbsTransaction> repoCbsTransaction,
                                      ApplicationDbContext ApplicationDbContext,
                                      IRepository<OprInstrument> repoOprInstrument,
                                       IConfiguration configuration,
                                       IRepository<admClientProfile> repoadmClientProfile,
                                       IRepository<admBankServiceSetup> repoadmBankServiceSetup,
                                        FunctionApiSetUpImplementation FunctionApiSetUpImplementation,
                                        LogManager LogManager, TransactionLogger TransactionLogger,
                                        IRepository<OprOverDraft> repoOprOverDraft,
                                        IRepository<oprServiceCharges> repooprServiceCharges,
                                        IRepository<OprToken> repoOprToken,
                                        IRepository<OprChqBookRequest> repoOprChqBookRequest,
                                        IRepository<oprCounterChq> repooprCounterChq,
                                        IRepository<OprStopChqRequest> repoOprStopChqRequest,
                                        IRepository<OprBusinessSearch> repoOprBusinessSearch,
                                        IRepository<OprAcctClosure> repoOprAcctClosure,
                                        IRepository<OprCard> repoOprCard,
                                        IRepository<oprStatementReq> repooprStatementReq,
                                        IRepository<OprTradeReference> repoOprTradeReference,
                                        IRepository<OprReferenceLetter> repoOprReferenceLetter)
        {
            _configuration = configuration;
            _repoCbsTransaction = repoCbsTransaction;
            _ApplicationDbContext = ApplicationDbContext;
            _repoOprInstrument = repoOprInstrument;
            AppSettingsPath = new AppSettingsPath(_configuration);
            db = new SqlConnection(AppSettingsPath.GetDefaultCon());
            _repoadmClientProfile = repoadmClientProfile;
            _repoadmBankServiceSetup = repoadmBankServiceSetup;
            _FunctionApiSetUpImplementation = FunctionApiSetUpImplementation;
            _LogManager = LogManager;
            _TransactionLogger = TransactionLogger;
            _repoOprOverDraft = repoOprOverDraft;
            _repooprServiceCharges = repooprServiceCharges;
            _repoOprToken = repoOprToken;
            _repoOprChqBookRequest = repoOprChqBookRequest;
            _repooprCounterChq = repooprCounterChq;
            _repoOprStopChqRequest = repoOprStopChqRequest;
            _repoOprBusinessSearch = repoOprBusinessSearch;
            _repoOprAcctClosure = repoOprAcctClosure;
            _repoOprCard = repoOprCard;
            _repooprStatementReq = repooprStatementReq;
            _repoOprTradeReference = repoOprTradeReference;
            _repoOprReferenceLetter = repoOprReferenceLetter;
        }
        public async Task<List<CbsTransaction>> GetTransTrancer(string TransTracer, int userId)
        {
            var get = await _repoCbsTransaction.GetManyListAsync(c => c.TransTracer == TransTracer && c.Status == "Not Validated");

            return get.ToList();
        }
        public async Task<ApiResponse> CbsTransUpdate(CbsTransaction cbs, int userId)
        {
            _LogManager.SaveLog("call CbsTranUpdate ");
            var rtv = new ApiResponse();
            try
            {
                _repoCbsTransaction.Update(cbs);
                var retV = await _ApplicationDbContext.SaveChanges(userId);
                if (retV > 0)
                {
                    _LogManager.SaveLog("call retV > 0 == success update " + retV);
                    rtv.CbsItbid = cbs.ItbId;
                    rtv.ResponseCode = 0;
                    rtv.ResponseMessage = "Success";
                    return rtv;
                }
                else
                {
                    _LogManager.SaveLog("call failed while updating ");
                    rtv.ResponseCode = -1;
                    rtv.ResponseMessage = "Failed While Updating";
                    return rtv;
                }
            }
            catch (Exception ex)
            {
                var exM = ex;

                // LogManager.SaveLog(ex.Message == null ? ex.InnerException.ToString() : ex.Message.ToString());
                // rtv.nErrorCode = -1;
                // rtv.sErrorText = "Failed While inserting";
                // return rtv;
            }
            return rtv;
        }

        public async Task<ApiResponse> UpdatePrimaryTBL(int ServiceId, string status, decimal? PrimaryId, int UserId, string UserName = null)
        {
            ApiResponse ApiResponse = new ApiResponse();
            try
            {
                _LogManager.SaveLog("call UpdatePrimaryTBL serviceId parameter = " + ServiceId);
                if (ServiceId == 1)
                {
                    var getOprIns = await _repoOprToken.GetAsync(c => c.ItbId == PrimaryId);
                    getOprIns.Status = status;
                    _repoOprToken.Update(getOprIns);
                    int Save = await _ApplicationDbContext.SaveChanges(UserId);
                    if (Save > 0)
                    {
                        ApiResponse.ResponseMessage = "Success";
                        ApiResponse.ResponseCode = 0;
                    }
                }

                if (ServiceId == 2)
                {
                    var getOprIns = await _repoOprChqBookRequest.GetAsync(c => c.ItbId == PrimaryId);
                    getOprIns.Status = status;
                    _repoOprChqBookRequest.Update(getOprIns);
                    int Save = await _ApplicationDbContext.SaveChanges(UserId);
                    if (Save > 0)
                    {
                        ApiResponse.ResponseMessage = "Success";
                        ApiResponse.ResponseCode = 0;
                    }
                }

                if (ServiceId == 3)
                {
                    var getOprIns = await _repooprCounterChq.GetAsync(c => c.ItbId == PrimaryId);
                    getOprIns.Status = status;
                    _repooprCounterChq.Update(getOprIns);
                    int Save = await _ApplicationDbContext.SaveChanges(UserId);
                    if (Save > 0)
                    {
                        ApiResponse.ResponseMessage = "Success";
                        ApiResponse.ResponseCode = 0;
                    }
                }
                if (ServiceId == 4)
                {
                    var getOprIns = await _repoOprStopChqRequest.GetAsync(c => c.ItbId == PrimaryId);
                    getOprIns.Status = status;
                    _repoOprStopChqRequest.Update(getOprIns);
                    int Save = await _ApplicationDbContext.SaveChanges(UserId);
                    if (Save > 0)
                    {
                        ApiResponse.ResponseMessage = "Success";
                        ApiResponse.ResponseCode = 0;
                    }
                }
                if (ServiceId == 5)
                {
                    var getOprIns = await _repoOprBusinessSearch.GetAsync(c => c.ItbId == PrimaryId);
                    getOprIns.Status = status;
                    _repoOprBusinessSearch.Update(getOprIns);
                    int Save = await _ApplicationDbContext.SaveChanges(UserId);
                    if (Save > 0)
                    {
                        ApiResponse.ResponseMessage = "Success";
                        ApiResponse.ResponseCode = 0;
                    }
                }
                if (ServiceId == 6)
                {
                    var getOprIns = await _repoOprAcctClosure.GetAsync(c => c.ItbId == PrimaryId);
                    getOprIns.Status = status;
                    _repoOprAcctClosure.Update(getOprIns);
                    int Save = await _ApplicationDbContext.SaveChanges(UserId);
                    if (Save > 0)
                    {
                        ApiResponse.ResponseMessage = "Success";
                        ApiResponse.ResponseCode = 0;
                    }
                }
                if (ServiceId == 7)
                {
                    var getOprIns = await _repoOprCard.GetAsync(c => c.ItbId == PrimaryId);
                    getOprIns.Status = status;
                    _repoOprCard.Update(getOprIns);
                    int Save = await _ApplicationDbContext.SaveChanges(UserId);
                    if (Save > 0)
                    {
                        ApiResponse.ResponseMessage = "Success";
                        ApiResponse.ResponseCode = 0;
                    }
                }
                if (ServiceId == 8)
                {
                    var getOprIns = await _repooprStatementReq.GetAsync(c => c.ItbId == PrimaryId);
                    getOprIns.Status = status;
                    _repooprStatementReq.Update(getOprIns);
                    int Save = await _ApplicationDbContext.SaveChanges(UserId);
                    if (Save > 0)
                    {
                        ApiResponse.ResponseMessage = "Success";
                        ApiResponse.ResponseCode = 0;
                    }
                }
                if (ServiceId == 9)
                {
                    var getOprIns = await _repoOprTradeReference.GetAsync(c => c.ItbId == PrimaryId);
                    getOprIns.Status = status;
                    _repoOprTradeReference.Update(getOprIns);
                    int Save = await _ApplicationDbContext.SaveChanges(UserId);
                    if (Save > 0)
                    {
                        ApiResponse.ResponseMessage = "Success";
                        ApiResponse.ResponseCode = 0;
                    }
                }

                if (ServiceId == 10)
                {
                    var getOprIns = await _repoOprReferenceLetter.GetAsync(c => c.ItbId == PrimaryId);
                    getOprIns.Status = status;
                    _repoOprReferenceLetter.Update(getOprIns);
                    int Save = await _ApplicationDbContext.SaveChanges(UserId);
                    if (Save > 0)
                    {
                        ApiResponse.ResponseMessage = "Success";
                        ApiResponse.ResponseCode = 0;
                    }
                }

                if (ServiceId == 11 || ServiceId == 14 || ServiceId == 18 || ServiceId == 19)
                {
                    _LogManager.SaveLog("call if ServiceId == 11, 14, 18, 19 ");
                    var getOprIns = await _repoOprInstrument.GetAsync(c => c.ItbId == PrimaryId);
                    getOprIns.Status = status;
                    _repoOprInstrument.Update(getOprIns);
                    int Save = await _ApplicationDbContext.SaveChanges(UserId);
                    if (Save > 0)
                    {
                        ApiResponse.ResponseMessage = "Success";
                        ApiResponse.ResponseCode = 0;
                    }
                }
                if (ServiceId == 13)
                {
                    var getOprIns = await _repoOprOverDraft.GetAsync(c => c.ItbId == PrimaryId);
                    getOprIns.Status = status;
                    _repoOprOverDraft.Update(getOprIns);
                    int Save = await _ApplicationDbContext.SaveChanges(UserId);
                    if (Save > 0)
                    {
                        ApiResponse.ResponseMessage = "Success";
                        ApiResponse.ResponseCode = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                var exM = ex.Message == null ? ex.InnerException.ToString() : ex.Message.ToString();
                _LogManager.SaveLog($"Internal Error While UpdatePrimaryTBL in CBSTransImplementation Error: {exM}");
            }

            return ApiResponse;
        }

        public async Task<ApiResponse> UpdateServiceCharge(Int64? ItbId, string status, int UserId, long ServiceItbId, string UserName = "system")
        {
            ApiResponse ApiResponse = new ApiResponse();
            try
            {
                _LogManager.SaveLog("call UpdateServiceCharge ");
                var getOprIns = await _repooprServiceCharges.GetAsync(c => c.ItbId == ItbId);
                getOprIns.Status = status;
                _repooprServiceCharges.Update(getOprIns);
                int Save = await _ApplicationDbContext.SaveChanges(UserId);
                if (Save > 0)
                {
                    _LogManager.SaveLog("call save > 0 == success ");
                    ApiResponse.ResponseMessage = "Success";
                    ApiResponse.ResponseCode = 0;
                }

                if (getOprIns.ServiceId == 13)
                {
                    _LogManager.SaveLog("call saveetOprIns.ServiceId == 13 ");
                    var getIfAllPosted = await _repooprServiceCharges.GetManyAsync(c => c.ServiceId == 13 && c.ServiceItbId == ServiceItbId);
                    if (getIfAllPosted.Count() > 0)
                    {
                        string PostedStatus = _configuration["Statuses:PostedStatus"];

                        var getIfOneisNotPosted = getIfAllPosted.Where(c => c.Status != PostedStatus).ToList();

                        if (getIfOneisNotPosted.Count() > 0)
                        {

                        }
                        else
                        {

                            var getOverdR = await _repoOprOverDraft.GetAsync(c => c.ItbId == getOprIns.ServiceItbId);
                            if (getOverdR != null)
                            {
                                var res = await ApproveOD(getOverdR, UserName);
                            }

                        }
                    }
                }

                if (getOprIns.ServiceId == 2)
                {
                    _LogManager.SaveLog("call saveetOprIns.ServiceId == 2 ");
                    var getIfAllPosted = await _repooprServiceCharges.GetManyAsync(c => c.ServiceId == 2 && c.ServiceItbId == ServiceItbId);
                    _LogManager.SaveLog("call saveetOprIns.ServiceId == 2 is getIfAllPosted " + getIfAllPosted);
                    if (getIfAllPosted.Count() > 0)
                    {
                        string PostedStatus = _configuration["Statuses:PostedStatus"];

                        var getIfOneisNotPosted = getIfAllPosted.Where(c => c.Status != PostedStatus).ToList();

                        if (getIfOneisNotPosted.Count() > 0)
                        {

                        }
                        else
                        {

                            var geChq = await _repoOprChqBookRequest.GetAsync(c => c.ItbId == getOprIns.ServiceItbId);
                            if (geChq != null)
                            {
                                geChq.Status = status;
                                _repoOprChqBookRequest.Update(geChq);
                                int Save2 = await _ApplicationDbContext.SaveChanges(UserId);
                                if (Save2 > 0)
                                {
                                    ApiResponse.ResponseMessage = "Success";
                                    ApiResponse.ResponseCode = 0;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _LogManager.SaveLog("call ex " + ex);
            }
            return ApiResponse;
        }

        public async Task<RevCalChargeModel> ChectTransRefExist(decimal ItbId, string TransReference)
        {
            _LogManager.SaveLog("call ChectTransRefExist ");

            DynamicParameters param = new DynamicParameters();
            var rtn = new DapperDATAImplementation<RevCalChargeModel>();

            param.Add("@pnItbId", ItbId);
            param.Add("@psTransReference", TransReference);
            _LogManager.SaveLog("call ItbId params == " + ItbId);
            _LogManager.SaveLog("call TransReference params == " + TransReference);
            var getTransRef = await rtn.ResponseObj("Isp_TransactionLogCheck", param, db);
            return getTransRef;
        }

        public async Task<PostTransactionResponse> PostTransactionsEthix(CbsTransaction cbs, string username)
        {


            GetIpForPostingTrans GetIpForPostingTrans = new GetIpForPostingTrans(_configuration, _LogManager);

            _LogManager.SaveLog("inside PostTransactionsEthix method ");
            _LogManager.SaveLog("cbs call parameters  " + cbs);
            _LogManager.SaveLog("cbs username parameters  " + username);
            PostTransactionResponse oPostTransactionResponse = new PostTransactionResponse();
            try
            {

                if (!GetIpForPostingTrans.GetIp())
                {
                    oPostTransactionResponse.nErrorCode = 99;
                    oPostTransactionResponse.sErrorText = "Invalid Server to Post Transaction, Kindly contact the System Administration";
                    return oPostTransactionResponse;
                }
                _LogManager.SaveLog(" valid server");




                var clientProfile = await _repoadmClientProfile.GetAsync(null);

                var get = await _FunctionApiSetUpImplementation.GetConnectDetails("PostTransaction");

                var bankServ = await _repoadmBankServiceSetup.GetAsync(c => c.Itbid == get.ConnectionId);

                string postDate = _Formatter.FormatDateCurrProcessing(cbs.TransactionDate);
                string valueDate = _Formatter.FormatDateCurrProcessing(cbs.ValueDate);

                _LogManager.SaveLog("cbs call postDate  " + postDate);
                _LogManager.SaveLog("cbs call valueDate  " + valueDate);

                TransactionPosting values = new TransactionPosting()
                {
                    TransRef = cbs.TransReference,
                    DrAccountNo = cbs.DrAcctNo,
                    DrAcctType = cbs.DrAcctType,
                    DrAcctTC = cbs.DrAcctTC,
                    DrAcctNarration = cbs.DrAcctNarration,
                    TranAmount = cbs.Amount.ToString(),
                    CrAcctNo = cbs.CrAcctNo,
                    CrAcctType = cbs.CrAcctType,
                    CrAcctTC = cbs.CrAcctTC,
                    CrAcctNarration = cbs.CrAcctNarration,
                    CurrencyISO = cbs.CcyCode,
                    PostDate = postDate != null ? postDate.Trim() : postDate,
                    ValueDate = valueDate != null ? valueDate.Trim() : valueDate,
                    UserName = username != null ? username.ToUpper() : null,
                    TranBatchID = cbs.BatchId != null ? cbs.BatchId.ToString() : null,
                    SupervisorName = cbs.CbsSupervisorId,
                    ChannelId = clientProfile.ChannelId,
                    ForcePostFlag = null,
                    Reversal = "2",// DetermineReversalCode(cbs.DrAcctType, cbs.CrAcctType, cbs.ParentTransactionId),
                    DrAcctChargeCode = cbs.DrAcctChargeCode,
                    DrAcctChargeAmt = cbs.DrAcctChargeAmt == null ? null : cbs.DrAcctChargeAmt.ToString(),
                    DrAcctTaxAmt = cbs.DrAcctTaxAmt == null ? null : cbs.DrAcctTaxAmt.ToString(),
                    DrAcctChequeNo = cbs.DrAcctChequeNo == null ? null : cbs.DrAcctChequeNo.ToString(),
                    DrAcctChgDescr = cbs.DrAcctChargeNarr,
                    CrAcctChargeCode = cbs.CrAcctChargeCode,
                    CrAcctChargeAmt = cbs.CrAcctChargeAmt == null ? null : cbs.CrAcctChargeAmt.ToString(),
                    CrAcctTaxAmt = cbs.CrAcctTaxAmt == null ? null : cbs.CrAcctTaxAmt.ToString(),
                    CrAcctChequeNo = cbs.CrAcctChequeNo == null ? null : cbs.CrAcctChequeNo.ToString(),
                    CrAcctChgDescr = cbs.CrAcctChargeNarr,
                    TransTracer = cbs.TransTracer,
                    ParentTransRef = cbs.ParentTransactionId == null ? null : cbs.ParentTransactionId,
                    RoutingNo = null,
                    FloatDays = null,
                    RimNo = null,
                    Direction = cbs.Direction == null ? null : cbs.Direction.ToString(),
                    DrAcctCashAmt = cbs.DrAcctCashAmt == null ? null : cbs.DrAcctCashAmt.ToString(),
                    CrAcctCashAmt = cbs.CrAcctCashAmt == null ? null : cbs.CrAcctCashAmt.ToString(),
                    DrAcctChargeBranch = cbs.DrAcctBranchCode,
                    CrAcctChargeBranch = cbs.CrAcctBranchCode == null ? null : cbs.CrAcctBranchCode.ToString(),
                    ConnectionStringId = get.ConnectionId == null ? 1 : get.ConnectionId

                };

                _LogManager.SaveLog("cbs call Transactionposting values  " + values);
                var json = JsonConvert.SerializeObject(values);

                _TransactionLogger.SaveTransLog(JsonConvert.SerializeObject(values), cbs.TransReference);
                var postdata = new StringContent(json, Encoding.UTF8, "application/json");

                var client = new HttpClient();
                var url = bankServ.WebServiceUrl.Trim() + "/api/Services/PostTransaction";

                var response = await client.PostAsync(url, postdata);

                string result = response.Content.ReadAsStringAsync().Result;
                //_LogManager.SaveLog("cbs call result  " + result);
                oPostTransactionResponse = JsonConvert.DeserializeObject<PostTransactionResponse>(result);

                //_LogManager.SaveLog("cbs call oPostTransactionResponse values  " + oPostTransactionResponse);
                if (oPostTransactionResponse.nErrorCode != 0)
                {

                    _LogManager.SaveLog("cbs call oPostTransactionResponse if errorcode != 0  " + oPostTransactionResponse.nErrorCode);
                    //"Core Banking Message: {COREBANKINGMSG}. Core Banking Returned Code{COREBANKINGCODE} Dr Acct {DRACTNO} Dr Acct Type {DRACCTTYPE} Cr Acct No. {CRACCTNO} Cr Acct Type {CRACCTTYPE}
                    string msg = _configuration["Message:PostTransaction"].ToString();
                    oPostTransactionResponse.sErrorText = msg.Replace("{COREBANKINGMSG}", oPostTransactionResponse.sErrorText).Replace("{COREBANKINGCODE}", oPostTransactionResponse.nErrorCode.ToString()).Replace("{DRACTNO}", values.DrAccountNo).Replace("{DRACCTTYPE}", values.DrAcctType).Replace("{CRACCTNO}", values.CrAcctNo).Replace("{CRACCTTYPE}", values.CrAcctType);



                }

                _TransactionLogger.SaveTransLog($"Response Code:  {oPostTransactionResponse.nErrorCode} Response Message: {oPostTransactionResponse.sErrorText}", cbs.TransReference);

            }
            catch (Exception ex)
            {
                var exM = ex.Message == null ? ex.InnerException.ToString() : ex.Message.ToString();

                _TransactionLogger.SaveTransLog($"Internal Error While Posting Transaction with TransRef:  { cbs.TransReference }", $"Error: {exM}");
            }
            oPostTransactionResponse.TransReference = cbs.TransReference;
            _LogManager.SaveLog("oPostTransactionResponse.TransReference  " + oPostTransactionResponse.TransReference);
            return oPostTransactionResponse;
        }

        public async Task<ApproveOverDraftResponse> ApproveOD(OprOverDraft ovDraft, string Username = "System")
        {
            ApproveOverDraftResponse oApproveResponse = new ApproveOverDraftResponse();

            try
            {
                var clientProfile = await _repoadmClientProfile.GetAsync(null);

                var get = await _FunctionApiSetUpImplementation.GetConnectDetails("ApproveOd");

                if (get.ConnectionId == null)
                    get.ConnectionId = 1;

                var bankServ = await _repoadmBankServiceSetup.GetAsync(c => c.Itbid == get.ConnectionId);

                string ExpiryDate = _Formatter.FormatDateCurrProcessing(ovDraft.NewExpiryDate);

                var values = new
                {
                    AcctNo = ovDraft.AcctNo,
                    AcctType = ovDraft.AcctType,
                    ODLimit = ovDraft.ApprovedLimit,
                    ODExpDate = ExpiryDate,
                    ODRate = ovDraft.ApprovedOdRate,
                    UserName = Username,
                    ODType = string.IsNullOrWhiteSpace(ovDraft.ODType) ? null : ovDraft.ODType.ToUpper().ToString(),
                    ConnectionStringId = get.ConnectionId
                };

                var json = JsonConvert.SerializeObject(values);

                var postdata = new StringContent(json, Encoding.UTF8, "application/json");

                var client = new HttpClient();

                var url = bankServ.WebServiceUrl.Trim() + "/api/Services/ApproveOd";

                var response = await client.PostAsync(url, postdata);

                string result = response.Content.ReadAsStringAsync().Result;

                oApproveResponse = JsonConvert.DeserializeObject<ApproveOverDraftResponse>(result);

            }
            catch (Exception ex)
            {
                var exM = ex.Message == null ? ex.InnerException.ToString() : ex.Message.ToString();
                _LogManager.SaveLog($"Internal Error While Approving OD:  { ovDraft.ItbId } Error: {exM}");
            }

            return oApproveResponse;
        }

        public string DetermineReversalCode(string DrAcctType, string CrAcctType, string parentTranRef)
        {
            if (!string.IsNullOrEmpty(DrAcctType) && !string.IsNullOrEmpty(CrAcctType))
            {
                if (DrAcctType.Trim() == "GL" && CrAcctType.Trim() == "GL" && !string.IsNullOrEmpty(parentTranRef))
                {
                    return "0";

                }

                else if ((DrAcctType.Trim() == "SA" || DrAcctType.Trim() == "CA") && !string.IsNullOrEmpty(parentTranRef))
                {
                    return "1";
                }

                else if ((CrAcctType.Trim() == "SA" || CrAcctType.Trim() == "CA") && !string.IsNullOrEmpty(parentTranRef))
                {
                    return "1";
                }
                else
                {
                    return "0";
                }

            }

            return "0";
        }

        public CbsTransaction passCbsToNewCBS(CbsTransaction cbs1)
        {
            var cbs = new CbsTransaction();

            cbs.ServiceId = cbs1.ServiceId;
            cbs.DrAcctBranchCode = cbs1.DrAcctBranchCode;
            cbs.DrAcctNo = cbs1.DrAcctNo;
            cbs.DrAcctType = cbs1.DrAcctType;
            cbs.DrAcctName = cbs1.DrAcctName;
            cbs.DrAcctBalance = cbs1.DrAcctBalance;
            cbs.DrAcctStatus = cbs1.DrAcctStatus;
            cbs.DrAcctTC = cbs1.DrAcctTC;
            cbs.DrAcctNarration = cbs1.DrAcctNarration;
            cbs.DrAcctAddress = cbs1.DrAcctAddress;
            cbs.DrAcctClassCode = cbs1.DrAcctClassCode;
            cbs.DrAcctChequeNo = cbs1.DrAcctChequeNo;
            cbs.DrAcctChargeCode = cbs1.DrAcctChargeCode;
            cbs.DrAcctChargeAmt = cbs1.DrAcctChargeAmt;
            cbs.DrAcctTaxAmt = cbs1.DrAcctTaxAmt;
            cbs.DrAcctChargeRate = cbs1.DrAcctChargeRate;
            cbs.DrAcctChargeNarr = cbs1.DrAcctChargeNarr;
            cbs.DrAcctBalAfterPost = cbs1.DrAcctBalAfterPost;
            cbs.CrAcctBalAfterPost = cbs1.CrAcctBalAfterPost;
            cbs.DrAcctOpeningDate = cbs1.DrAcctOpeningDate;
            cbs.DrAcctIndusSector = cbs1.DrAcctIndusSector;
            cbs.DrAcctCbsTranId = cbs1.DrAcctCbsTranId;
            cbs.DrAcctCustType = cbs1.DrAcctCustType;
            cbs.DrAcctCustNo = cbs1.DrAcctCustNo;
            cbs.DrAcctCashBalance = cbs1.DrAcctCashBalance;
            cbs.DrAcctCashAmt = cbs1.DrAcctCashAmt;
            cbs.DrAcctCity = cbs1.DrAcctCity;
            cbs.DrAcctIncBranch = cbs1.DrAcctIncBranch;
            cbs.DrAcctValUserId = cbs1.DrAcctValUserId;
            cbs.DrAcctValErrorCode = cbs1.DrAcctValErrorCode;
            cbs.DrAcctValErrorMsg = cbs1.DrAcctValErrorMsg;
            cbs.CcyCode = cbs1.CcyCode;
            cbs.TaxRate = cbs1.TaxRate;
            cbs.Amount = cbs1.Amount;
            cbs.CrAcctBranchCode = cbs1.CrAcctBranchCode;
            cbs.CrAcctNo = cbs1.CrAcctNo;
            cbs.CrAcctType = cbs1.CrAcctType;
            cbs.CrAcctName = cbs1.CrAcctName;
            cbs.CrAcctBalance = cbs1.CrAcctBalance;
            cbs.CrAcctStatus = cbs1.CrAcctStatus;
            cbs.CrAcctTC = cbs1.CrAcctTC;
            cbs.CrAcctNarration = cbs1.CrAcctNarration;
            cbs.CrAcctAddress = cbs1.CrAcctAddress;
            cbs.CrAcctProdCode = cbs1.CrAcctProdCode;
            cbs.CrAcctChequeNo = cbs1.CrAcctChequeNo;
            cbs.CrAcctChargeCode = cbs1.CrAcctChargeCode;
            cbs.CrAcctChargeAmt = cbs1.CrAcctChargeAmt;
            cbs.CrAcctTaxAmt = cbs1.CrAcctTaxAmt;
            cbs.CrAcctChargeRate = cbs1.CrAcctChargeRate;
            cbs.CrAcctChargeNarr = cbs1.CrAcctChargeNarr;
            cbs.CrAcctOpeningDate = cbs1.CrAcctOpeningDate;
            cbs.CrAcctIndusSector = cbs1.CrAcctIndusSector;
            cbs.CrAcctCbsTranId = cbs1.CrAcctCbsTranId;
            cbs.CrAcctCustType = cbs1.CrAcctCustType;
            cbs.CrAcctCustNo = cbs1.CrAcctCustNo;
            cbs.CrAcctCashBalance = cbs1.CrAcctCashBalance;
            cbs.CrAcctCashAmt = cbs1.CrAcctCashAmt;
            cbs.CrAcctCity = cbs1.CrAcctCity;
            cbs.CrAcctIncBranch = cbs1.CrAcctIncBranch;
            cbs.CrAcctValUserId = cbs1.CrAcctValUserId;
            cbs.CrAcctValErrorCode = cbs1.CrAcctValErrorCode;
            cbs.CrAcctValErrorMsg = cbs1.CrAcctValErrorMsg;
            cbs.TransactionDate = cbs1.TransactionDate;
            cbs.ValueDate = cbs1.ValueDate;
            cbs.DateCreated = DateTime.Now;
            cbs.Status = cbs1.Status;
            cbs.TransReference = cbs1.TransReference;
            cbs.TransTracer = cbs1.TransTracer;
            cbs.ChannelId = cbs1.ChannelId;
            cbs.DeptId = cbs1.DeptId;
            cbs.ProcessingDept = cbs1.ProcessingDept;
            cbs.BatchId = cbs1.BatchId;
            cbs.BatchSeqNo = cbs1.BatchSeqNo;
            cbs.PostingDate = cbs1.PostingDate;
            cbs.UserId = cbs1.UserId;
            cbs.CbsUserId = cbs1.CbsUserId;
            cbs.SupervisorId = cbs1.SupervisorId;
            cbs.CbsSupervisorId = cbs1.CbsSupervisorId;
            cbs.Direction = cbs1.Direction;
            cbs.PrimaryId = cbs1.PrimaryId;
            cbs.PostingErrorCode = cbs1.PostingErrorCode;
            cbs.PostingErrorDescr = cbs1.PostingErrorDescr;
            cbs.Rejected = cbs1.Rejected;
            cbs.Reversal = cbs1.Reversal;
            cbs.RejectedBy = cbs1.RejectedBy;
            cbs.RejectionDate = cbs1.RejectionDate;
            cbs.ReversedBy = cbs1.ReversedBy;
            cbs.ReversalDate = cbs1.ReversalDate;
            cbs.OriginatingBranchId = cbs1.OriginatingBranchId;
            cbs.ParentTransactionId = cbs1.ParentTransactionId;
            cbs.CbsChannelId = cbs1.CbsChannelId;
            cbs.LcyEquivAmt = cbs1.LcyEquivAmt;
            cbs.LclEquivExchRate = cbs1.LclEquivExchRate;
            cbs.AuthRejection = cbs1.AuthRejection;
            cbs.RejectedIds = cbs1.RejectedIds;

            return cbs;
        }

        public CbsTransaction CbsExchangeDrForCr(CbsTransaction cbsDr, CbsTransaction cbsCr)
        {
            var cbs = new CbsTransaction();

            cbs.ServiceId = cbsDr.ServiceId;
            cbs.DrAcctBranchCode = cbsCr.CrAcctBranchCode;
            cbs.DrAcctNo = cbsCr.CrAcctNo;
            cbs.DrAcctType = cbsCr.CrAcctType;
            cbs.DrAcctName = cbsCr.CrAcctName;
            cbs.DrAcctBalance = cbsCr.CrAcctBalance;
            cbs.DrAcctStatus = cbsCr.CrAcctStatus;
            cbs.DrAcctTC = cbsCr.CrAcctTC;
            cbs.DrAcctNarration = cbsCr.CrAcctNarration;
            cbs.DrAcctAddress = cbsCr.CrAcctAddress;
            //cbs.DrAcctClassCode = cbsCr.CrAcctClassCode;
            cbs.DrAcctChequeNo = cbsCr.CrAcctChequeNo;
            cbs.DrAcctChargeCode = cbsCr.CrAcctChargeCode;
            cbs.DrAcctChargeAmt = cbsCr.CrAcctChargeAmt;
            cbs.DrAcctTaxAmt = cbsCr.CrAcctTaxAmt;
            cbs.DrAcctChargeRate = cbsCr.CrAcctChargeRate;
            cbs.DrAcctChargeNarr = cbsCr.CrAcctChargeNarr;
            cbs.DrAcctBalAfterPost = cbsCr.CrAcctBalAfterPost;
            cbs.DrAcctOpeningDate = cbsCr.CrAcctOpeningDate;
            cbs.DrAcctIndusSector = cbsCr.CrAcctIndusSector;
            cbs.DrAcctCbsTranId = cbsCr.CrAcctCbsTranId;
            cbs.DrAcctCustType = cbsCr.CrAcctCustType;
            cbs.DrAcctCustNo = cbsCr.CrAcctCustNo;
            cbs.DrAcctCashBalance = cbsCr.CrAcctCashBalance;
            cbs.DrAcctCashAmt = cbsCr.CrAcctCashAmt;
            cbs.DrAcctCity = cbsCr.CrAcctCity;
            cbs.DrAcctIncBranch = cbsCr.CrAcctIncBranch;
            cbs.DrAcctValUserId = cbsCr.CrAcctValUserId;
            cbs.DrAcctValErrorCode = cbsCr.CrAcctValErrorCode;
            cbs.DrAcctValErrorMsg = cbsCr.CrAcctValErrorMsg;

            // CR Leg

            cbs.CrAcctBalAfterPost = cbsDr.DrAcctBalAfterPost;
            cbs.CcyCode = cbsDr.CcyCode;
            cbs.TaxRate = cbsDr.TaxRate;
            cbs.Amount = cbsDr.Amount;
            cbs.CrAcctBranchCode = cbsDr.DrAcctBranchCode;
            cbs.CrAcctNo = cbsDr.DrAcctNo;
            cbs.CrAcctType = cbsDr.DrAcctType;
            cbs.CrAcctName = cbsDr.DrAcctName;
            cbs.CrAcctBalance = cbsDr.DrAcctBalance;
            cbs.CrAcctStatus = cbsDr.DrAcctStatus;
            cbs.CrAcctTC = cbsDr.DrAcctTC;
            cbs.CrAcctNarration = cbsDr.DrAcctNarration;
            cbs.CrAcctAddress = cbsDr.DrAcctAddress;
            // cbs.CrAcctProdCode = cbsDr.DrAcctProdCode ;
            cbs.CrAcctChequeNo = cbsDr.DrAcctChequeNo;
            cbs.CrAcctChargeCode = cbsDr.DrAcctChargeCode;
            cbs.CrAcctChargeAmt = cbsDr.DrAcctChargeAmt;
            cbs.CrAcctTaxAmt = cbsDr.DrAcctTaxAmt;
            cbs.CrAcctChargeRate = cbsDr.DrAcctChargeRate;
            cbs.CrAcctChargeNarr = cbsDr.DrAcctChargeNarr;
            cbs.CrAcctOpeningDate = cbsDr.DrAcctOpeningDate;
            cbs.CrAcctIndusSector = cbsDr.DrAcctIndusSector;
            cbs.CrAcctCbsTranId = cbsDr.DrAcctCbsTranId;
            cbs.CrAcctCustType = cbsDr.DrAcctCustType;
            cbs.CrAcctCustNo = cbsDr.DrAcctCustNo;
            cbs.CrAcctCashBalance = cbsDr.DrAcctCashBalance;
            cbs.CrAcctCashAmt = cbsDr.DrAcctCashAmt;
            cbs.CrAcctCity = cbsDr.DrAcctCity;
            cbs.CrAcctIncBranch = cbsDr.DrAcctIncBranch;
            cbs.CrAcctValUserId = cbsDr.DrAcctValUserId;
            cbs.CrAcctValErrorCode = cbsDr.DrAcctValErrorCode;
            cbs.CrAcctValErrorMsg = cbsDr.DrAcctValErrorMsg;

            cbs.TransactionDate = cbsDr.TransactionDate;
            cbs.ValueDate = cbsDr.ValueDate;
            cbs.DateCreated = DateTime.Now;
            cbs.Status = cbsDr.Status;
            cbs.TransReference = cbsDr.TransReference;
            cbs.TransTracer = cbsDr.TransTracer;
            cbs.ChannelId = cbsDr.ChannelId;
            cbs.DeptId = cbsDr.DeptId;
            cbs.ProcessingDept = cbsDr.ProcessingDept;
            cbs.BatchId = cbsDr.BatchId;
            cbs.BatchSeqNo = cbsDr.BatchSeqNo;
            cbs.PostingDate = cbsDr.PostingDate;
            cbs.UserId = cbsDr.UserId;
            cbs.CbsUserId = cbsDr.CbsUserId;
            cbs.SupervisorId = cbsDr.SupervisorId;
            cbs.CbsSupervisorId = cbsDr.CbsSupervisorId;
            cbs.Direction = cbsDr.Direction;
            cbs.PrimaryId = cbsDr.PrimaryId;
            cbs.PostingErrorCode = cbsDr.PostingErrorCode;
            cbs.PostingErrorDescr = cbsDr.PostingErrorDescr;
            cbs.Rejected = cbsDr.Rejected;
            cbs.Reversal = cbsDr.Reversal;
            cbs.RejectedBy = cbsDr.RejectedBy;
            cbs.RejectionDate = cbsDr.RejectionDate;
            cbs.ReversedBy = cbsDr.ReversedBy;
            cbs.ReversalDate = cbsDr.ReversalDate;
            cbs.OriginatingBranchId = cbsDr.OriginatingBranchId;
            cbs.ParentTransactionId = cbsDr.ParentTransactionId;
            cbs.CbsChannelId = cbsDr.CbsChannelId;
            cbs.LcyEquivAmt = cbsDr.LcyEquivAmt;
            cbs.LclEquivExchRate = cbsDr.LclEquivExchRate;
            cbs.AuthRejection = cbsDr.AuthRejection;
            cbs.RejectedIds = cbsDr.RejectedIds;

            return cbs;
        }


    }
}