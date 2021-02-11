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
    public class InstrumentApprovalController : ControllerBase
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
        IRepository<OprInstrument> _repoOprInstrument;

        Formatter _Formatter = new Formatter();
        UsersImplementation _UsersImplementation;
        AccountValidationImplementation _AccountValidationImplementation;
        ComputeChargesImplementation _ComputeChargesImplementation;
        ServiceChargeImplementation _ServiceChargeImplementation;
        public InstrumentApprovalController(IConfiguration configuration,
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
                                      IRepository<CbsTransaction> repoCbsTransaction,
                                      IRepository<OprInstrument> repoOprInstrument
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
            _repoOprInstrument = repoOprInstrument;
        }

        [HttpPost("GetAll")]
        public async Task<IActionResult> GetAll(ParamLoadPage AnyAuth)
        {
            try
            {
                var rtn = new DapperDATAImplementation<OprInstrumentDTO>();
                DynamicParameters param = new DynamicParameters();

                var RoleAssign = await _RoleAssignImplementation.GetRoleAssign(AnyAuth.MenuId, AnyAuth.RoleId);


                param.Add("@pnUserId", AnyAuth.UserId);
                param.Add("@BranchNo", AnyAuth.psBranchNo);
                param.Add("@pnDeptId", AnyAuth.pnDeptId);
                param.Add("@psBranches", null);
                param.Add("@psIsGlobalSupervisor", RoleAssign.IsGlobalSupervisor);

                var _response = await rtn.LoadData("Isp_ApprInstruments", param, db);


                if (_response != null)
                {
                    var getAllInstrument = await
                            _repoadmService.GetManyAsync(c => c.ServiceId == 11 || c.ServiceId == 14 || c.ServiceId == 18 || c.ServiceId == 19);


                    var res = new
                    {
                        _response = _response,
                        getAllInstrument = getAllInstrument,
                        roleAssign = RoleAssign

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

                    var CBS = await _repoCbsTransaction.GetAsync(c => c.PrimaryId == instrumentDetails.ItbId && c.ServiceId == p.ServiceId);

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

                    var transactions = await _repoBatchItems.GetManyAsync(c => c.BatchNo == i.BatchNo);

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

        [HttpPost("ApproveInstrumentTrans")]
        public async Task<IActionResult> ApproveInstrumentTrans(InstrumentDTO p)
        {
            int batchSucess = 0;
            StringBuilder sb = new StringBuilder("<ul>");
            try
            {
                string UnPostedStatus = _configuration["Statuses:UnPostedStatus"];

                var SubtringClass = new List<SubtringClassDTO>();

                foreach (var a in p.ListOprInstrument)
                {
                    var rtn = new DapperDATAImplementation<CbsTransInsertBatchDTO>();
                    DynamicParameters param = new DynamicParameters();

                    string TransDate = _Formatter.FormatToDateYearMonthDay(p.TransactionDate);

                    param.Add("@pnServiceId", a.ServiceId);
                    param.Add("@pnServiceItbId", a.ItbId);
                    param.Add("@pdtTransnDate", TransDate);

                    var insertRes = await rtn.LoadData("Isp_CbsTransInsert", param, db);

                    if (insertRes.Count() > 0)
                    {
                        batchSucess += 1;

                        foreach (var item in insertRes)
                        {

                            var cbs = await _repoCbsTransaction.GetAsync(c => c.ItbId == item.ItbId);

                            AccountValParam DRAccountValidation = new AccountValParam();

                            DRAccountValidation.AcctNo = cbs.DrAcctNo;
                            DRAccountValidation.AcctType = cbs.DrAcctType;
                            DRAccountValidation.CrncyCode = cbs.CcyCode;
                            DRAccountValidation.Username = p.LoginUserName;

                            var DrAcctDetails = await _AccountValidationImplementation.ValidateAccountCall(DRAccountValidation);
                            if (DrAcctDetails != null)
                            {
                                cbs.DrAcctBranchCode = DrAcctDetails.nBranch;
                                cbs.DrAcctName = DrAcctDetails.sName;
                                cbs.DrAcctBalance = DrAcctDetails.nBalanceDec;
                                cbs.DrAcctStatus = DrAcctDetails.sStatus;
                                cbs.DrAcctAddress = DrAcctDetails.sAddress;
                                cbs.DrAcctType = DrAcctDetails.sAccountType;
                                cbs.DrAcctClassCode = DrAcctDetails.sProductCode;
                                cbs.DrAcctChequeNo = DrAcctDetails.nLastChqNo != null ? _Formatter.ValIntergers(DrAcctDetails.nLastChqNo) : (decimal?)null;
                                cbs.DrAcctOpeningDate = DrAcctDetails.sAcctOpenDate != null ? _Formatter.ValidateDate(DrAcctDetails.sAcctOpenDate) : (DateTime?)null;
                                cbs.DrAcctIndusSector = DrAcctDetails.sSector;
                                cbs.DrAcctCustType = DrAcctDetails.sCustomerType;
                                cbs.DrAcctCustNo = DrAcctDetails.sCustomerId != null ? _Formatter.ValIntergers(DrAcctDetails.sCustomerId) : (int?)null;
                                cbs.DrAcctCashBalance = DrAcctDetails.nCashBalance != null ? _Formatter.ValIntergers(DrAcctDetails.nCashBalance) : (int?)null;
                                cbs.DrAcctCity = DrAcctDetails.sCity;
                                cbs.DrAcctIncBranch = DrAcctDetails.nBranch;
                                cbs.DrAcctValUserId = p.LoginUserId;
                                cbs.DrAcctValErrorCode = DrAcctDetails.nErrorCode;
                                cbs.DrAcctValErrorMsg = DrAcctDetails.sErrorText;
                            }


                            /*cbs.DrAcctNo					= DrAcctDetails. ;
                             ;
                            cbs.DrAcctTC					= DrAcctDetails. ;
                            cbs.DrAcctNarration				= DrAcctDetails. ;
                          ;
                            */

                            /*cbs.DrAcctChargeCode			= DrAcctDetails. ;
                             cbs.DrAcctChargeAmt				= DrAcctDetails. ;
                             cbs.DrAcctTaxAmt				= DrAcctDetails. ;
                             cbs.DrAcctChargeRate			= DrAcctDetails. ;
                             cbs.DrAcctChargeNarr			= DrAcctDetails. ;
                             cbs.DrAcctBalAfterPost			= DrAcctDetails. ;
                             cbs.CrAcctBalAfterPost			= DrAcctDetails. ;
                              cbs.CcyCode = DrAcctDetails. ;
                            cbs.TaxRate = DrAcctDetails. ;
                            cbs.Amount = DrAcctDetails. ;
                            cbs.DrAcctCbsTranId				= DrAcctDetails.sCustomerType ;
                            cbs.DrAcctCashAmt				= DrAcctDetails.ca ;
                             */







                            AccountValParam CRAccountValidation = new AccountValParam();

                            CRAccountValidation.AcctNo = cbs.CrAcctNo;
                            CRAccountValidation.AcctType = cbs.CrAcctType;
                            CRAccountValidation.CrncyCode = cbs.CcyCode;
                            CRAccountValidation.Username = p.LoginUserName;

                            var CrAcctDetails = await _AccountValidationImplementation.ValidateAccountCall(CRAccountValidation);
                            if (CrAcctDetails != null)
                            {
                                cbs.CrAcctBranchCode = CrAcctDetails.nBranch;
                                //cbs.CrAcctNo = CrAcctDetails. ;
                                cbs.CrAcctType = CrAcctDetails.sAccountType;
                                cbs.CrAcctName = CrAcctDetails.sName;
                                cbs.CrAcctBalance = CrAcctDetails.nBalanceDec;
                                cbs.CrAcctStatus = CrAcctDetails.sStatus;
                                //cbs.CrAcctTC = CrAcctDetails. ;
                                //cbs.CrAcctNarration = CrAcctDetails. ;
                                cbs.CrAcctAddress = CrAcctDetails.sAddress;
                                cbs.CrAcctProdCode = CrAcctDetails.sProductCode;
                                cbs.CrAcctChequeNo = CrAcctDetails.nLastChqNo != null ? _Formatter.ValIntergers(CrAcctDetails.nLastChqNo) : (decimal?)null;
                                //cbs.CrAcctChargeCode = CrAcctDetails. ;
                                //cbs.CrAcctChargeAmt = CrAcctDetails. ;
                                //cbs.CrAcctTaxAmt = CrAcctDetails. ;
                                //cbs.CrAcctChargeRate = CrAcctDetails. ;
                                //cbs.CrAcctChargeNarr = CrAcctDetails. ;
                                cbs.CrAcctOpeningDate = CrAcctDetails.sAcctOpenDate != null ? _Formatter.ValidateDate(CrAcctDetails.sAcctOpenDate) : (DateTime?)null;
                                cbs.CrAcctIndusSector = CrAcctDetails.sSector;
                                //cbs.CrAcctCbsTranId = CrAcctDetails. ;
                                //cbs.CrAcctCashAmt = CrAcctDetails. ;
                                cbs.CrAcctCustType = CrAcctDetails.sCustomerType;
                                cbs.CrAcctCustNo = CrAcctDetails.sCustomerId != null ? _Formatter.ValIntergers(CrAcctDetails.sCustomerId) : (int?)null;
                                cbs.CrAcctCashBalance = CrAcctDetails.nCashBalance != null ? _Formatter.ValIntergers(CrAcctDetails.nCashBalance) : (int?)null;
                                cbs.CrAcctCity = CrAcctDetails.sCity;
                                cbs.CrAcctIncBranch = CrAcctDetails.nBranch;
                                cbs.CrAcctValUserId = p.LoginUserId;
                                cbs.CrAcctValErrorCode = CrAcctDetails.nErrorCode;
                                cbs.CrAcctValErrorMsg = CrAcctDetails.sErrorText;
                            }

                            cbs.Status = UnPostedStatus;
                            cbs.SupervisorId = p.LoginUserId;
                            cbs.CbsSupervisorId = p.LoginUserName;


                            _repoCbsTransaction.Update(cbs);

                            int updatebatch = await _ApplicationDbContext.SaveChanges(p.LoginUserId);


                        }

                    }



                }

                if (batchSucess == p.ListOprInstrument.Count())
                {

                    ApiResponse.ResponseMessage = "Approved Successfully";
                    return Ok(ApiResponse);
                }
                else if (batchSucess > 0)
                {
                    ApiResponse.ResponseMessage = "Not All Approved Successfully";
                    return BadRequest(ApiResponse);
                }
                else if (batchSucess == 0)
                {
                    ApiResponse.ResponseMessage = "Non of the Transaction was Approved!";
                    return BadRequest(ApiResponse);
                }

            }
            catch (Exception ex)
            {
                var exM = ex;
                return BadRequest();
            }
            ApiResponse.StringbuilderMessage = sb.ToString();
            ApiResponse.ResponseMessage = sb.ToString();

            return BadRequest(ApiResponse);

        }

        [HttpPost("RejectInstrument")]
        public async Task<IActionResult> RejectInstrument(InstrumentDTO p)
        {
            try
            {
                string RejectStatus =  _configuration["Statuses:RejectedStatus"];

                string Ids = string.Empty,  tranIds = string.Empty, IdsRej = string.Empty;
                int getCommit = 0;
                 
                foreach(var b in p.ListRejectionReasonDTO)
                {
                        IdsRej +=  b.ItbId +",";
                }

               string RejIds = _Formatter.RemoveLast(IdsRej);

                foreach(var b in p.ListOprInstrument)
                {
                   var get = await _repoOprInstrument.GetAsync(c => c.ItbId == b.ItbId);
                   get.Rejected = "Y";
                   get.RejectedBy = p.LoginUserId;
                   get.RejectedDate = DateTime.Now;
                   get.RejectedIds = RejIds;
                   get.Status = RejectStatus;
                   get.InstrumentStatus = RejectStatus;

                    _repoOprInstrument.Update(get);
                    int update = await _ApplicationDbContext.SaveChanges(p.LoginUserId);

                    if(update > 0){
                        getCommit += 1;
                    }
                }
               
                if (getCommit == p.ListOprInstrument.Count())
                {
                    ApiResponse.ResponseCode =   0;
                    ApiResponse.sErrorText=  "Rejected Successfully!";
                        
                    return Ok(ApiResponse);
                }
        }
        catch (Exception ex)
        {
                ApiResponse.ResponseMessage =  ex == null ? ex.InnerException.Message : ex.Message;
            
                ApiResponse.ResponseCode = -99;
                return BadRequest(ApiResponse); 
        }
        
            return BadRequest(ApiResponse); 
        }

    }
}