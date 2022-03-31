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
using RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO;
using RevAssuranceApi.OperationImplemention;
using RevAssuranceApi.RevenueAssurance.Repository.Interface;
using RevAssuranceApi.Helper;

namespace RevAssuranceApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [EnableCors("CorsPolicy")]
    public class ApproveBatchController : ControllerBase
    {
        IConfiguration _configuration;
        ApiResponse ApiResponse = new ApiResponse();
        TokenGenerator TokenGenerator;
        AppSettingsPath AppSettingsPath;
        IDbConnection db = null;
        ApplicationDbContext _ApplicationDbContext;
        RoleAssignImplementation _RoleAssignImplementation;
        IRepository<admService> _repoadmService;
        IRepository<CbsTransaction> _repoCbsTransaction;
        ChargeImplementation _ChargeImplementation;
        IRepository<BatchControl> _repoBatchControl;
        IRepository<admTransactionConfiguration> _repoadmTransactionConfiguration;
        IRepository<BatchItems> _repoBatchItems;

        Formatter _Formatter = new Formatter();
        UsersImplementation _UsersImplementation;
        AccountValidationImplementation _AccountValidationImplementation;
        ComputeChargesImplementation _ComputeChargesImplementation;
        ServiceChargeImplementation _ServiceChargeImplementation;
        CBSTransImplementation _CBSTransImplementation;
         LogManager _LogManager;
        public ApproveBatchController(IConfiguration configuration,
                                      ApplicationDbContext ApplicationDbContext,
                                      RoleAssignImplementation RoleAssignImplementation,
                                      IRepository<BatchControl> repoBatchControl,
                                      IRepository<admService> repoadmService,
                                      ChargeImplementation ChargeImplementation,
                                      IRepository<BatchItems> repoBatchItems,
                                      UsersImplementation UsersImplementation,
                                      AccountValidationImplementation AccountValidationImplementation,
                                      ComputeChargesImplementation ComputeChargesImplementation,
                                      ServiceChargeImplementation ServiceChargeImplementation,
                                      IRepository<admTransactionConfiguration> repoadmTransactionConfiguration,
                                      CBSTransImplementation CBSTransImplementation,
                                      IRepository<CbsTransaction> repoCbsTransaction,
                                       LogManager LogManager
                            )
        {
            _configuration = configuration;
            AppSettingsPath = new AppSettingsPath(_configuration);
            TokenGenerator = new TokenGenerator(_configuration);
            db = new SqlConnection(AppSettingsPath.GetDefaultCon());
            _ApplicationDbContext = ApplicationDbContext;
            _RoleAssignImplementation = RoleAssignImplementation;
            _repoBatchControl = repoBatchControl;
            _repoadmService = repoadmService;
            _ChargeImplementation = ChargeImplementation;
            _repoBatchItems = repoBatchItems;
            _UsersImplementation = UsersImplementation;
            _AccountValidationImplementation = AccountValidationImplementation;
            _ComputeChargesImplementation = ComputeChargesImplementation;
            _ServiceChargeImplementation = ServiceChargeImplementation;
            _repoadmTransactionConfiguration = repoadmTransactionConfiguration;
            _CBSTransImplementation = CBSTransImplementation;
            _repoCbsTransaction = repoCbsTransaction;
            _LogManager = LogManager;
        }

        [HttpPost("GetAll")]
        public async Task<IActionResult> GetAll(ParamLoadPage AnyAuth)
        {
            try
            {
                var rtn = new DapperDATAImplementation<BatchControlDTONew>();
                DynamicParameters param = new DynamicParameters();

                var RoleAssign = await _RoleAssignImplementation.GetRoleAssign(AnyAuth.MenuId, AnyAuth.RoleId);
                string IsGlobalView = "N";

                if (RoleAssign != null)
                {
                    IsGlobalView = RoleAssign.IsGlobalSupervisor == true ? "Y" : "N";
                }

                param.Add("@pdtCurrentDate", _Formatter.FormatToDateYearMonthDay(AnyAuth.pdtCurrentDate));
                param.Add("@psBranchNo", AnyAuth.psBranchNo);
                param.Add("@pnDeptId", AnyAuth.pnDeptId);
                param.Add("@pnGlobalView", IsGlobalView);
                param.Add("@ServiceId", AnyAuth.ServiceId);

                var _response = await rtn.LoadData("isp_ApproveBatchItem", param, db);

                if (_response != null)
                {
                    var roleAssign = await _RoleAssignImplementation.GetRoleAssign(AnyAuth.MenuId, AnyAuth.RoleId);
                    var admService = await _repoadmService.GetAsync(c => c.ServiceId == AnyAuth.ServiceId);

                    var getCharge = await _ChargeImplementation.GetChargeDetails(AnyAuth.ServiceId);

                    var listoprServiceCharges = new List<oprServiceCharges>();

                    if (getCharge.Count > 0)
                    {
                        foreach (var b in getCharge)
                        {
                            listoprServiceCharges.Add(new oprServiceCharges
                            {

                                ItbId = 0,
                                ServiceId = (int)b.ServiceId,
                                ServiceItbId = 0,
                                ChgAcctNo = null,
                                ChgAcctType = null,
                                ChgAcctName = null,
                                ChgAvailBal = null,
                                ChgAcctCcy = null,
                                ChgAcctStatus = null,
                                ChargeCode = b.ChargeCode,
                                ChargeRate = null,
                                OrigChgAmount = null,
                                OrigChgCCy = null,
                                ExchangeRate = null,
                                EquivChgAmount = null,
                                EquivChgCcy = null,
                                ChgNarration = null,
                                TaxAcctNo = null,
                                TaxAcctType = null,
                                TaxRate = null,
                                TaxAmount = null,
                                TaxNarration = null,
                                IncBranch = null,
                                IncAcctNo = null,
                                IncAcctType = null,
                                IncAcctName = null,
                                IncAcctBalance = null,
                                IncAcctStatus = null,
                                IncAcctNarr = null,
                                SeqNo = null,
                                Status = null,
                                DateCreated = null,
                                TemplateId = b.TemplateId
                            });
                        }
                    }

                    var res = new
                    {
                        _response = _response,
                        roleAssign = roleAssign,
                        admService = admService,
                        charge = listoprServiceCharges,
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

        [HttpPost("GetAllIncomplete")]
        public async Task<IActionResult> GetAllIncomplete(ParamLoadPage AnyAuth)
        {
            try
            {
                var rtn = new DapperDATAImplementation<BatchControlDTONew>();
                DynamicParameters param = new DynamicParameters();

                var RoleAssign = await _RoleAssignImplementation.GetRoleAssign(AnyAuth.MenuId, AnyAuth.RoleId);
                string IsGlobalView = "N";

                if (RoleAssign != null)
                {
                    IsGlobalView = RoleAssign.IsGlobalSupervisor == true ? "Y" : "N";
                }

                param.Add("@pdtCurrentDate", _Formatter.FormatToDateYearMonthDay(AnyAuth.pdtCurrentDate));
                param.Add("@psBranchNo", AnyAuth.psBranchNo);
                param.Add("@pnDeptId", AnyAuth.pnDeptId);
                param.Add("@pnGlobalView", IsGlobalView);
                param.Add("@ServiceId", AnyAuth.ServiceId);

                var _response = await rtn.LoadData("isp_InCompleteBatchItem", param, db);

                if (_response.Count() > 0)
                {
                    var res = new
                    {
                        _response = _response
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
        public async Task<IActionResult> Add(BatchControlDTO p)
        {
            try
            {
                BatchControl batch = new BatchControl();

                batch = p.BatchControl;

                batch.Status = "Loaded";
                batch.DateCreated = DateTime.Now;

                await _repoBatchControl.AddAsync(batch);
                int rev = await _ApplicationDbContext.SaveChanges(p.LoginUserId);

                if (rev > 0)
                {

                    ApiResponse.ResponseCode = 0;
                    ApiResponse.ResponseMessage = "Processed Successfully";
                    var res = new
                    {
                        batch = batch,
                        ApiResponse = ApiResponse
                    };
                    return Ok(res);
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

        [HttpPost("Update")]
        public async Task<IActionResult> Update(BatchControlDTO p)
        {
            try
            {
                BatchControl batch = new BatchControl();

                batch = p.BatchControl;

                _repoBatchControl.Update(batch);

                int rev = await _ApplicationDbContext.SaveChanges(p.LoginUserId);

                if (rev > 0)
                {
                    ApiResponse.ResponseCode = 0;
                    ApiResponse.ResponseMessage = "Processed Successfully";

                    return Ok(ApiResponse);
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


        [HttpPost("AddItem")]
        public async Task<IActionResult> AddItem(BatchControlDTO p)
        {
            try
            {
                BatchItems batchItem = new BatchItems();

                batchItem = p.BatchItems;

                string UnauthorizedStatus = _configuration["Statuses:UnauthorizedStatus"];
                batchItem.DateCreated = DateTime.Now;
                batchItem.Status = UnauthorizedStatus;
                batchItem.UserId = p.LoginUserId;

                await _repoBatchItems.AddAsync(batchItem);
                int rev = await _ApplicationDbContext.SaveChanges(p.LoginUserId);

                if (rev > 0)
                {

                    ApiResponse.ResponseCode = 0;
                    ApiResponse.ResponseMessage = "Processed Successfully";

                    return Ok(ApiResponse);
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
        public async Task<IActionResult> GetById(BatchControlDTO p)
        {
            try
            {
                var instrumentDetails = await _repoBatchControl.GetAsync(c => c.BatchNo == p.BatchControl.BatchNo);
                instrumentDetails.RecordCount = instrumentDetails.RecordCount == null ? 0 : instrumentDetails.RecordCount;
                instrumentDetails.PostedTransCount = instrumentDetails.PostedTransCount == null ? 0 : instrumentDetails.PostedTransCount;
                instrumentDetails.TotalDr = instrumentDetails.TotalDr == null ? Convert.ToDecimal("0.00") : instrumentDetails.TotalDr;
                instrumentDetails.TotalCr = instrumentDetails.TotalCr == null ? Convert.ToDecimal("0.00") : instrumentDetails.TotalCr;
                instrumentDetails.TDifference = instrumentDetails.TDifference == null ? Convert.ToDecimal("0.00") : instrumentDetails.TDifference;
                if (instrumentDetails != null)
                {

                    var allUsers = await _UsersImplementation.GetAllUser(instrumentDetails.LoadedBy, instrumentDetails.RejectedBy, 0);

                    var batchItems = await _repoBatchItems.GetManyAsync(c => c.BatchNo == instrumentDetails.BatchNo);

                    var res = new
                    {
                        instrumentDetails = instrumentDetails,
                        allUsers = allUsers,
                        batchItems = batchItems
                    };

                    return Ok(res);

                }
            }
            catch (Exception ex)
            {
                var exM = ex;
                return BadRequest();
            }

            return BadRequest();
        }

        [HttpPost("GetByIdItem")]
        public async Task<IActionResult> GetByIdItem(BatchControlDTO p)
        {
            try
            {
                var instrumentDetails = await _repoBatchItems.GetAsync(c => c.ItbId == p.BatchItems.ItbId);

                if (instrumentDetails != null)
                {

                    var allUsers = await _UsersImplementation.GetAllUser(instrumentDetails.UserId, 0, 0);
                    var serviceChargeslist = new List<oprServiceCharges>();
                    serviceChargeslist.Add(new oprServiceCharges
                    {
                        ServiceId = p.ServiceId,
                        ServiceItbId = instrumentDetails.ItbId,
                        ChgAcctNo = instrumentDetails.AcctNo,
                        ChgAcctType = instrumentDetails.AcctType,
                        ChgAcctName = instrumentDetails.AcctName,
                        ChgAvailBal = instrumentDetails.AcctBalance,
                        ChgAcctCcy = instrumentDetails.CcyCode,
                        ChgAcctStatus = instrumentDetails.AcctStatus,
                        ChargeCode = instrumentDetails.ChargeCode,
                        ChargeRate = instrumentDetails.TaxRate,
                        OrigChgAmount = instrumentDetails.ChargeAmount,
                        OrigChgCCy = instrumentDetails.CcyCode,
                        ExchangeRate = null,// instrumentDetails.ItbId,
                        EquivChgAmount = null,// instrumentDetails.ItbId,
                        EquivChgCcy = instrumentDetails.CcyCode,
                        ChgNarration = instrumentDetails.ChgNarration,
                        TaxAcctNo = instrumentDetails.TaxAcctNo,
                        TaxAcctType = instrumentDetails.TaxAcctType,
                        TaxRate = instrumentDetails.TaxRate,
                        TaxAmount = instrumentDetails.TaxAmount,
                        TaxNarration = instrumentDetails.TaxNarration,
                        IncBranch = instrumentDetails.IncBranch,
                        IncAcctNo = instrumentDetails.IncAcctNo,
                        IncAcctType = instrumentDetails.IncAcctType,
                        IncAcctName = instrumentDetails.IncAcctName,
                        IncAcctBalance = instrumentDetails.IncAcctBalance,
                        IncAcctStatus = instrumentDetails.IncAcctStatus,
                        IncAcctNarr = instrumentDetails.IncAcctNarr,
                        // SeqNo  = null,
                        // Status  = null,
                        // DateCreated  = null,

                    });

                    if (instrumentDetails.ChargeCode == null)
                    {
                        serviceChargeslist = null;
                    }

                    var res = new
                    {
                        instrumentDetails = instrumentDetails,
                        allUsers = allUsers,
                        serviceChargeslist = serviceChargeslist,

                    };

                    return Ok(res);

                }
            }
            catch (Exception ex)
            {
                var exM = ex;
                return BadRequest();
            }

            return BadRequest();
        }

        [HttpPost("CalCulateCharge")]
        public async Task<IActionResult> CalCulateCharge(BatchControlDTO p)
        {
            try
            {
                AccountValParam AccountValParam = new AccountValParam();

                AccountValParam.AcctNo = p.AcctNo;
                AccountValParam.AcctType = p.AcctType == null ? null : p.AcctType.Trim();

                AccountValParam.CrncyCode = p.CcyCode;
                AccountValParam.Username = p.LoginUserName;

                var InstrumentAcctDetails = await _AccountValidationImplementation.ValidateAccountCall(AccountValParam);
                InstrumentAcctDetails.sRsmId = InstrumentAcctDetails.sCustomerId != null ? Convert.ToInt32(InstrumentAcctDetails.sCustomerId) : 0;

                List<RevCalChargeModel> chgList = new List<RevCalChargeModel>();
                if (p.ChargeThisAcct != false)
                {
                    foreach (var i in p.ListoprServiceCharge)
                    {
                        AccountValParam AccountVal = new AccountValParam();

                        AccountVal.AcctType = i.ChgAcctType == null ? InstrumentAcctDetails.sAccountType : i.ChgAcctType;
                        AccountVal.AcctNo = i.ChgAcctNo == null ? p.AcctNo : i.ChgAcctNo;
                        AccountVal.CrncyCode = InstrumentAcctDetails.sCrncyIso.Trim();
                        AccountVal.Username = p.LoginUserName;

                        var ChgDetails = await _AccountValidationImplementation.ValidateAccountCall(AccountVal);

                        OperationViewModel OperationViewModel = new OperationViewModel();

                        decimal amt = p.TransAmout != null ? _Formatter.ValDecimal(p.TransAmout) : 0;
                        OperationViewModel.serviceId = p.ServiceId;
                        OperationViewModel.TransAmount = amt.ToString();
                        OperationViewModel.InstrumentAcctNo = i.ChgAcctNo == null ? p.AcctNo : i.ChgAcctNo;
                        OperationViewModel.InstrumentCcy = InstrumentAcctDetails.sCrncyIso;
                        OperationViewModel.ChargeCode = i.ChargeCode;

                        int? TempId = i.TemplateId != null ? _Formatter.ValIntergers(i.TemplateId.ToString()) : 0;

                        OperationViewModel.TempTypeId = TempId == 0 ? null : TempId;

                        var calCharge = await _ComputeChargesImplementation.CalChargeModel(OperationViewModel,
                                                                                            ChgDetails.nBranch.ToString(), ChgDetails.sAccountType, ChgDetails.sCrncyIso);
                        calCharge.chgAcctNo = AccountVal.AcctNo;
                        calCharge.chgAcctType = AccountVal.AcctType;
                        calCharge.chgAcctName = ChgDetails.sName;
                        calCharge.chgAvailBal = ChgDetails.nBalance;
                        calCharge.chgAcctCcy = ChgDetails.sCrncyIso;
                        calCharge.chgAcctStatus = ChgDetails.sStatus;
                        calCharge.TemplateId = TempId;

                        AccountValParam AccountIncomeVal = new AccountValParam();

                        AccountIncomeVal.AcctType = calCharge.sChargeIncAcctType;
                        AccountIncomeVal.AcctNo = calCharge.sChargeIncAcctNo;
                        AccountIncomeVal.CrncyCode = calCharge.sChgCurrency;
                        AccountIncomeVal.Username = p.LoginUserName;

                        var IncomeDetails = await _AccountValidationImplementation.ValidateAccountCall(AccountIncomeVal);


                        calCharge.incBranch = IncomeDetails.nBranch;
                        calCharge.incBranchString = IncomeDetails.sBranchName;
                        calCharge.incAcctNo = calCharge.sChargeIncAcctNo;
                        calCharge.incAcctType = calCharge.sChargeIncAcctType;
                        calCharge.incAcctName = IncomeDetails.sName;
                        calCharge.incAcctBalance = IncomeDetails.nBalanceDec;
                        calCharge.incAcctBalanceString = IncomeDetails.nBalance;
                        calCharge.incAcctStatus = IncomeDetails.sStatus;
                        calCharge.incAcctNarr = calCharge.sNarration;
                        calCharge.chargeCode = i.ChargeCode;

                        calCharge.chargeRate = calCharge.nChargeRate != null ? calCharge.nChargeRate.ToString() : "0";
                        calCharge.origChgAmount = calCharge.nOrigChargeAmount != null ? calCharge.nOrigChargeAmount.ToString() : "0";
                        calCharge.exchangeRate = calCharge.nExchRate != null ? calCharge.nExchRate.ToString() : "0";
                        calCharge.equivChgAmount = calCharge.nActualChgAmt != null ? calCharge.nActualChgAmt.ToString() : "0";
                        calCharge.taxAmount = calCharge.nTaxAmt != null ? calCharge.nTaxAmt.ToString() : "0";


                        chgList.Add(calCharge);

                    }

                }


                if (p.ChargeThisAcct == false)
                {
                    chgList = null;
                }

                var chg = new
                {
                    InstrumentAcctDetails = InstrumentAcctDetails,
                    chgList = chgList,


                };

                return Ok(chg);

            }
            catch (Exception ex)
            {
                ApiResponse.ResponseMessage = ex == null ? ex.InnerException.Message : ex.Message;

                ApiResponse.ResponseCode = -99;
                return BadRequest(ApiResponse);
            }
        }

        [HttpPost("AddBatchItem")]
        public async Task<IActionResult> AddBatchItem(BatchControlDTO p)
        {
            try
            {
                int rev = -1;
                BatchItems batchItemChg = new BatchItems();

                batchItemChg = p.BatchItems;

                string UnauthorizedStatus = _configuration["Statuses:UnauthorizedStatus"];
                batchItemChg.BatchNo = (long)p.BatchNo;
                batchItemChg.ServiceStatus = UnauthorizedStatus;
                batchItemChg.DateCreated = DateTime.Now;
                batchItemChg.TransactionDate = Convert.ToDateTime(p.TransactionDate);
                batchItemChg.ValueDate = Convert.ToDateTime(p.ValueDate);
                batchItemChg.BatchSeqNo = 1;
                batchItemChg.UserId = p.LoginUserId;
                //var serviceRef = await _ComputeChargesImplementation.GenServiceRef(p.ServiceId);
                //batchItem.ReferenceNo   = serviceRef.nReference;
                await _repoBatchItems.AddAsync(batchItemChg);

                if (p.ChargeThisAcct == false)
                {
                    AccountValParam AccountIncomeVal = new AccountValParam();

                    AccountIncomeVal.AcctType = batchItemChg.AcctType;
                    AccountIncomeVal.AcctNo = batchItemChg.AcctNo;
                    AccountIncomeVal.CrncyCode = batchItemChg.CcyCode;
                    AccountIncomeVal.Username = p.LoginUserName;

                    var IncomeDetails = await _AccountValidationImplementation.ValidateAccountCall(AccountIncomeVal);

                    batchItemChg.City = IncomeDetails.sCity;
                    batchItemChg.ValUserId = _Formatter.ValIntergers(IncomeDetails.sCustomerId);
                    batchItemChg.ValErrorCode = IncomeDetails.nErrorCode;
                    batchItemChg.ValErrorMsg = IncomeDetails.sErrorText;

                    batchItemChg.ClassCode = IncomeDetails.sProductCode;
                    batchItemChg.OpeningDate = _Formatter.ValidateDate(IncomeDetails.sAcctOpenDate);
                    batchItemChg.IndusSector = IncomeDetails.sSector;
                    batchItemChg.CustType = IncomeDetails.sIdentityType;
                    batchItemChg.CustNo = _Formatter.ValIntergers(IncomeDetails.sCustomerId);
                    batchItemChg.RsmId = _Formatter.ValIntergers(IncomeDetails.sCustomerId);
                    batchItemChg.CashBalance = _Formatter.ValDecimal(IncomeDetails.nCashBalance);

                    rev = await _ApplicationDbContext.SaveChanges(p.LoginUserId);
                }
                else
                {
                    List<RevCalChargeModel> chgList = new List<RevCalChargeModel>();

                    int SeqNo = 0;
                    foreach (var b in p.ListoprServiceCharge)
                    {
                        SeqNo += 1;

                        b.ServiceId = p.ServiceId;
                        // var taxnar =  b.TaxNarration +" Ref: "+ serviceRef.nReference;
                        // b.TaxNarration = taxnar;
                        // b.SeqNo = SeqNo;
                        // var Innar =  b.IncAcctNarr +" Ref: "+ serviceRef.nReference;
                        // b.IncAcctNarr = Innar;

                        batchItemChg.ServiceId = batchItemChg.ServiceId;
                        batchItemChg.BatchNo = batchItemChg.BatchNo;
                        batchItemChg.BranchNo = batchItemChg.BranchNo;
                        batchItemChg.AcctNo = batchItemChg.AcctNo;
                        batchItemChg.AcctType = batchItemChg.AcctType;
                        batchItemChg.AcctName = batchItemChg.AcctName;
                        batchItemChg.AcctBalance = batchItemChg.AcctBalance;
                        batchItemChg.AcctStatus = batchItemChg.AcctStatus;
                        batchItemChg.CbsTC = batchItemChg.CbsTC;
                        batchItemChg.ChequeNo = batchItemChg.ChequeNo;
                        batchItemChg.CcyCode = batchItemChg.CcyCode;
                        batchItemChg.Amount = batchItemChg.Amount;
                        batchItemChg.DrCr = batchItemChg.DrCr;
                        batchItemChg.Narration = batchItemChg.Narration;

                        var ChgNaration = b.ChgNarration + " Ref: " + batchItemChg.BatchNo;

                        batchItemChg.ChargeCode = b.ChargeCode;
                        batchItemChg.ChargeAmount = b.EquivChgAmount;
                        batchItemChg.ChgNarration = ChgNaration;
                        batchItemChg.TaxAcctNo = b.TaxAcctNo;
                        batchItemChg.TaxAcctType = b.TaxAcctType;
                        batchItemChg.TaxRate = b.TaxRate;
                        batchItemChg.TaxAmount = b.TaxAmount;
                        batchItemChg.TaxNarration = b.TaxNarration;
                        batchItemChg.IncBranch = b.IncBranch;
                        batchItemChg.IncAcctNo = b.IncAcctNo;
                        batchItemChg.IncAcctType = b.IncAcctType;
                        batchItemChg.IncAcctName = b.IncAcctName;
                        batchItemChg.IncAcctBalance = b.IncAcctBalance;
                        batchItemChg.IncAcctStatus = b.IncAcctStatus;
                        batchItemChg.IncAcctNarr = b.IncAcctNarr;

                        AccountValParam AccountIncomeVal = new AccountValParam();

                        AccountIncomeVal.AcctType = b.IncAcctType;
                        AccountIncomeVal.AcctNo = b.IncAcctNo;
                        AccountIncomeVal.CrncyCode = batchItemChg.CcyCode;
                        AccountIncomeVal.Username = p.LoginUserName;

                        var IncomeDetails = await _AccountValidationImplementation.ValidateAccountCall(AccountIncomeVal);

                        batchItemChg.ClassCode = IncomeDetails.sProductCode;
                        batchItemChg.OpeningDate = _Formatter.ValidateDate(IncomeDetails.sAcctOpenDate);
                        batchItemChg.IndusSector = IncomeDetails.sSector;
                        batchItemChg.CustType = IncomeDetails.sIdentityType;
                        batchItemChg.CustNo = _Formatter.ValIntergers(IncomeDetails.sCustomerId);
                        batchItemChg.RsmId = IncomeDetails.sRsmId;
                        batchItemChg.CashBalance = _Formatter.ValDecimal(IncomeDetails.nCashBalance);

                        if (IncomeDetails.AvailBal != null && IncomeDetails.nCashBalance != null)
                        {
                            var cashAmount = _Formatter.ValDecimal(IncomeDetails.AvailBal) - _Formatter.ValDecimal(IncomeDetails.nCashBalance);

                            batchItemChg.CashAmt = cashAmount;

                        }

                        batchItemChg.City = IncomeDetails.sCity;
                        batchItemChg.ValUserId = _Formatter.ValIntergers(IncomeDetails.sCustomerId);
                        batchItemChg.ValErrorCode = IncomeDetails.nErrorCode;
                        batchItemChg.ValErrorMsg = IncomeDetails.sErrorText;

                        batchItemChg.DateCreated = DateTime.Now;
                        batchItemChg.ServiceStatus = UnauthorizedStatus;
                        batchItemChg.Status = null;
                        batchItemChg.BatchSeqNo = SeqNo;
                        batchItemChg.UserId = p.LoginUserId;
                        batchItemChg.SupervisorId = null;
                        batchItemChg.Direction = 1;

                        batchItemChg.DismissedBy = null;
                        batchItemChg.DismissedDate = null;
                        batchItemChg.Rejected = null;
                        batchItemChg.RejectionIds = null;
                        batchItemChg.RejectionDate = null;



                        await _repoBatchItems.AddAsync(batchItemChg);
                        rev = await _ApplicationDbContext.SaveChanges(p.LoginUserId);
                    }

                    if (rev > 0)
                    {
                        ApiResponse.ResponseMessage = "Your Request was Successful!";

                        var res = new
                        {

                            ApiResponse = ApiResponse
                        };
                        return Ok(ApiResponse);
                    }

                }

                if (rev > 0)
                {
                    ApiResponse.ResponseMessage = "Your Request was Successful!";
                    // var getAll = await _repoBatchItems.GetManyAsync(c=> c.BatchNo == p.BatchNo);

                    var res = new
                    {
                        // getAll = getAll,
                        ApiResponse = ApiResponse
                    };
                    return Ok(ApiResponse);
                }



                ApiResponse.ResponseMessage = "Error Occured!";
                return BadRequest(ApiResponse);

            }
            catch (Exception ex)
            {
                ApiResponse.ResponseMessage = ex == null ? ex.InnerException.Message : ex.Message;

                ApiResponse.ResponseCode = -99;
                return BadRequest(ApiResponse);
            }
        }

        [HttpPost("UpdateBatchItem")]
        public async Task<IActionResult> UpdateBatchItem(BatchControlDTO p)
        {
            try
            {
                int rev = -1;
                BatchItems batchItemChg = new BatchItems();

                batchItemChg = p.BatchItems;


                batchItemChg.UserId = p.LoginUserId;

                _repoBatchItems.Update(batchItemChg);

                if (p.ChargeThisAcct == false)
                {
                    AccountValParam AccountIncomeVal = new AccountValParam();

                    AccountIncomeVal.AcctType = batchItemChg.AcctType;
                    AccountIncomeVal.AcctNo = batchItemChg.AcctNo;
                    AccountIncomeVal.CrncyCode = batchItemChg.CcyCode;
                    AccountIncomeVal.Username = p.LoginUserName;

                    var IncomeDetails = await _AccountValidationImplementation.ValidateAccountCall(AccountIncomeVal);

                    batchItemChg.City = IncomeDetails.sCity;
                    batchItemChg.ValUserId = _Formatter.ValIntergers(IncomeDetails.sCustomerId);
                    batchItemChg.ValErrorCode = IncomeDetails.nErrorCode;
                    batchItemChg.ValErrorMsg = IncomeDetails.sErrorText;

                    batchItemChg.ClassCode = IncomeDetails.sProductCode;
                    batchItemChg.OpeningDate = _Formatter.ValidateDate(IncomeDetails.sAcctOpenDate);
                    batchItemChg.IndusSector = IncomeDetails.sSector;
                    batchItemChg.CustType = IncomeDetails.sIdentityType;
                    batchItemChg.CustNo = _Formatter.ValIntergers(IncomeDetails.sCustomerId);
                    batchItemChg.RsmId = _Formatter.ValIntergers(IncomeDetails.sCustomerId);
                    batchItemChg.CashBalance = _Formatter.ValDecimal(IncomeDetails.nCashBalance);

                    rev = await _ApplicationDbContext.SaveChanges(p.LoginUserId);
                }
                else
                {
                    List<RevCalChargeModel> chgList = new List<RevCalChargeModel>();

                    int SeqNo = 0;
                    foreach (var b in p.ListoprServiceCharge)
                    {
                        SeqNo += 1;

                        b.ServiceId = p.ServiceId;
                        // var taxnar =  b.TaxNarration +" Ref: "+ serviceRef.nReference;
                        // b.TaxNarration = taxnar;
                        // b.SeqNo = SeqNo;
                        // var Innar =  b.IncAcctNarr +" Ref: "+ serviceRef.nReference;
                        // b.IncAcctNarr = Innar;

                        batchItemChg.ServiceId = batchItemChg.ServiceId;
                        batchItemChg.BatchNo = batchItemChg.BatchNo;
                        batchItemChg.BranchNo = batchItemChg.BranchNo;
                        batchItemChg.AcctNo = batchItemChg.AcctNo;
                        batchItemChg.AcctType = batchItemChg.AcctType;
                        batchItemChg.AcctName = batchItemChg.AcctName;
                        batchItemChg.AcctBalance = batchItemChg.AcctBalance;
                        batchItemChg.AcctStatus = batchItemChg.AcctStatus;
                        batchItemChg.CbsTC = batchItemChg.CbsTC;
                        batchItemChg.ChequeNo = batchItemChg.ChequeNo;
                        batchItemChg.CcyCode = batchItemChg.CcyCode;
                        batchItemChg.Amount = batchItemChg.Amount;
                        batchItemChg.DrCr = batchItemChg.DrCr;
                        batchItemChg.Narration = batchItemChg.Narration;

                        var ChgNaration = b.ChgNarration;

                        batchItemChg.ChargeCode = b.ChargeCode;
                        batchItemChg.ChargeAmount = b.EquivChgAmount;
                        batchItemChg.ChgNarration = ChgNaration;
                        batchItemChg.TaxAcctNo = b.TaxAcctNo;
                        batchItemChg.TaxAcctType = b.TaxAcctType;
                        batchItemChg.TaxRate = b.TaxRate;
                        batchItemChg.TaxAmount = b.TaxAmount;
                        batchItemChg.TaxNarration = b.TaxNarration;
                        batchItemChg.IncBranch = b.IncBranch;
                        batchItemChg.IncAcctNo = b.IncAcctNo;
                        batchItemChg.IncAcctType = b.IncAcctType;
                        batchItemChg.IncAcctName = b.IncAcctName;
                        batchItemChg.IncAcctBalance = b.IncAcctBalance;
                        batchItemChg.IncAcctStatus = b.IncAcctStatus;
                        batchItemChg.IncAcctNarr = b.IncAcctNarr;

                        AccountValParam AccountIncomeVal = new AccountValParam();

                        AccountIncomeVal.AcctType = b.IncAcctType;
                        AccountIncomeVal.AcctNo = b.IncAcctNo;
                        AccountIncomeVal.CrncyCode = batchItemChg.CcyCode;
                        AccountIncomeVal.Username = p.LoginUserName;

                        var IncomeDetails = await _AccountValidationImplementation.ValidateAccountCall(AccountIncomeVal);

                        batchItemChg.ClassCode = IncomeDetails.sProductCode;
                        batchItemChg.OpeningDate = _Formatter.ValidateDate(IncomeDetails.sAcctOpenDate);
                        batchItemChg.IndusSector = IncomeDetails.sSector;
                        batchItemChg.CustType = IncomeDetails.sIdentityType;
                        batchItemChg.CustNo = _Formatter.ValIntergers(IncomeDetails.sCustomerId);
                        batchItemChg.RsmId = IncomeDetails.sRsmId;
                        batchItemChg.CashBalance = _Formatter.ValDecimal(IncomeDetails.nCashBalance);

                        if (IncomeDetails.AvailBal != null && IncomeDetails.nCashBalance != null)
                        {
                            var cashAmount = _Formatter.ValDecimal(IncomeDetails.AvailBal) - _Formatter.ValDecimal(IncomeDetails.nCashBalance);

                            batchItemChg.CashAmt = cashAmount;

                        }

                        batchItemChg.City = IncomeDetails.sCity;
                        batchItemChg.ValUserId = _Formatter.ValIntergers(IncomeDetails.sCustomerId);
                        batchItemChg.ValErrorCode = IncomeDetails.nErrorCode;
                        batchItemChg.ValErrorMsg = IncomeDetails.sErrorText;



                        batchItemChg.BatchSeqNo = SeqNo;
                        batchItemChg.UserId = p.LoginUserId;
                        // batchItemChg.SupervisorId			= null;
                        batchItemChg.Direction = 1;

                        // batchItemChg.DismissedBy			= null;
                        // batchItemChg.DismissedDate			= null;
                        // batchItemChg.Rejected				= null ;
                        // batchItemChg.RejectionIds			= null ;
                        // batchItemChg.RejectionDate			= null ;



                        _repoBatchItems.Update(batchItemChg);
                        rev = await _ApplicationDbContext.SaveChanges(p.LoginUserId);
                    }

                    if (rev > 0)
                    {
                        ApiResponse.ResponseMessage = "Your Request was Successful!";

                        var res = new
                        {

                            ApiResponse = ApiResponse
                        };
                        return Ok(ApiResponse);
                    }

                }

                if (rev > 0)
                {
                    ApiResponse.ResponseMessage = "Your Request was Successful!";
                    // var getAll = await _repoBatchItems.GetManyAsync(c=> c.BatchNo == p.BatchNo);

                    var res = new
                    {
                        // getAll = getAll,
                        ApiResponse = ApiResponse
                    };
                    return Ok(ApiResponse);
                }



                ApiResponse.ResponseMessage = "Error Occured!";
                return BadRequest(ApiResponse);

            }
            catch (Exception ex)
            {
                ApiResponse.ResponseMessage = ex == null ? ex.InnerException.Message : ex.Message;

                ApiResponse.ResponseCode = -99;
                return BadRequest(ApiResponse);
            }
        }


        [HttpPost("RemoveBatch")]
        public async Task<IActionResult> RemoveBatch(BatchControlDTO p)
        {
            ApiResponse apiResponse = new ApiResponse();

            try
            {
                StringBuilder sb = new StringBuilder("<ul>");

                foreach (var b in p.ListBatchControl)
                {
                    int rev = -1;
                    var get = await _repoBatchControl.GetAsync(c => c.BatchNo == b.BatchNo);
                    get.Status = "Closed";

                    _repoBatchControl.Update(get);

                    rev = await _ApplicationDbContext.SaveChanges(p.LoginUserId);
                    if (rev > 0)
                    {
                        sb.Append($"<li> Success: Batch No: { b.BatchNo }  Removed Successfully!</li>");

                    }
                    else
                    {
                        sb.Append($"<li> Failed: Batch No: { b.BatchNo }  Did'nt Removed</li>");

                    }
                }
                sb.Append("</ul>");
                apiResponse.ResponseMessage = sb.ToString();

                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                var exM = ex;
            }

            apiResponse.ResponseMessage = "Error Occured!";

            return BadRequest(apiResponse);


        }


        [HttpPost("GetTransConfig")]
        public async Task<IActionResult> GetTransConfig(BatchControlDTO p)
        {
            try
            {
                var admTransactionConfiguration = await _repoadmTransactionConfiguration.GetAsync(c => c.ServiceId == p.ServiceId);


                return Ok(admTransactionConfiguration);




            }
            catch (Exception ex)
            {
                var exM = ex;
                return BadRequest();
            }

            return BadRequest();
        }

        [HttpPost("ProcessAllItems")]
        public async Task<IActionResult> ProcessAllItems(BatchControlDTO p)
        {
            int rev = -1;
            try
            {

                foreach (var i in p.ListBatchControl)
                {

                    var rtn = new DapperDATAImplementation<BatchItemsTemp>();
                    string script = $"select * from BatchItemsTemp where batchNo in({i.BatchNo})";
                    var transactions = await rtn.LoadListNoParam(script, db);


                    foreach (var b in transactions)
                    {


                        BatchItems item = new BatchItems();

                        item.ServiceId = b.ServiceId;
                        item.BatchNo = b.BatchNo;
                        item.BranchNo = b.BranchNo;
                        item.AcctNo = b.AcctNo;
                        item.AcctType = b.AcctType;
                        item.AcctName = null; // b.AcctName ;
                        item.AcctBalance = null; // b.AcctBalance ;
                        item.AcctStatus = null; // b.AcctStatus ;
                        item.CbsTC = b.CbsTC;
                        item.ChequeNo = null; // b.ChequeNo ;
                        item.CcyCode = b.CcyCode;
                        item.Amount = b.Amount;
                        item.DrCr = b.DrCr;
                        item.Narration = b.Narration;
                        item.ChargeCode = b.ChargeCode;
                        item.ChargeAmount = b.ChargeAmount;
                        item.ChgNarration = b.ChgNarration;
                        item.TaxAcctNo = null; //b.TaxAcctNo ;
                        item.TaxAcctType = null;//b.TaxAcctType ;
                        item.TaxRate = null;//b.TaxRate ;
                        item.TaxAmount = null; //b.TaxAmount ;
                        item.TaxNarration = null;//b.TaxNarration ;
                        item.IncBranch = null;//b.IncBranch ;
                        item.IncAcctNo = null;//b.IncAcctNo ;
                        item.IncAcctType = null; //b.IncAcctType ;
                        item.IncAcctName = null; //b.IncAcctName ;
                        item.IncAcctBalance = null;//b.IncAcctBalance ;
                        item.IncAcctStatus = null; //b.IncAcctStatus ;
                        item.IncAcctNarr = null;//b.IncAcctNarr ;
                        item.ClassCode = null;// b.ClassCode ;
                        item.OpeningDate = null; //b.OpeningDate ;
                        item.IndusSector = null; //b.IndusSector ;
                        item.CustType = null;//b.CustType ; //
                        item.CustNo = null;//b.CustNo ;
                        item.RsmId = null; //b.RsmId ;
                        item.CashBalance = null; //b. ;
                        item.CashAmt = null; //b. ;
                        item.City = null;//b. ;
                        item.ValUserId = null; //b. ;
                        item.ValErrorCode = null;//b. ;
                        item.ValErrorMsg = null;//b. ;
                        item.TransactionDate = null;//b. ;
                        item.ValueDate = null;//b. ;
                        item.DateCreated = DateTime.Now;
                        item.ServiceStatus = b.ServiceStatus;
                        item.Status = null; //b. ;
                        item.DeptId = b.DeptId;
                        item.ProcessingDept = b.ProcessingDept;
                        item.BatchSeqNo = b.BatchSeqNo;
                        item.UserId = b.UserId;
                        item.SupervisorId = null; //b.SupervisorId ;
                        item.Direction = null; //b. ;
                        item.OriginatingBranchId = b.OriginatingBranchId;
                        item.DismissedBy = null;//b. ;
                        item.DismissedDate = null;//b. ;
                        item.Rejected = null;//b. ;
                        item.RejectionIds = null;//b. ;
                        item.RejectionDate = null;//b. ;
                        item.ReferenceNo = b.ReferenceNo;

                        await _repoBatchItems.AddAsync(item);
                        rev = await _ApplicationDbContext.SaveChanges(p.LoginUserId);

                    }
                }

                if (rev > 0)
                {
                    ApiResponse.ResponseCode = 0;
                    ApiResponse.ResponseMessage = "Processed Successfully";

                    return Ok(ApiResponse);
                }
            }
            catch (Exception ex)
            {
                var exM = ex;
                return BadRequest();
            }

            return BadRequest();
        }

        [HttpPost("ApproveTrans")]
        public async Task<IActionResult> ApproveTrans(BatchControlDTO p)
        {
            int batchSucess = 0, failure = 0, tranPostForBatch = 0, TransNotPostForBatch = 0;
            StringBuilder sb = new StringBuilder("<ul>");
             var SubtringClass = new List<SubtringClassDTO>();
				 int postErrorCode = -999999;
            string postErrorText = string.Empty;
				
										 
            try
            {
                GetIpForPostingTrans GetIpForPostingTrans = new GetIpForPostingTrans(_configuration, _LogManager);
                if(!GetIpForPostingTrans.GetIp()) {

                    ApiResponse.ResponseCode =  99;
                    ApiResponse.ResponseMessage = "Invalid Server to Post Transaction, Kindly contact the System Administration";
                    return BadRequest(ApiResponse);
                }

                string PostedStatus = _configuration["Statuses:PostedStatus"];

              

                foreach (var a in p.ListBatchControl)
                {               
                    var rtn = new DapperDATAImplementation<CbsTransInsertBatchDTO>();
                    DynamicParameters param = new DynamicParameters();

                    string TransDate = _Formatter.FormatToDateYearMonthDay(p.TransactionDate);

                    param.Add("@pnServiceId", p.ServiceId);
                    param.Add("@pnServiceItbId", a.BatchNo);
                    param.Add("@pdtTransnDate", TransDate);

                    var Batchitems = await rtn.LoadData("Isp_CbsTransInsert", param, db);
                 
                    foreach (var b in Batchitems)
                    {        
                        var cbs = await _repoCbsTransaction.GetAsync(c => c.ItbId == b.ItbId);
                        var checkTransExit = await _CBSTransImplementation.ChectTransRefExist(cbs.ItbId, cbs.TransReference);
                        if (checkTransExit.nReturnCode == -1)
                        {
                            sb.Append($"<li> Failed: Trans. Ref: {cbs.TransReference} Failed. Reason: { checkTransExit.sReturnMsg } </li>");

                            failure++;
                            ApiResponse.ResponseCode = checkTransExit.nReturnCode;
                            ApiResponse.ResponseMessage = checkTransExit.sReturnMsg;
                            continue;
                        }

                        var postResp = await _CBSTransImplementation.PostTransactionsEthix(cbs, p.LoginUserName);

                        if (postResp != null)
                        {
                                ApiResponse.ResponseCode = postResp.nErrorCode;
                                ApiResponse.ResponseMessage = postResp.sErrorText;
                                SubtringClass.Add(new SubtringClassDTO { TransTracer = cbs.TransTracer, PostErrorCode =  postResp.nErrorCode, PostErrorText = postResp.sErrorText });
                            if (postResp.nErrorCode == 0)
                            {
                                cbs.PostingErrorCode = postResp.nErrorCode;
                                cbs.PostingErrorDescr = postResp.sErrorText;
                                cbs.UserId = p.LoginUserId;
                                cbs.CbsUserId = p.LoginUserName;
                                cbs.DrAcctCashAmt = !string.IsNullOrEmpty(postResp.drCashAmt) ? decimal.Parse(postResp.drCashAmt) : 0.00m;
                                cbs.DrAcctCbsTranId = _Formatter.ValDecimal(postResp.CbsTranId);
                                cbs.CrAcctCbsTranId = _Formatter.ValDecimal(postResp.CbsTranId);
                                cbs.DrAcctBalAfterPost = !string.IsNullOrEmpty(postResp.nDrBalance) ? decimal.Parse(postResp.nDrBalance) : 0.00m;
                                cbs.CrAcctBalAfterPost = !string.IsNullOrEmpty(postResp.nCrBalance) ? decimal.Parse(postResp.nCrBalance) : 0.00m;
                                cbs.PostingDate = DateTime.Now;

                                if (postResp.sErrorText.Trim().ToLower() != "success")
                                {
                                    TransNotPostForBatch += 1;
                                    _repoCbsTransaction.Update(cbs);

                                    var update = await _CBSTransImplementation.CbsTransUpdate(cbs, (int)p.LoginUserId);
                                }
                                else if (postResp.sErrorText.Trim().ToLower() == "success")
                                {
                                    cbs.Status = PostedStatus;

                                    _repoCbsTransaction.Update(cbs);

                                    var update = await _CBSTransImplementation.CbsTransUpdate(cbs, (int)p.LoginUserId);

                                    var itemTBL = await _repoBatchItems.GetAsync(c => c.ItbId == cbs.PrimaryId);

                                    itemTBL.Status = PostedStatus;

                                    int updatePrimary = await _ApplicationDbContext.SaveChanges(p.LoginUserId);

                                    tranPostForBatch += 1;
                                    var getBatchDetails = await _repoBatchControl.GetAsync(c => c.BatchNo == a.BatchNo);

                                    getBatchDetails.PostedTransCount = tranPostForBatch;

                                    getBatchDetails.PostedDrCount = getBatchDetails.PostedDrCount == null ? 0 : getBatchDetails.PostedDrCount;
                                    getBatchDetails.PostedCrCount = getBatchDetails.PostedCrCount == null ? 0 : getBatchDetails.PostedCrCount;

                                    if (itemTBL.DrCr.Trim().ToLower() == "dr")
                                    {

                                        int postCount = _Formatter.ValIntergers(getBatchDetails.PostedDrCount.ToString()) + 1;
                                        getBatchDetails.PostedDrCount = postCount;
                                    }
                                    if (itemTBL.DrCr.Trim().ToLower() == "cr")
                                    {
                                        int postCount = _Formatter.ValIntergers(getBatchDetails.PostedCrCount.ToString()) + 1;
                                        getBatchDetails.PostedCrCount = postCount;
                                    }

                                    _repoBatchControl.Update(getBatchDetails);
                                    int upbatch = await _ApplicationDbContext.SaveChanges(p.LoginUserId);

                                    
                                }


                                ApiResponse.ResponseCode = postResp.nErrorCode;
                                ApiResponse.ResponseMessage = postResp.sErrorText;

                                continue;

                            }
                            else
                            {

                                var deleteTransRef = await _CBSTransImplementation.ChectTransRefExist(cbs.ItbId, cbs.TransReference);

                                cbs.PostingErrorCode = postResp.nErrorCode;
                                cbs.PostingErrorDescr = postResp.sErrorText;
                                ApiResponse.ResponseCode = postResp.nErrorCode;
                                ApiResponse.ResponseMessage = postResp.sErrorText;
                                cbs.UserId = p.LoginUserId;
                                var update = await _CBSTransImplementation.CbsTransUpdate(cbs, (int)p.LoginUserId);

                                failure++;
                                continue;
                            }
                        }
                    }

                    batchSucess += 1;

                    if (tranPostForBatch == Batchitems.Count())
                    {
                        var getBatchDetails = await _repoBatchControl.GetAsync(c => c.BatchNo == a.BatchNo);
                        getBatchDetails.Status = PostedStatus;
                        int upbatch = await _ApplicationDbContext.SaveChanges(p.LoginUserId);

                        sb.Append($"<li> Success: Batch No- { a.BatchNo } Posted Successful!. Trans Count: { tranPostForBatch }</li>");

                    }
                    else
                    {
                        var getBatchDetails = await _repoBatchControl.GetAsync(c => c.BatchNo == a.BatchNo);
                        int faliedTransCount = Batchitems.Count() - tranPostForBatch;
                        getBatchDetails.PostedTransCount = tranPostForBatch;

                        getBatchDetails.Status = _configuration["Statuses:Incomplete"];
                        _repoBatchControl.Update(getBatchDetails);
                        int upbatch = await _ApplicationDbContext.SaveChanges(p.LoginUserId);

                        sb.Append($"<li> Batch No- { a.BatchNo } Success: Trans Count: { tranPostForBatch } Failed: Trans Count: { faliedTransCount }</li>");

                    }
                }

                /*if (batchSucess == p.ListBatchControl.Count())
                {
                    sb.Append("</ul>");
                    ApiResponse.StringbuilderMessage = sb.ToString();
                    ApiResponse.ResponseMessage = sb.ToString();

                    return Ok(ApiResponse);
                }
                else
                {
                    ApiResponse.ResponseCode = -1;
                    ApiResponse.ResponseMessage = "Batch not Processed Successfully!";
                    return BadRequest(ApiResponse);
                }
            
            */

              var getTransPostAttemp = new List<SubtringClassDTO>();

                int TotalPostedSucess = 0, TotalPostedFailed = 0;

               foreach(var res in SubtringClass)
                {
                    TotalPostedSucess = SubtringClass.Count(c=> c.PostErrorCode == 0 );
                    TotalPostedFailed = SubtringClass.Count(c=> c.PostErrorCode != 0);
                    var ToTalTransTracerCount = SubtringClass.Count(c=> c.TransTracer  == res.TransTracer);

                     var getSubtringClass = getTransPostAttemp.FirstOrDefault(c => c.TransTracer == res.TransTracer);
                     string UnSuccesfulTrans = TotalPostedFailed > 0 ? $"{TotalPostedFailed} UnSuccessful transaction(s) Reason: {res.PostErrorText}" : string.Empty;

                     if(getSubtringClass == null)
                     sb.Append($"<li> TransTracer: { res.TransTracer }- { TotalPostedSucess } Successful transaction(s). {UnSuccesfulTrans} TransTracer Count: {ToTalTransTracerCount}</li>");
                    
                     getTransPostAttemp.Add(new SubtringClassDTO { TransTracer = res.TransTracer, PostErrorCode =  postErrorCode, PostErrorText = postErrorText});
                }
                 
                sb.Append("</ul>");

               // rtv.StringbuilderMessage = sb2.ToString();
                //rtv.ResponseMessage = sb2.ToString();

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
            catch (Exception ex)
            {
                var exM = ex;
                return BadRequest();
            }

            sb.Append("</ul>");
            ApiResponse.StringbuilderMessage = sb.ToString();
            ApiResponse.ResponseMessage = sb.ToString();

            return BadRequest(ApiResponse);

        }
        
        
        [HttpPost("ReApproveTrans")]
        public async Task<IActionResult> ReApproveTrans(BatchControlDTO p)
        {
            int batchSucess = 0, failure = 0, tranPostForBatch = 0, 
            TransNotPostForBatch = 0,  formalPostedCount = 0;
            StringBuilder sb = new StringBuilder("<ul>");
            try
            {
                string PostedStatus = _configuration["Statuses:PostedStatus"];

                var SubtringClass = new List<SubtringClassDTO>();

                foreach (var a in p.ListBatchControl)
                {
                    var Batchitems = await _repoBatchItems.GetManyAsync(c => c.BatchNo == a.BatchNo);
                  
                    var getBatchDetailsFirst = await _repoBatchControl.GetAsync(c => c.BatchNo == a.BatchNo);
                    if(getBatchDetailsFirst.PostedTransCount != null)
                    formalPostedCount = _Formatter.ValIntergers(getBatchDetailsFirst.PostedTransCount.ToString());
                    foreach (var b in Batchitems)
                    {
                       
                        var cbs = await _repoCbsTransaction.GetAsync(c => c.PrimaryId == b.ItbId && c.ServiceId == p.ServiceId && c.Status != PostedStatus);
                        if (cbs != null)
                        {
                            var checkTransExit = await _CBSTransImplementation.ChectTransRefExist(cbs.ItbId, cbs.TransReference);
                          
                         
                            if (checkTransExit.nReturnCode == -1)
                            {
                                sb.Append($"<li> Failed: Trans. Ref: {cbs.TransReference} Failed. Reason: { checkTransExit.sReturnMsg } </li>");

                                failure++;
                                ApiResponse.ResponseCode = checkTransExit.nReturnCode;
                                ApiResponse.ResponseMessage = checkTransExit.sReturnMsg;
                                continue;
                            }

                           
                            var postResp = await _CBSTransImplementation.PostTransactionsEthix(cbs, p.LoginUserName);
                            
                            if (postResp != null)
                            {
                                if (postResp.nErrorCode == 0)
                                {
                                    cbs.PostingErrorCode = postResp.nErrorCode;
                                    cbs.PostingErrorDescr = postResp.sErrorText;
                                    cbs.UserId = p.LoginUserId;
                                    cbs.CbsUserId = p.LoginUserName;
                                    cbs.DrAcctCashAmt = !string.IsNullOrEmpty(postResp.drCashAmt) ? decimal.Parse(postResp.drCashAmt) : 0.00m;
                                    cbs.DrAcctCbsTranId = _Formatter.ValDecimal(postResp.CbsTranId);
                                    cbs.CrAcctCbsTranId = _Formatter.ValDecimal(postResp.CbsTranId);
                                    cbs.DrAcctBalAfterPost = !string.IsNullOrEmpty(postResp.nDrBalance) ? decimal.Parse(postResp.nDrBalance) : 0.00m;
                                    cbs.CrAcctBalAfterPost = !string.IsNullOrEmpty(postResp.nCrBalance) ? decimal.Parse(postResp.nCrBalance) : 0.00m;
                                    cbs.PostingDate = DateTime.Now;

                                    if (postResp.sErrorText.Trim().ToLower() != "success")
                                    {

                                        TransNotPostForBatch += 1;
                                        _repoCbsTransaction.Update(cbs);

                                        var update = await _CBSTransImplementation.CbsTransUpdate(cbs, (int)p.LoginUserId);
                                    }
                                    else if (postResp.sErrorText.Trim().ToLower() == "success")
                                    {
                                        cbs.Status = PostedStatus;

                                        _repoCbsTransaction.Update(cbs);

                                        var update = await _CBSTransImplementation.CbsTransUpdate(cbs, (int)p.LoginUserId);

                                        var itemTBL = await _repoBatchItems.GetAsync(c => c.ItbId == cbs.PrimaryId);

                                        itemTBL.Status = PostedStatus;

                                        int updatePrimary = await _ApplicationDbContext.SaveChanges(p.LoginUserId);

                                        tranPostForBatch += 1;
                                        var getBatchDetails = await _repoBatchControl.GetAsync(c => c.BatchNo == a.BatchNo);
                                        if(getBatchDetails.PostedTransCount != null)
                                        {
                                            int PostTranCount  = _Formatter.ValIntergers(getBatchDetails.PostedTransCount.ToString()) + 1;

                                            getBatchDetails.PostedTransCount = PostTranCount;
                                           
                                        }
                                        
                                        getBatchDetails.PostedDrCount = getBatchDetails.PostedDrCount == null ? 0 : getBatchDetails.PostedDrCount;
                                        getBatchDetails.PostedCrCount = getBatchDetails.PostedCrCount == null ? 0 : getBatchDetails.PostedCrCount;

                                        if (itemTBL.DrCr.Trim().ToLower() == "dr")
                                        {

                                            int postCount = _Formatter.ValIntergers(getBatchDetails.PostedDrCount.ToString()) + 1;
                                            getBatchDetails.PostedDrCount = postCount;
                                        }
                                        if (itemTBL.DrCr.Trim().ToLower() == "cr")
                                        {
                                            int postCount = _Formatter.ValIntergers(getBatchDetails.PostedCrCount.ToString()) + 1;
                                            getBatchDetails.PostedCrCount = postCount;
                                        }

                                        _repoBatchControl.Update(getBatchDetails);
                                        int upbatch = await _ApplicationDbContext.SaveChanges(p.LoginUserId);

                                         _repoBatchItems.Update(itemTBL);
                                        int upbatchItem = await _ApplicationDbContext.SaveChanges(p.LoginUserId);

                                        var getSubtringClass = SubtringClass.FirstOrDefault(c => c.BatchNo == a.BatchNo);
                                        SubtringClass.Add(new SubtringClassDTO { BatchNo = a.BatchNo });
                                    }


                                    ApiResponse.ResponseCode = postResp.nErrorCode;
                                    ApiResponse.ResponseMessage = postResp.sErrorText;

                                    continue;

                                }
                                else
                                {

                                    var deleteTransRef = await _CBSTransImplementation.ChectTransRefExist(cbs.ItbId, cbs.TransReference);

                                    cbs.PostingErrorCode = postResp.nErrorCode;
                                    cbs.PostingErrorDescr = postResp.sErrorText;
                                    ApiResponse.ResponseCode = postResp.nErrorCode;
                                    ApiResponse.ResponseMessage = postResp.sErrorText;
                                    cbs.UserId = p.LoginUserId;
                                    var update = await _CBSTransImplementation.CbsTransUpdate(cbs, (int)p.LoginUserId);

                                    failure++;
                                    continue;
                                }
                            }


                        }


                    }

                    batchSucess += 1;

                    int totalPosted = formalPostedCount + tranPostForBatch;

                    if (totalPosted == Batchitems.Count())
                    {
                        var getBatchDetails = await _repoBatchControl.GetAsync(c => c.BatchNo == a.BatchNo);
                        getBatchDetails.Status = PostedStatus;
                        int upbatch = await _ApplicationDbContext.SaveChanges(p.LoginUserId);

                        sb.Append($"<li> Success: Batch No- { a.BatchNo } Post Successful the Remaining: { tranPostForBatch } Transaction</li>");

                    }
                    else
                    {
                        var getBatchDetails = await _repoBatchControl.GetAsync(c => c.BatchNo == a.BatchNo);
                        int faliedTransCount = Batchitems.Count() - tranPostForBatch;
                        getBatchDetails.PostedTransCount = tranPostForBatch;

                        getBatchDetails.Status = _configuration["Statuses:Incomplete"];
                        _repoBatchControl.Update(getBatchDetails);
                        int upbatch = await _ApplicationDbContext.SaveChanges(p.LoginUserId);

                        int Remain = Batchitems.Count() -  totalPosted;
                        sb.Append($"<li> Batch No- { a.BatchNo } Success: Trans Count: { totalPosted } Failed: Trans Count Remains: { Remain }</li>");

                    }
                }

                if (batchSucess == p.ListBatchControl.Count())
                {
                    sb.Append("</ul>");
                    ApiResponse.StringbuilderMessage = sb.ToString();
                    ApiResponse.ResponseMessage = sb.ToString();

                    return Ok(ApiResponse);
                }
                else
                {
                    ApiResponse.ResponseCode = -1;
                    ApiResponse.ResponseMessage = "Record not Processed Successfully!";
                    return BadRequest(ApiResponse);
                }
            }
            catch (Exception ex)
            {
                var exM = ex;
                return BadRequest();
            }

            sb.Append("</ul>");
            ApiResponse.StringbuilderMessage = sb.ToString();
            ApiResponse.ResponseMessage = sb.ToString();

            return BadRequest(ApiResponse);

        }

    }
}