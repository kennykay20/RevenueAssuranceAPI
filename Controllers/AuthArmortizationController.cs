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
    public class AuthArmortizationController : ControllerBase
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
        public AuthArmortizationController(IConfiguration configuration,
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
                param.Add("@psOriginBranch", AnyAuth.originBranch.ToString());
                param.Add("@pnDeptId", AnyAuth.pnDeptId);
                param.Add("@psIsGlobalSupervisor", RoleAssign.IsGlobalSupervisor);

                var rtn = new DapperDATAImplementation<OprAmortizationScheduleDTO>();
                var _response = await rtn.LoadData("Isp_ApprovedAmortizationBatchList", param, db);
                if (_response != null)
                {
                    var res = new
                    {
                        _response = _response,

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

        [HttpPost("Approve")]
        public async Task<IActionResult> Approve(OprAmortizationDTO p)
        {

            int retV1 = 0, trackDismiss = 0;

            ApiResponse rtv = new ApiResponse();
            try
            {
                string ArmStatusActive = _configuration["Statuses:ArmStatusActive"];


                string Ids = string.Empty, tranIds = string.Empty;

                foreach (var b in p.ListOprAmortizationScheduleDTO)
                {
                    var get = await _repoOprAmortizationSchedule.GetAsync(c => c.ScheduleId == b.ScheduleId);

                    get.Status = ArmStatusActive;
                    get.SupervisorId = p.LoginUserId;

                    _repoOprAmortizationSchedule.Update(get);

                    retV1 = await _ApplicationDbContext.SaveChanges(p.LoginUserId);

                    if (retV1 > 0)
                        trackDismiss++;
                }


                if (trackDismiss == p.ListOprAmortizationScheduleDTO.Count())
                {
                    ApiResponse.ResponseMessage = "Instrument(s) Authorized Successfully";
                    ApiResponse.ResponseCode = 0;
                    return Ok(ApiResponse);
                }

            }
            catch (Exception ex)
            {

            }
            ApiResponse.ResponseMessage = "Items(s) is Not Processed, Hence Cannot be Authorized";
            return BadRequest(ApiResponse);
        }

        [HttpPost("RejectTrans")]
        public async Task<IActionResult> RejectTrans(OprAmortizationDTO p)
        {
            try
            {
                string RejectedStatus = _configuration["Statuses:RejectedStatus"];

                string Ids = string.Empty, tranIds = string.Empty, IdsRej = string.Empty;

                int retV1, trackDismiss = 0;

                tranIds = _Formatter.RemoveLast(Ids);


                foreach (var b in p.ListRejectionReasonDTO)
                {
                    IdsRej += b.ItbId + ",";
                }

                string RejIds = _Formatter.RemoveLast(IdsRej);

                foreach (var b in p.ListOprAmortizationScheduleDTO)
                {
                    var get = await _repoOprAmortizationSchedule.GetAsync(c => c.ScheduleId == b.ScheduleId);

                    get.Status = RejectedStatus;
                    get.SupervisorId = p.LoginUserId;

                    _repoOprAmortizationSchedule.Update(get);

                    retV1 = await _ApplicationDbContext.SaveChanges(p.LoginUserId);

                    if (retV1 > 0)
                        trackDismiss++;
                }

                 if (trackDismiss == p.ListOprAmortizationScheduleDTO.Count())
                {
                    ApiResponse.ResponseCode = 0;
                    ApiResponse.ResponseMessage = "Rejected Successfully!";

                    return Ok(ApiResponse);
                }
            }
            catch (Exception ex)
            {
                ApiResponse.ResponseMessage = ex == null ? ex.InnerException.Message : ex.Message;

                ApiResponse.ResponseCode = -99;
                return BadRequest(ApiResponse);
            }
 ApiResponse.ResponseMessage = "Your Rejection was not Successful!";
            return BadRequest(ApiResponse);
        }

    }
}