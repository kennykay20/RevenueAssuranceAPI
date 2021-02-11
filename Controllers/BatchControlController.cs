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

namespace RevAssuranceApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [EnableCors("CorsPolicy")]
    public class BatchControlController : ControllerBase
    {
        IConfiguration _configuration;
        ApiResponse ApiResponse = new ApiResponse();
        TokenGenerator TokenGenerator;
        AppSettingsPath AppSettingsPath;
        IDbConnection db = null;
        ApplicationDbContext _ApplicationDbContext;
        RoleAssignImplementation _RoleAssignImplementation;
        IRepository<admService> _repoadmService;
        ChargeImplementation _ChargeImplementation;
        IRepository<CbsTransaction> _repoCbsTransaction;
        IRepository<BatchControl> _repoBatchControl;
        IRepository<admTransactionConfiguration> _repoadmTransactionConfiguration;
        IRepository<BatchItems> _repoBatchItems;

        Formatter _Formatter = new Formatter();
        UsersImplementation _UsersImplementation;
        AccountValidationImplementation _AccountValidationImplementation;
        ComputeChargesImplementation _ComputeChargesImplementation;
        ServiceChargeImplementation _ServiceChargeImplementation;
        public BatchControlController(IConfiguration configuration,
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
                                      IRepository<CbsTransaction> repoCbsTransaction
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
            _repoCbsTransaction = repoCbsTransaction;
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

                var _response = await rtn.LoadData("isp_BatchControl", param, db);


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

        [HttpPost("Add")]
        public async Task<IActionResult> Add(BatchControlDTO p)
        {
            try
            {
                BatchControl batch = new BatchControl();

                batch = p.BatchControl;

                batch.Status = "Loaded";//   _configuration["Statuses:UnauthorizedStatus"];
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
            
                    var CBS = await _repoCbsTransaction.GetAsync(c=> c.PrimaryId == instrumentDetails.ItbId && c.ServiceId == p.ServiceId);

                    var res = new
                    {
                        instrumentDetails = instrumentDetails,
                        allUsers = allUsers,
                        serviceChargeslist = serviceChargeslist,
                        CBS = CBS

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


        [HttpPost("ProcessAllItem")]
        public async Task<IActionResult> ProcessAllItem(BatchControlDTO p)
        {
            int rev = -1, getNoOfSuccess = 0, batchSucess = 0;
            try
            {
                string UnauthorizedStatus = _configuration["Statuses:UnauthorizedStatus"];
                foreach (var i in p.ListBatchControl)
                {
           
                    var transactions = await _repoBatchItems.GetManyAsync(c=> c.BatchNo == i.BatchNo);

                    foreach (var b in transactions)
                    {
                        b.ServiceStatus = UnauthorizedStatus;
                        _repoBatchItems.Update(b);
                        rev = await _ApplicationDbContext.SaveChanges(p.LoginUserId);
                        getNoOfSuccess++;
                    }

                    i.Status = UnauthorizedStatus;
                    _repoBatchControl.Update(i);
                    int updatebatch = await _ApplicationDbContext.SaveChanges(p.LoginUserId);
                    if (updatebatch > 0 & getNoOfSuccess == transactions.Count())
                    {
                        batchSucess++;
                    }

                    getNoOfSuccess = 0;
                }

                if (batchSucess == p.ListBatchControl.Count() && rev > 0)
                {
                    ApiResponse.ResponseCode = 0;
                    ApiResponse.ResponseMessage = "Processed Successfully!";

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

        }


    }
}