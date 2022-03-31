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
    public class ArmortizationController : ControllerBase
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
        IRepository<OprAmortizationSchedule> _repoOprAmortizationSchedule;
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
        public ArmortizationController(IConfiguration configuration,
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
                                         IRepository<OprAmortizationSchedule> repoOprAmortizationSchedule)
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
            _repoOprAmortizationSchedule = repoOprAmortizationSchedule;
        }

        [HttpPost("GetAll")]
        public async Task<IActionResult> GetAll(ArmotizationParam AnyAuth)
        {
            try
            {
                var RoleAssign = await _RoleAssignImplementation.GetRoleAssign(AnyAuth.MenuId, AnyAuth.RoleId);
                DynamicParameters param = new DynamicParameters();
                param.Add("@pnUserId", AnyAuth.pnUserId);

                if (!string.IsNullOrEmpty(AnyAuth.pdtStartDate))
                {
                    AnyAuth.pdtStartDate = _Formatter.FormatToDateYearMonthDay(AnyAuth.pdtStartDate);

                    param.Add("@psStartDate", AnyAuth.pdtStartDate);
                }
                if (!string.IsNullOrEmpty(AnyAuth.pdtEndDate))
                {
                    AnyAuth.pdtEndDate = _Formatter.FormatToDateYearMonthDay(AnyAuth.pdtEndDate);
                    param.Add("@psEndDate", AnyAuth.pdtEndDate);
                }
                if (!string.IsNullOrEmpty(AnyAuth.psDrAcctNo))
                {
                    param.Add("@psDrAcctNo", AnyAuth.psDrAcctNo);
                }
                if (!string.IsNullOrEmpty(AnyAuth.psCrAcctNo))
                {
                    param.Add("@psCrAcctNo", AnyAuth.psCrAcctNo);
                }
                if (!string.IsNullOrEmpty(AnyAuth.psCurrencyIso))
                {
                    param.Add("@psCcyCode", AnyAuth.psCurrencyIso);
                }
                if (!string.IsNullOrEmpty(AnyAuth.TotalAmt))
                {
                    param.Add("@pnTotalAmt", Convert.ToDecimal(AnyAuth.TotalAmt));
                }
                if (!string.IsNullOrEmpty(AnyAuth.psStatus))
                {
                    param.Add("@psStatus", AnyAuth.psStatus);
                }

                param.Add("@psIsGlobalSupervisor", AnyAuth.psIsGlobalSupervisor);

                var rtn = new DapperDATAImplementation<OprAmortizationScheduleDTO>();
                var _response = await rtn.LoadData("Isp_AmortizationList", param, db);
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
        public async Task<IActionResult> Add(OprAmortizationDTO p)
        {
            try
            {
                var amrt = new OprAmortizationSchedule();

                amrt = p.OprAmortizationSchedule;

                var serviceRef = await _ComputeChargesImplementation.GenServiceRef(amrt.ServiceId);
                amrt.InstrumentRef = serviceRef.nReference;
                amrt.TransTracer = serviceRef.nReference;
                amrt.Channel = "Manual";
                amrt.DateCreated = DateTime.Now;
                amrt.Status = "InActive";
                amrt.MlyProcessed = "N";
                amrt.ServiceId = amrt.ServiceId;
                amrt.Rejected = "N";
                amrt.NewAmrt = "Y";
                amrt.UserId = p.LoginUserId;
                await _repoOprAmortizationSchedule.AddAsync(amrt);
                var retV1 = await _ApplicationDbContext.SaveChanges(p.LoginUserId);
                if (retV1 > 0)
                {
                    ApiResponse.ResponseMessage = "Amortization Saved Successfully and Forwarded for Authorization";
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

        [HttpPost("Update")]
        public async Task<IActionResult> Update(OprAmortizationDTO p)
        {
            //OprAmortizationSchedule dataCheck = null; 
            try
            {
                //dataCheck = await _repoOprAmortizationSchedule.GetAsync(x => x.ScheduleId == p.OprAmortizationSchedule.ScheduleId);

                var dataCheck = _ApplicationDbContext.OprAmortizationSchedule.FirstOrDefault(x => x.ScheduleId == p.OprAmortizationSchedule.ScheduleId);

                if (dataCheck != null)
                {
                    if (dataCheck.TotalAmount == p.OprAmortizationSchedule.TotalAmount && dataCheck.EffectiveDate == p.OprAmortizationSchedule.EffectiveDate
                     && dataCheck.Term == p.OprAmortizationSchedule.Term && dataCheck.TermPeriod == p.OprAmortizationSchedule.TermPeriod && dataCheck.ExpiryDate == p.OprAmortizationSchedule.ExpiryDate
                     && dataCheck.CurrencyCode == p.OprAmortizationSchedule.CurrencyCode && dataCheck.ProcessingDept == p.OprAmortizationSchedule.ProcessingDept
                     && dataCheck.DrAcctNo == p.OprAmortizationSchedule.DrAcctNo && dataCheck.DrAcctType == p.OprAmortizationSchedule.DrAcctType
                     && dataCheck.DrAcctNarration == p.OprAmortizationSchedule.DrAcctNarration && dataCheck.DrAcctTC == p.OprAmortizationSchedule.DrAcctTC && dataCheck.CrAcctNarration == p.OprAmortizationSchedule.CrAcctNarration
                     && dataCheck.CrAcctNo == p.OprAmortizationSchedule.CrAcctNo && dataCheck.CrAcctTC == p.OprAmortizationSchedule.CrAcctTC
                     && dataCheck.CrAcctType == p.OprAmortizationSchedule.CrAcctType && dataCheck.NoInstalmt == p.OprAmortizationSchedule.NoInstalmt
                     && dataCheck.InstlmtAmount == p.OprAmortizationSchedule.InstlmtAmount && dataCheck.InstlmtProcessed == p.OprAmortizationSchedule.InstlmtProcessed
                     && dataCheck.FirstInstlmtDate == p.OprAmortizationSchedule.FinalInstlmtDate && dataCheck.NextInstlmtDate == p.OprAmortizationSchedule.NextInstlmtDate
                     && dataCheck.FinalInstlmtDate == p.OprAmortizationSchedule.FinalInstlmtDate && dataCheck.InstlmtRem == p.OprAmortizationSchedule.InstlmtRem)
                    {
                        ApiResponse.ResponseMessage = _configuration["Message:NoUpdate"];
                        ApiResponse.ResponseCode = 2;
                        return Ok(ApiResponse);
                    }
                }

                var amrt = new OprAmortizationSchedule();

                amrt = p.OprAmortizationSchedule;
                dataCheck.TotalAmount = p.OprAmortizationSchedule.TotalAmount;
                dataCheck.EffectiveDate = p.OprAmortizationSchedule.EffectiveDate;
                dataCheck.Term = p.OprAmortizationSchedule.Term;
                dataCheck.TermPeriod = p.OprAmortizationSchedule.TermPeriod;
                dataCheck.ExpiryDate = p.OprAmortizationSchedule.ExpiryDate;
                dataCheck.CurrencyCode = p.OprAmortizationSchedule.CurrencyCode;
                dataCheck.ProcessingDept = p.OprAmortizationSchedule.ProcessingDept;
                dataCheck.DrAcctNo = p.OprAmortizationSchedule.DrAcctNo;
                dataCheck.DrAcctType = p.OprAmortizationSchedule.DrAcctType;
                dataCheck.DrAcctNarration = p.OprAmortizationSchedule.DrAcctNarration;
                dataCheck.DrAcctTC = p.OprAmortizationSchedule.DrAcctTC;
                dataCheck.CrAcctNarration = p.OprAmortizationSchedule.CrAcctNarration;
                dataCheck.CrAcctNo = p.OprAmortizationSchedule.CrAcctNo;
                dataCheck.CrAcctTC = p.OprAmortizationSchedule.CrAcctTC;
                dataCheck.CrAcctType = p.OprAmortizationSchedule.CrAcctType;
                dataCheck.NoInstalmt = p.OprAmortizationSchedule.NoInstalmt;
                dataCheck.InstlmtAmount = p.OprAmortizationSchedule.InstlmtAmount;
                dataCheck.InstlmtProcessed = p.OprAmortizationSchedule.InstlmtProcessed;
                dataCheck.FirstInstlmtDate = p.OprAmortizationSchedule.FinalInstlmtDate;
                dataCheck.NextInstlmtDate = p.OprAmortizationSchedule.NextInstlmtDate;
                dataCheck.FinalInstlmtDate = p.OprAmortizationSchedule.FinalInstlmtDate;
                dataCheck.InstlmtRem = p.OprAmortizationSchedule.InstlmtRem;

                //_repoOprAmortizationSchedule.Update(dataCheck);
                _ApplicationDbContext.OprAmortizationSchedule.Update(dataCheck);
                var retV1 = await _ApplicationDbContext.SaveChanges(p.LoginUserId);
                if (retV1 > 0)
                {
                    ApiResponse.ResponseMessage = $"Amortization with Schedule Id {dataCheck.ScheduleId} Updated Successfully!";
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

        [HttpPost("GetById")]
        public async Task<IActionResult> GetById(OprAmortizationDTO p)
        {
            try
            {
                var serviceChargeslist = new List<oprServiceCharges>();
                var instrumentDetails = await
                _repoOprAmortizationSchedule.GetAsync(c => c.ScheduleId == p.OprAmortizationSchedule.ScheduleId);


                AccountValParam AccountValParam = new AccountValParam();

                AccountValParam.AcctNo = instrumentDetails.DrAcctNo;
                AccountValParam.AcctType = instrumentDetails.DrAcctType == null ? null : instrumentDetails.DrAcctType.Trim();

                AccountValParam.CrncyCode = instrumentDetails.CurrencyCode;
                AccountValParam.Username = "System";

                var valDRInstrumentAcct = await _AccountValidationImplementation.ValidateAccountCall(AccountValParam);


                AccountValParam CrAccountValParam = new AccountValParam();

                CrAccountValParam.AcctNo = instrumentDetails.CrAcctNo;
                CrAccountValParam.AcctType = instrumentDetails.CrAcctType == null ? null : instrumentDetails.CrAcctType.Trim();

                CrAccountValParam.CrncyCode = instrumentDetails.CurrencyCode;
                CrAccountValParam.Username = "System";

                var valCRInstrumentAcct = await _AccountValidationImplementation.ValidateAccountCall(CrAccountValParam);






                if (instrumentDetails != null)
                {
                    serviceChargeslist = await _ServiceChargeImplementation.GetServiceChargesByServIdAndItbId(instrumentDetails.ServiceId, (int)instrumentDetails.ScheduleId);



                    var allUsers = await _UsersImplementation.GetAllUser(instrumentDetails.UserId, instrumentDetails.RejectedBy, 0);

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
                        valDRInstrumentAcct = valDRInstrumentAcct,
                        valCRInstrumentAcct = valCRInstrumentAcct

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
                        chargeSetUp = list

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

        [HttpPost("GetReadEndDate")]
        public IActionResult GetReadEndDate(ArmotizationParam param)
        {
            DateTime dt = new DateTime();
            int Tenur = 0;
            try
            {
                if (DateTime.TryParse(param.StartDate, out dt) && int.TryParse(param.Tenure, out Tenur))
                {
                    if (param.TimeBasis == "DAYS")
                    {
                        Tenur = Tenur * 1;
                    }
                    else if (param.TimeBasis == "WEEKS")
                    {
                        Tenur = Tenur * 7;
                    }
                    else if (param.TimeBasis == "MONTHS")
                    {
                        int Month = dt.Month;
                        if (Month == 9 || Month == 4 || Month == 6 || Month == 11)
                        {
                            Tenur = Tenur * 30;
                        }
                        else if (Month == 1 || Month == 3 || Month == 5 || Month == 7 || Month == 8 || Month == 10 || Month == 12)
                        {
                            Tenur = Tenur * 31;
                        }
                        else if (Month == 2)
                        {
                            if (DateTime.IsLeapYear(dt.Year))
                            {
                                Tenur = Tenur * 29;
                            }
                            else
                            {
                                Tenur = Tenur * 28;
                            }

                        }
                    }
                    else if (param.TimeBasis == "YEARS")
                    {

                        Tenur = Tenur * 365;

                    }

                    DateTime dte = dt.AddDays(Tenur);
                    ReturnValues.nErrorCode = 0;
                    ReturnValues.sErrorText = "Success";
                    if (param.TimeBasis == "DAYS")
                    {
                        ReturnValues.NoInstalmt = Convert.ToInt32(Math.Abs(Math.Round(dte.Subtract(dt).TotalDays, 0)));
                    }
                    if (param.TimeBasis == "MONTHS")
                    {
                        ReturnValues.NoInstalmt = Convert.ToInt32(Math.Abs(Math.Round(dte.Subtract(dt).Days / (365.25 / 12), 0)));
                        double dtf = dte.Subtract(dt).Days / (365.25 / 12);
                    }
                    if (param.TimeBasis == "YEARS")
                    {
                        ReturnValues.NoInstalmt = dte.Year - dt.Year;
                    }

                    ReturnValues.InstlmtAmount = _Formatter.FormattedAmount(Convert.ToDecimal(Math.Round(Convert.ToDouble(param.TotalAmount) / (double)ReturnValues.NoInstalmt, 2)));

                    double dtf2 = dte.Subtract(dt).TotalDays;

                    ReturnValues.FirstInstlmtDate = _Formatter.FormatTransDate(dt);

                    if (_ComputeChargesImplementation.GetNextRunDate(dt, param.TimeBasis) > dte)
                    {

                        ReturnValues.NextInstlmtDate = null;
                    }
                    else if (_Formatter.FormatTransDate(_ComputeChargesImplementation.GetNextRunDate(dt, param.TimeBasis)) == _Formatter.FormatTransDate(dte))
                    {
                        ReturnValues.NextInstlmtDate = null;
                    }
                    else
                    {
                        ReturnValues.NextInstlmtDate = _Formatter.FormatTransDate(_ComputeChargesImplementation.GetNextRunDate(dt, param.TimeBasis));

                    }

                    ReturnValues.FinalInstlmtDate = _Formatter.FormatTransDate(_ComputeChargesImplementation.GetEndRunDate(dt, param.TimeBasis, (int)ReturnValues.NoInstalmt, _Formatter.FormatTransDate(_ComputeChargesImplementation.GetNextRunDate(dt, param.TimeBasis)), Tenur));
                    ReturnValues.expiryDate = _Formatter.FormatTransDate(dt.AddDays(Tenur));

                    ApiResponse.FinalInstlmtDate = ReturnValues.FinalInstlmtDate;
                    ApiResponse.expiryDate = ReturnValues.expiryDate;
                    ApiResponse.NextInstlmtDate = ReturnValues.NextInstlmtDate;
                    ApiResponse.InstlmtAmount = ReturnValues.InstlmtAmount;
                    ApiResponse.FirstInstlmtDate = ReturnValues.FirstInstlmtDate;
                    ApiResponse.NoInstalmt = ReturnValues.NoInstalmt;
                    ApiResponse.InstlmtProcessed = 0;

                    return Ok(ApiResponse);
                }



            }
            catch (Exception ex)
            {

                var exM = ex;
            }

            return BadRequest(ApiResponse);
        }

        [HttpPost("Dismiss")]
        public async Task<IActionResult> Dismiss(OprAmortizationDTO p)
        {

            int retV1 = 0, trackDismiss = 0;

            ApiResponse rtv = new ApiResponse();
            try
            {
                string DimissedStatus = _configuration["Statuses:DimissedStatus"];


                string Ids = string.Empty, tranIds = string.Empty;

                foreach (var b in p.ListOprAmortizationScheduleDTO)
                {
                    var get = await _repoOprAmortizationSchedule.GetAsync(c => c.ScheduleId == b.ScheduleId);

                    get.Status = DimissedStatus;
                    get.DismissedBy = p.LoginUserId;
                    get.DismissedDate = DateTime.Now;

                    _repoOprAmortizationSchedule.Update(get);

                    retV1 = await _ApplicationDbContext.SaveChanges(p.LoginUserId);

                    if (retV1 > 0)
                        trackDismiss++;
                }


                if (trackDismiss == p.ListOprAmortizationScheduleDTO.Count())
                {
                    ApiResponse.ResponseMessage = "Record Dismissed Successfully!";
                    ApiResponse.ResponseCode = 0;
                    return Ok(ApiResponse);
                }

            }
            catch (Exception ex)
            {

            }
            ApiResponse.ResponseMessage = "Error Occured!";
            return BadRequest(ApiResponse);
        }

    }
}