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
using RevAssuranceApi.Helper;

namespace RevAssuranceApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [EnableCors("CorsPolicy")]
    //[Authorize]
    public class AccountClosureController : ControllerBase
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
        IRepository<OprAcctClosure> _repoOprAcctClosure;
        IRepository<admService> _repoadmService;

        IRepository<admCharge> _repoadmCharge;

        IRepository<admBankBranch> _repoadmBankBranch;
        AccountValidationImplementation _AccountValidationImplementation;
        ApplicationReturnMessageImplementation _ApplicationReturnMessageImplementation;
        ComputeChargesImplementation _ComputeChargesImplementation;
        UsersImplementation _UsersImplementation;
        HeaderLogin _HeaderLogin;
        RoleAssignImplementation _RoleAssignImplementation;
        LogManager _LogManager;

        IRepository<admErrorMsg> _repoadmErrorMsg;


        public AccountClosureController(
                                        IConfiguration configuration,
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
                                           IRepository<OprAcctClosure> repoOprAcctClosure,
                                           LogManager LogManager,
                                            IRepository<admErrorMsg> repoadmErrorMsg


                                          )
        {

            _configuration = configuration;
            AppSettingsPath = new AppSettingsPath(_configuration);
            TokenGenerator = new TokenGenerator(_configuration);
            db = new SqlConnection(AppSettingsPath.GetDefaultCon());
            _ApplicationDbContext = ApplicationDbContext;
            _ChargeImplementation = ChargeImplementation;
            _ServiceChargeImplementation = ServiceChargeImplementation;
            _repooprServiceCharge = repooprServiceCharge;
            _repoOprAcctClosure = repoOprAcctClosure;
            _repoadmCharge = repoadmCharge;
            _repoadmBankBranch = repoadmBankBranch;
            _AccountValidationImplementation = AccountValidationImplementation;
            _ApplicationReturnMessageImplementation = ApplicationReturnMessageImplementation;
            _ComputeChargesImplementation = ComputeChargesImplementation;
            _UsersImplementation = UsersImplementation;
            _HeaderLogin = HeaderLogin;
            _RoleAssignImplementation = RoleAssignImplementation;
            _repoadmService = repoadmService;
            _LogManager = LogManager;
            _repoadmErrorMsg = repoadmErrorMsg;

        }

        [HttpPost("GetAll")]
        public async Task<IActionResult> GetAll(ParamLoadPage AnyAuth)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();

                var RoleAssign = await _RoleAssignImplementation.GetRoleAssign(AnyAuth.MenuId, AnyAuth.RoleId);
                string IsGlobalView = "N";
                if (RoleAssign != null)
                {
                    IsGlobalView = RoleAssign.IsGlobalSupervisor == true ? "Y" : "N";
                }
                if (!string.IsNullOrEmpty(AnyAuth.pdtCurrentDate))
                {
                    param.Add("@pdtCurrentDate", _Formatter.FormatToDateYearMonthDay(AnyAuth.pdtCurrentDate));
                }
                if (!string.IsNullOrEmpty(AnyAuth.psBranchNo))
                {
                    param.Add("@psBranchNo", AnyAuth.psBranchNo);
                }
                if (!string.IsNullOrEmpty(AnyAuth.pnDeptId))
                {
                    param.Add("@pnDeptId", AnyAuth.pnDeptId);
                }
                if (!string.IsNullOrEmpty(AnyAuth.ServiceId.ToString()))
                {
                    param.Add("@ServiceId", AnyAuth.ServiceId);
                }
                param.Add("@pnGlobalView", IsGlobalView);



                var rtn = new DapperDATAImplementation<OprAcctClosureDTO>();

                var _response = await rtn.LoadData("isp_GetAccountClosure", param, db);

                if (_response != null)
                {
                    var getCharge = await _ChargeImplementation.GetChargeDetails(AnyAuth.ServiceId);

                    var admService = await _repoadmService.GetAsync(c => c.ServiceId == AnyAuth.ServiceId);

                    var res = new
                    {
                        _response = _response,
                        charge = getCharge,
                        RoleAssign = RoleAssign,
                        admService = admService
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

        [HttpPost("GetAllSearchHistory")]
        public async Task<IActionResult> GetAllSearchHistory(ParamLoadPage AnyAuth)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                // if(!string.IsNullOrEmpty(AnyAuth.UserId.ToString()))
                // {
                //     param.Add("@pnUserId", AnyAuth.UserId);
                // }


                var RoleAssign = await _RoleAssignImplementation.GetRoleAssign(AnyAuth.MenuId, AnyAuth.RoleId);
                string IsGlobalView = "N";
                if (RoleAssign != null)
                {
                    IsGlobalView = RoleAssign.IsGlobalSupervisor == true ? "Y" : "N";
                }
                if (!string.IsNullOrEmpty(AnyAuth.TransactionDate))
                {
                    param.Add("@transactionDate", _Formatter.FormatToDateYearMonthDay(AnyAuth.TransactionDate));
                }
                if (AnyAuth.ServiceId != null)
                {
                    param.Add("@ServiceId", Convert.ToInt32(AnyAuth.ServiceId));
                }

                if (!string.IsNullOrEmpty(AnyAuth.psBranchNo))
                {
                    param.Add("@psBranchNo", Convert.ToInt32(AnyAuth.psBranchNo));
                }

                if (!string.IsNullOrEmpty(AnyAuth.psStatus))
                {
                    param.Add("@psStatus", AnyAuth.psStatus);
                }
                if (!string.IsNullOrEmpty(AnyAuth.referenceNo) || !string.IsNullOrWhiteSpace(AnyAuth.referenceNo))
                {
                    param.Add("@referenceNo", AnyAuth.referenceNo);
                }
                if (!string.IsNullOrEmpty(AnyAuth.psCcyCode))
                {
                    param.Add("@psCcyCode", AnyAuth.psCcyCode);
                }

                if (!string.IsNullOrEmpty(AnyAuth.Amount))
                {
                    param.Add("@pnAmount", Convert.ToDecimal(AnyAuth.Amount));
                }

                if (!string.IsNullOrEmpty(AnyAuth.psAcctNo))
                {
                    param.Add("@AcctNo", AnyAuth.psAcctNo);
                }

                if (!string.IsNullOrEmpty(AnyAuth.AccountName))
                {
                    param.Add("@AccountName", AnyAuth.AccountName);
                }

                if (!string.IsNullOrEmpty(AnyAuth.psAcctType))
                {
                    param.Add("@AcctType", AnyAuth.psAcctType);
                }

                if (!string.IsNullOrEmpty(AnyAuth.pnDeptId))
                {
                    param.Add("@psDeptId", Convert.ToInt32(AnyAuth.pnDeptId));
                }
                // param.Add("@pnGlobalView", IsGlobalView);



                var rtn = new DapperDATAImplementation<OprAcctClosureDTO>();

                var _response = await rtn.LoadData("isp_GetAccountClosureHis", param, db);

                if (_response != null)
                {
                    var getCharge = await _ChargeImplementation.GetChargeDetails(AnyAuth.ServiceId);

                    var admService = await _repoadmService.GetAsync(c => c.ServiceId == AnyAuth.ServiceId);

                    var res = new
                    {
                        _response = _response,
                        charge = getCharge,
                        RoleAssign = RoleAssign,
                        admService = admService
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

        [HttpGet("GetAllHistory")]
        public async Task<IActionResult> GetAllHistory()
        {
            try
            {
                var rtn = new DapperDATAImplementation<OprAcctClosureDTO>();
                DynamicParameters param = new DynamicParameters();

                var _response = await rtn.LoadData("isp_GetAllAccountClosureHistory", null, db);

                if (_response != null)
                {
                    //    var getCharge = await _ChargeImplementation.GetChargeDetails();

                    //    var admService = await _repoadmService.GetAsync(c=> c.ServiceId == AnyAuth.ServiceId) ;

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
                ApiResponse.ResponseMessage = ex == null ? ex.InnerException.Message : ex.Message;

                ApiResponse.ResponseCode = -99;
                return BadRequest(ApiResponse);
            }
        }

        [HttpPost("Add")]
        public async Task<IActionResult> Add(AcctClosureDTO p)
        {
            try
            {
                string UnauthorizedStatus = _configuration["Statuses:UnauthorizedStatus"];

                OprAcctClosure _oprAcctClosure = new OprAcctClosure();

                _oprAcctClosure = p.OprAcctClosure;

                _oprAcctClosure.ServiceStatus = UnauthorizedStatus;

                _oprAcctClosure.DateCreated = DateTime.Now;
                _oprAcctClosure.UserId = p.LoginUserId;
                _oprAcctClosure.RsmId = p.RsmId;

                var serviceRef = await _ComputeChargesImplementation.GenServiceRef(p.ServiceId);
                _oprAcctClosure.ReferenceNo = serviceRef.nReference;

                await _repoOprAcctClosure.AddAsync(_oprAcctClosure);
                int rev = await _ApplicationDbContext.SaveChanges(p.LoginUserId);


                int SeqNo = 0, SaveServiceChg = 0;
                foreach (var b in p.ListoprServiceCharge)
                {
                    SeqNo += 1;
                    b.ServiceItbId = _oprAcctClosure.ItbId;
                    b.ServiceId = p.ServiceId;
                    b.Status = null;
                    b.DateCreated = DateTime.Now;
                    var nar = b.ChgNarration + " Ref: " + serviceRef.nReference;
                    b.ChgNarration = nar;
                    var taxnar = b.TaxNarration + " Ref: " + serviceRef.nReference;
                    b.TaxNarration = taxnar;
                    b.SeqNo = SeqNo;
                    var Innar = b.IncAcctNarr + " Ref: " + serviceRef.nReference;

                    b.IncAcctNarr = Innar;
                    SaveServiceChg = await _ServiceChargeImplementation.SaveServiceCharges(b, (int)p.LoginUserId);
                }

                //int ggg = Convert.ToInt16("uuu");
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


                var resApp = await _ApplicationReturnMessageImplementation.GetAppReturnMsg(20011);
                var exM = ex == null ? ex.InnerException.Message : ex.Message;
                _LogManager.SaveLog($"Error Message for Account Closure when Inserting rescord: { exM } in login controler. StackTrace: {ex.StackTrace}");

                ApiResponse.ResponseMessage = $"Error {20011}-{ resApp.ErrorText}";
                return BadRequest(ApiResponse);
            }

            return BadRequest(ApiResponse);
        }

        [HttpPost("Update")]
        public async Task<IActionResult> Update(AcctClosureDTO p)
        {
            try
            {
                int rev = -1;

                OprAcctClosure _oprAcctClosure = new OprAcctClosure();

                //var get = await _repoOprAcctClosure.GetAsync(x => x.ItbId == p.OprAcctClosure.ItbId);
                var get = _ApplicationDbContext.OprAcctClosure.FirstOrDefault(x => x.ItbId == p.OprAcctClosure.ItbId);
                if (get != null)
                {
                    if (get.ReferenceNo == p.OprAcctClosure.ReferenceNo && get.AcctNo == p.OprAcctClosure.AcctNo
                        && get.AcctName.Trim() == p.OprAcctClosure.AcctName.Trim() && get.Status == p.OprAcctClosure.Status
                        && get.AcctType == p.OprAcctClosure.AcctType && get.CcyCode == p.OprAcctClosure.CcyCode
                        && get.ProcessingDept == p.OprAcctClosure.ProcessingDept && get.AcctSic == p.OprAcctClosure.AcctSic
                        && get.RsmId == p.OprAcctClosure.RsmId && get.AvailBal == p.OprAcctClosure.AvailBal)
                    {
                        ApiResponse.ResponseCode = -99;
                        ApiResponse.ResponseMessage = "No changes was made";

                        return Ok(ApiResponse);
                    }


                    get.ItbId = p.OprAcctClosure.ItbId;
                    get.ReferenceNo = p.OprAcctClosure.ReferenceNo;
                    get.AcctNo = p.OprAcctClosure.AcctNo;
                    get.AcctName = p.OprAcctClosure.AcctName.Trim();
                    get.Status = p.OprAcctClosure.Status;
                    get.AcctType = p.OprAcctClosure.AcctType;
                    get.CcyCode = p.OprAcctClosure.CcyCode;
                    get.ProcessingDept = p.OprAcctClosure.ProcessingDept;
                    get.AcctSic = p.OprAcctClosure.AcctSic;
                    get.RsmId = p.OprAcctClosure.RsmId;
                    get.AvailBal = p.OprAcctClosure.AvailBal;


                }

                //_oprAcctClosure = p.OprAcctClosure;


                //_repoOprAcctClosure.Update(get);
                _ApplicationDbContext.OprAcctClosure.Update(get);
                rev = await _ApplicationDbContext.SaveChanges(p.LoginUserId);
                int upServiceCharge = -1;
                if (rev > 0)
                {
                    foreach (var b in p.ListoprServiceCharge)
                    {
                        _repooprServiceCharge.Update(b);
                        upServiceCharge = await _ApplicationDbContext.SaveChanges(p.UserId);

                    }
                }

                if (rev > 0 && upServiceCharge > 0)
                {
                    ApiResponse.ResponseCode = 0;
                    ApiResponse.ResponseMessage = "Record Updated Successfully";

                    return Ok(ApiResponse);

                }
            }
            catch (Exception ex)
            {
                ApiResponse.ResponseMessage = ex == null ? ex.InnerException.Message : ex.Message;

                ApiResponse.ResponseCode = -99;
                return BadRequest(ApiResponse);
            }
            ApiResponse.ResponseMessage = "Error Occured!";
            return BadRequest(ApiResponse);
        }

        [HttpPost("CalCulateCharge")]
        public async Task<IActionResult> CalCulateCharge(AcctClosureDTO p)
        {
            try
            {
                AccountValParam AccountValParam = new AccountValParam();

                AccountValParam.AcctNo = p.AcctNo;
                AccountValParam.AcctType = p.AcctType == null ? null : p.AcctType.Trim();

                AccountValParam.CrncyCode = p.CcyCode;
                AccountValParam.Username = p.LoginUserName;

                var InstrumentAcctDetails = await _AccountValidationImplementation.ValidateAccountCall(AccountValParam);

                List<RevCalChargeModel> chgList = new List<RevCalChargeModel>();

                foreach (var i in p.ListoprServiceCharge)
                {
                    AccountValParam AccountVal = new AccountValParam();

                    AccountVal.AcctType = i.ChgAcctType == null ? InstrumentAcctDetails.sAccountType : i.ChgAcctType;
                    AccountVal.AcctNo = i.ChgAcctNo == null ? p.AcctNo : i.ChgAcctNo;
                    AccountVal.CrncyCode = InstrumentAcctDetails.sCrncyIso.Trim();
                    AccountVal.Username = p.LoginUserName;

                    var ChgDetails = await _AccountValidationImplementation.ValidateAccountCall(AccountVal);

                    OperationViewModel OperationViewModel = new OperationViewModel();

                    OperationViewModel.serviceId = p.ServiceId;
                    OperationViewModel.TransAmount = p.TransAmout;
                    OperationViewModel.InstrumentAcctNo = i.ChgAcctNo == null ? p.AcctNo : i.ChgAcctNo;
                    OperationViewModel.InstrumentCcy = InstrumentAcctDetails.sCrncyIso;
                    OperationViewModel.ChargeCode = i.ChargeCode;

                    var calCharge = await _ComputeChargesImplementation.CalChargeModel(OperationViewModel,
                                                                                        ChgDetails.nBranch.ToString(), ChgDetails.sAccountType, ChgDetails.sCrncyIso);
                    calCharge.chgAcctNo = AccountVal.AcctNo;
                    calCharge.chgAcctType = AccountVal.AcctType;
                    calCharge.chgAcctName = ChgDetails.sName;
                    calCharge.chgAvailBal = ChgDetails.nBalance;
                    calCharge.chgAcctCcy = ChgDetails.sCrncyIso;
                    calCharge.chgAcctStatus = ChgDetails.sStatus;


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

                    chgList.Add(calCharge);
                }

                var chg = new
                {
                    InstrumentAcctDetails = InstrumentAcctDetails,
                    chgList = chgList
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



        [HttpPost("GetById")]
        public async Task<IActionResult> GetById(AcctClosureDTO p)
        {
            try
            {


                var serviceChargeslist = new List<oprServiceCharges>();
                var instrumentDetails = await _repoOprAcctClosure.GetAsync(c => c.ItbId == p.OprAcctClosure.ItbId);


                AccountValParam AccountValParam = new AccountValParam();

                AccountValParam.AcctNo = instrumentDetails.AcctNo;
                AccountValParam.AcctType = instrumentDetails.AcctType == null ? null : instrumentDetails.AcctType.Trim();

                AccountValParam.CrncyCode = instrumentDetails.CcyCode;
                AccountValParam.Username = p.LoginUserName;

                var valInstrumentAcct = await _AccountValidationImplementation.ValidateAccountCall(AccountValParam);





                if (instrumentDetails != null)
                {
                    serviceChargeslist = await _ServiceChargeImplementation.GetServiceChargesByServIdAndItbId(instrumentDetails.ServiceId, (int)instrumentDetails.ItbId);
                    var allUsers = await _UsersImplementation.GetAllUser(instrumentDetails.UserId, instrumentDetails.RejectedBy, instrumentDetails.DismissedBy);

                    var chargeSetUp = new List<admCharge>();

                    if (serviceChargeslist.Count() == 0)
                    {
                        chargeSetUp = await _ChargeImplementation.GetChargeDetails(instrumentDetails.ServiceId);



                        foreach (var b in chargeSetUp)
                        {
                            serviceChargeslist.Add(new oprServiceCharges
                            {
                                ServiceId = p.ServiceId,
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

                            });
                        }
                    }


                    var res = new
                    {
                        instrumentDetails = instrumentDetails,
                        serviceChargeslist = serviceChargeslist,
                        allUsers = allUsers,
                        valInstrumentAcct = valInstrumentAcct,



                    };

                    return Ok(res);

                }
                else
                {
                    var chargeSetUp = await _ChargeImplementation.GetChargeDetails(p.ServiceId);

                    var list = new List<oprServiceCharges>();


                    foreach (var b in chargeSetUp)
                    {
                        list.Add(new oprServiceCharges
                        {
                            ServiceId = p.ServiceId,
                            ServiceItbId = 0,
                            ChgAcctNo = null, //b.ChargeAcctNo,
                            ChgAcctType = null,//b.ChargeAcctType,
                            ChgAcctName = null,
                            ChgAvailBal = null,
                            ChgAcctCcy = null,// b.CurrencyIso,
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

                        });
                    }


                    var res = new
                    {
                        chargeSetUp = list,


                    };

                    return Ok(res);

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