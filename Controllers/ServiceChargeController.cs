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
using RevAssuranceApi.RevenueAssurance.Repository.Interface;
using RevAssuranceApi.Response;

namespace RevAssuranceApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [EnableCors("CorsPolicy")]
    //[Authorize]
    public class ServiceChargeController : ControllerBase
    {
        IConfiguration _configuration;
        ApiResponse ApiResponse = new ApiResponse();
        TokenGenerator TokenGenerator;
        AppSettingsPath AppSettingsPath;
        IDbConnection db = null;
        ApplicationDbContext _ApplicationDbContext;
        IRepository<oprServiceCharges> _repooprServiceCharge;
        IRepository<admStatusItem> _repoStatusItem;

        Formatter _Formatter = new Formatter();
        ChargeImplementation _ChargeImplementation;
        ServiceChargeImplementation _ServiceChargeImplementation;
        AccountValidationImplementation _AccountValidationImplementation;
        ComputeChargesImplementation _ComputeChargesImplementation;
        public ServiceChargeController(
                                        IConfiguration configuration,
                                        ApplicationDbContext ApplicationDbContext,
                                        ChargeImplementation ChargeImplementation,
                                        ServiceChargeImplementation ServiceChargeImplementation,
                                        IRepository<oprServiceCharges> repooprServiceCharge,
                                        AccountValidationImplementation AccountValidationImplementation,
                                        ComputeChargesImplementation ComputeChargesImplementation,
                                        IRepository<admStatusItem> repoStatusItem)
        {
            _configuration = configuration;
            AppSettingsPath = new AppSettingsPath(_configuration);
            TokenGenerator = new TokenGenerator(_configuration);
            db = new SqlConnection(AppSettingsPath.GetDefaultCon());
            _ApplicationDbContext = ApplicationDbContext;
            _repoStatusItem = repoStatusItem;
            _ChargeImplementation = ChargeImplementation;
            _ServiceChargeImplementation = ServiceChargeImplementation;
            _repooprServiceCharge = repooprServiceCharge;
            _AccountValidationImplementation = AccountValidationImplementation;
            _ComputeChargesImplementation = ComputeChargesImplementation;
        }



        [HttpPost("GetAll")]
        public async Task<IActionResult> GetAll(ServiceChargeDTO AnyAuth)
        {
            try
            {
                var _response = await _ServiceChargeImplementation.GetServiceCharges(AnyAuth);

                if (_response != null)
                {

                    return Ok(_response);
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

        [HttpPost("GetAllStatus")]
        public async Task<IActionResult> GetAllStatus(statusItemDTO AnyAuth)
        {
            try
            {
                var _response = await _ServiceChargeImplementation.GetAllStatusItem();

                if (_response != null)
                {

                    return Ok(_response);
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
        public async Task<IActionResult> Add(ServiceChargeDTO param)
        {
            try
            {
                int _response = -1;
                foreach (var p in param.ListoprServiceCharge)
                {
                    p.DateCreated = DateTime.Now;
                    p.Status = _configuration["Statuses:ActiveStatus"];
                    await _repooprServiceCharge.AddAsync(p);
                    _response = await _ApplicationDbContext.SaveChanges((int)param.UserId);
                }
                if (_response > 0)
                {
                    ApiResponse.ResponseCode = 0;
                    ApiResponse.sErrorText = _configuration["Message:AddedChgSuc"];

                    return Ok(ApiResponse);
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

        [HttpPost("Update")]
        public async Task<IActionResult> Update(ServiceChargeDTO param)
        {
            try
            {
                int _response = -1;
                foreach (var p in param.ListoprServiceCharge)
                {
                    p.DateCreated = DateTime.Now;
                    p.Status = _configuration["Statuses:ActiveStatus"];
                    _repooprServiceCharge.Update(p);
                    _response = await _ApplicationDbContext.SaveChanges((int)param.UserId);
                }
                if (_response > 0)
                {
                    ApiResponse.ResponseCode = 0;
                    ApiResponse.sErrorText = _configuration["Message:AddedChgSuc"];

                    return Ok(ApiResponse);
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

        [HttpPost("CalCulateCharge")]
        public async Task<IActionResult> CalCulateCharge(InstrumentDTO p)
        {
            try
            {

                AccountValParam AccountValParam = new AccountValParam();
                if (string.IsNullOrWhiteSpace(p.AcctType))
                {
                    p.AcctType = null;
                }
                if (string.IsNullOrWhiteSpace(p.CcyCode))
                {
                    p.CcyCode = null;
                }
                AccountValParam.AcctNo = p.AcctNo;
                AccountValParam.AcctType = p.AcctType == null ? null : p.AcctType.Trim();

                AccountValParam.CrncyCode = p.CcyCode == null ? null : p.CcyCode.Trim();
                AccountValParam.Username = p.LoginUserName;

                var InstrumentAcctDetails = await _AccountValidationImplementation.ValidateAccountCall(AccountValParam);


                InstrumentAcctDetails.sRsmId = InstrumentAcctDetails.sCustomerId != null ? Convert.ToInt32(InstrumentAcctDetails.sCustomerId) : 0;
                if (InstrumentAcctDetails != null)
                {
                    var ValidateCheNoDetails = await _AccountValidationImplementation.ValidateLastChqNo(AccountValParam);
                    InstrumentAcctDetails.nLastChqNo = ValidateCheNoDetails.nLastChqNo;

                }

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

                    decimal amt = p.TransAmout != null ? _Formatter.ValDecimal(p.TransAmout) : 0;
                    OperationViewModel.serviceId = p.ServiceId;
                    OperationViewModel.TransAmount = amt.ToString();
                    OperationViewModel.InstrumentAcctNo = i.ChgAcctNo == null ? p.AcctNo : i.ChgAcctNo;
                    OperationViewModel.InstrumentCcy = InstrumentAcctDetails.sCrncyIso;
                    OperationViewModel.ChargeCode = i.ChargeCode;

                    OperationViewModel.pnCalcAmt = p.pnCalcAmt == "0" ? null : p.pnCalcAmt;
                    if (!string.IsNullOrWhiteSpace(OperationViewModel.pnCalcAmt))
                    {
                        decimal calAmt = _Formatter.ValDecimal(p.pnCalcAmt);
                        OperationViewModel.pnCalcAmt = calAmt.ToString();
                    }

                    int? TempId = i.TemplateId != null ? _Formatter.ValIntergers(i.TemplateId.ToString()) : 0;

                    OperationViewModel.TempTypeId = TempId;// == 0 ? null : TempId;

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


        [HttpPost("ValidateAccountCall")]
        public async Task<IActionResult> ValidateAccountCall(InstrumentDTO p)
        {
            try
            {


                AccountValParam AccountValParam = new AccountValParam();
                if (string.IsNullOrWhiteSpace(p.AcctType))
                {
                    p.AcctType = null;
                }
                if (string.IsNullOrWhiteSpace(p.CcyCode))
                {
                    p.CcyCode = null;
                }
                AccountValParam.AcctNo = p.AcctNo;
                AccountValParam.AcctType = p.AcctType == null ? null : p.AcctType.Trim();

                AccountValParam.CrncyCode = p.CcyCode == null ? null : p.CcyCode.Trim();
                AccountValParam.Username = p.LoginUserName;

                var InstrumentAcctDetails = await _AccountValidationImplementation.ValidateAccountCall(AccountValParam);


                InstrumentAcctDetails.sRsmId = InstrumentAcctDetails.sCustomerId != null ? Convert.ToInt32(InstrumentAcctDetails.sCustomerId) : 0;
                if (InstrumentAcctDetails != null)
                {
                    var ValidateCheNoDetails = await _AccountValidationImplementation.ValidateLastChqNo(AccountValParam);
                    InstrumentAcctDetails.nLastChqNo = ValidateCheNoDetails.nLastChqNo;

                }



                var accountData = new
                {
                    InstrumentAcctDetails = InstrumentAcctDetails
                };

                return Ok(accountData);

            }
            catch (Exception ex)
            {
                ApiResponse.ResponseMessage = ex == null ? ex.InnerException.Message : ex.Message;

                ApiResponse.ResponseCode = -99;
                return BadRequest(ApiResponse);
            }
        }


        [HttpPost("CalCulateChargeOverDraft")]
        public async Task<IActionResult> CalCulateChargeOverDraft(InstrumentDTO p)
        {
            try
            {


                AccountValParam AccountValParam = new AccountValParam();
                if (string.IsNullOrWhiteSpace(p.AcctType))
                {
                    p.AcctType = null;
                }
                if (string.IsNullOrWhiteSpace(p.CcyCode))
                {
                    p.CcyCode = null;
                }
                AccountValParam.AcctNo = p.AcctNo;
                AccountValParam.AcctType = p.AcctType == null ? null : p.AcctType.Trim();

                AccountValParam.CrncyCode = p.CcyCode;
                AccountValParam.Username = p.LoginUserName;

                var InstrumentAcctDetails = await _AccountValidationImplementation.ValidateAccountCallOverDraft(AccountValParam);



                List<RevCalChargeModel> chgList = new List<RevCalChargeModel>();

                foreach (var i in p.ListoprServiceCharge)
                {
                    AccountValParam AccountVal = new AccountValParam();

                    AccountVal.AcctType = i.ChgAcctType == null ? (InstrumentAcctDetails.sAccountType == null ? p.AcctType.Trim() : InstrumentAcctDetails.sAccountType) : i.ChgAcctType;
                    AccountVal.AcctNo = i.ChgAcctNo == null ? p.AcctNo : i.ChgAcctNo;
                    AccountVal.CrncyCode = InstrumentAcctDetails.sCrncyIso == null ? p.CcyCode : InstrumentAcctDetails.sCrncyIso.Trim();
                    AccountVal.Username = p.LoginUserName;

                    var ChgDetails = await _AccountValidationImplementation.ValidateAccountCall(AccountVal);

                    OperationViewModel OperationViewModel = new OperationViewModel();

                    decimal amt = p.TransAmout != null ? _Formatter.ValDecimal(p.TransAmout) : 0;
                    OperationViewModel.serviceId = p.ServiceId;
                    OperationViewModel.TransAmount = amt.ToString();
                    OperationViewModel.InstrumentAcctNo = i.ChgAcctNo == null ? p.AcctNo : i.ChgAcctNo;
                    OperationViewModel.InstrumentCcy = InstrumentAcctDetails.sCrncyIso;
                    OperationViewModel.ChargeCode = i.ChargeCode;

                    OperationViewModel.pnCalcAmt = p.pnCalcAmt == "0" ? null : p.pnCalcAmt;
                    if (!string.IsNullOrWhiteSpace(OperationViewModel.pnCalcAmt))
                    {
                        decimal calAmt = _Formatter.ValDecimal(p.pnCalcAmt);
                        OperationViewModel.pnCalcAmt = calAmt.ToString();
                    }

                    int? TempId = i.TemplateId != null ? _Formatter.ValIntergers(i.TemplateId.ToString()) : 0;

                    OperationViewModel.TempTypeId = TempId;// == 0 ? null : TempId;

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

        [HttpPost("AccountNoVal")]
        public async Task<IActionResult> AccountNoVal(InstrumentDTO p)
        {
            try
            {


                AccountValParam AccountValParam = new AccountValParam();
                if (string.IsNullOrWhiteSpace(p.AcctType))
                {
                    p.AcctType = null;
                }
                if (string.IsNullOrWhiteSpace(p.CcyCode))
                {
                    p.CcyCode = null;
                }
                AccountValParam.AcctNo = p.AcctNo;
                AccountValParam.AcctType = p.AcctType == null ? null : p.AcctType.Trim();

                AccountValParam.CrncyCode = p.CcyCode;
                AccountValParam.Username = p.LoginUserName;

                var InstrumentAcctDetails = await _AccountValidationImplementation.ValidateAccountCall(AccountValParam);


                InstrumentAcctDetails.sRsmId = InstrumentAcctDetails.sCustomerId != null ? Convert.ToInt32(InstrumentAcctDetails.sCustomerId) : 0;


                var chg = new
                {
                    InstrumentAcctDetails = InstrumentAcctDetails,

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


        [HttpPost("CalCulateChargeAmmendReprint")]
        public async Task<IActionResult> CalCulateChargeAmmendReprint(InstrumentDTO p)
        {
            try
            {
                AccountValParam AccountValParam = new AccountValParam();
                if (string.IsNullOrWhiteSpace(p.AcctType))
                {
                    p.AcctType = null;
                }
                if (string.IsNullOrWhiteSpace(p.CcyCode))
                {
                    p.CcyCode = null;
                }
                AccountValParam.AcctNo = p.AcctNo;
                AccountValParam.AcctType = p.AcctType == null ? null : p.AcctType.Trim();

                AccountValParam.CrncyCode = p.CcyCode;
                AccountValParam.Username = p.LoginUserName;

                var InstrumentAcctDetails = await _AccountValidationImplementation.ValidateAccountCall(AccountValParam);


                InstrumentAcctDetails.sRsmId = InstrumentAcctDetails.sCustomerId != null ? Convert.ToInt32(InstrumentAcctDetails.sCustomerId) : 0;

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

                    decimal amt = 0;// zero should be past According MR COllins July 29,3030  p.TransAmout != null ? _Formatter.ValDecimal(p.TransAmout) : 0;
                    OperationViewModel.serviceId = p.ServiceId;
                    OperationViewModel.TransAmount = amt.ToString();
                    OperationViewModel.InstrumentAcctNo = i.ChgAcctNo == null ? p.AcctNo : i.ChgAcctNo;
                    OperationViewModel.InstrumentCcy = InstrumentAcctDetails.sCrncyIso;
                    OperationViewModel.ChargeCode = i.ChargeCode;

                    int? TempId = i.TemplateId != null ? _Formatter.ValIntergers(i.TemplateId.ToString()) : 0;

                    OperationViewModel.TempTypeId = TempId;// == 0 ? null : TempId;

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


        [HttpPost("ActtVal")]
        public async Task<IActionResult> ActtVal(AccountValParam AccountValParam)
        {
            try
            {

                AccountValidationDTO values = new AccountValidationDTO();

                if (string.IsNullOrWhiteSpace(AccountValParam.AcctType))
                {
                    AccountValParam.AcctType = null;
                }

                //if(string.IsNullOrWhiteSpace(AccountValParam.CcyCode)){
                //AccountValParam.CcyCode = null;
                //}

                //AccountValParam.AcctNo = p.AcctNo;
                //AccountValParam.AcctType = p.AcctType == null ? null : p.AcctType.Trim();

                // AccountValParam.CrncyCode = p.CcyCode;
                // AccountValParam.Username = AccountValParam.LoginUserName;

                values = await _AccountValidationImplementation.ValidateAccountCall(AccountValParam);

                values.AcctType = values.sAccountType;
                values.AcctStatus = values.sStatus;
                values.AcctName = values.sName;
                values.AvailBal = values.nBalance;
                values.AcctType = values.sAccountType;
                values.CcyCode = values.sCrncyIso;
                values.AcctCCy = values.sCrncyIso;
                values.AcctNo = AccountValParam.AcctNo;
                return Ok(values);
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