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
using RevAssuranceApi.Helper;

namespace RevAssuranceApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [EnableCors("CorsPolicy")]
    //[Authorize]
    public class InstrumentController : ControllerBase
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
        IRepository<OprInstrument> _repoOprInstrument;
        IRepository<oprCollateral> _repooprCollateral;
        IRepository<admService> _repoadmService;

        IRepository<admCharge> _repoadmCharge;

        IRepository<admBankBranch> _repoadmBankBranch;
        AccountValidationImplementation _AccountValidationImplementation;
        ApplicationReturnMessageImplementation _ApplicationReturnMessageImplementation;
        ComputeChargesImplementation _ComputeChargesImplementation;
        UsersImplementation _UsersImplementation;
        HeaderLogin _HeaderLogin;
        RoleAssignImplementation _RoleAssignImplementation;
        IRepository<admDepartment> _repoadmDepartment;
        IRepository<admTemplate> _repoadmTemplate;
        IRepository<oprInstrmentTemp> _repooprInstrmentTem;
        IRepository<OprAmmendAndReprint> _repoOprAmmendAndReprint;
        AmmendReprintReasonImplementation _AmmendReprintReasonImplementation;
        IRepository<OprAmortizationSchedule> _repoOprAmortizationSchedule;
        IRepository<CbsTransaction> _repoCbsTransaction;
        IRepository<admClientProfile> _repoadmClientProfile;
        CBSTransImplementation _CBSTransImplementation;
        static CbsTransaction staticCbsTransaction;
        IConfiguration _IConfiguration;
        ImagesPathSettings _ImagesPathSettings;
        public InstrumentController(
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
                                       IRepository<oprCollateral> repooprCollateral,
                                       IRepository<admService> repoadmService,
                                       IRepository<admDepartment> repoadmDepartment,
                                        IRepository<admTemplate> repoadmTemplate,
                                         IRepository<oprInstrmentTemp> repooprInstrmentTem,
                                         IRepository<OprAmmendAndReprint> repoOprAmmendAndReprint,
                                         AmmendReprintReasonImplementation AmmendReprintReasonImplementation,
                                         IRepository<OprAmortizationSchedule> repoOprAmortizationSchedule,
                                         IRepository<CbsTransaction> repoCbsTransaction,
                                         IRepository<admClientProfile> repoadmClientProfile,
                                         CBSTransImplementation CBSTransImplementation,
                                         IConfiguration IConfiguration, ImagesPathSettings ImagesPathSettings
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
            _repoOprInstrument = repoOprInstrument;
            _repoadmCharge = repoadmCharge;
            _repoadmBankBranch = repoadmBankBranch;
            _AccountValidationImplementation = AccountValidationImplementation;
            _ApplicationReturnMessageImplementation = ApplicationReturnMessageImplementation;
            _ComputeChargesImplementation = ComputeChargesImplementation;
            _UsersImplementation = UsersImplementation;
            _HeaderLogin = HeaderLogin;
            _RoleAssignImplementation = RoleAssignImplementation;
            _repooprCollateral = repooprCollateral;
            _repoadmService = repoadmService;
            _repoadmDepartment = repoadmDepartment;
            _repoadmTemplate = repoadmTemplate;
            _repooprInstrmentTem = repooprInstrmentTem;
            _repoOprAmmendAndReprint = repoOprAmmendAndReprint;
            _AmmendReprintReasonImplementation = AmmendReprintReasonImplementation;
            _repoOprAmortizationSchedule = repoOprAmortizationSchedule;
            _repoCbsTransaction = repoCbsTransaction;
            _repoadmClientProfile = repoadmClientProfile;
            _CBSTransImplementation = CBSTransImplementation;
            _IConfiguration = IConfiguration;
            _ImagesPathSettings = ImagesPathSettings;

        }

        [HttpPost("GetAll")]
        public async Task<IActionResult> GetAll(InstrumentDTO AnyAuth)
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

                param.Add("@pdtCurrentDate", _Formatter.FormatToDateYearMonthDay(AnyAuth.pdtCurrentDate));
                param.Add("@psBranchNo", AnyAuth.psBranchNo);
                param.Add("@pnDeptId", AnyAuth.pnDeptId);
                param.Add("@pnGlobalView", IsGlobalView);
                param.Add("@ServiceId", AnyAuth.ServiceId);

                var rtn = new DapperDATAImplementation<OprInstrumentDTO>();

                var _response = await rtn.LoadData("isp_GetInstrument", param, db);

                if (_response != null)
                {
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

                    var admService = await _repoadmService.GetAsync(c => c.ServiceId == AnyAuth.ServiceId);


                    var res = new
                    {
                        _response = _response,
                        charge = listoprServiceCharges,
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


        [HttpPost("SearchAmortization")]
        public async Task<IActionResult> SearchAmortization(ArmotizationParam AnyAuth)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();

                if (AnyAuth.EffectiveDate != null)
                {
                    AnyAuth.EffectiveDate = _Formatter.ValidateDateReturnString(AnyAuth.EffectiveDate);
                    param.Add("@pdtEffectiveDate", AnyAuth.EffectiveDate);
                }

                if (AnyAuth.ExpiryDate != null)
                {
                    AnyAuth.ExpiryDate = _Formatter.ValidateDateReturnString(AnyAuth.ExpiryDate);
                    param.Add("@pdtExpiryDate", AnyAuth.ExpiryDate);
                }
                if (AnyAuth.TotalAmount != null)
                {
                    param.Add("@psTotalAmount", AnyAuth.TotalAmount);
                }
                if (AnyAuth.pnServiceId != null)
                {
                    param.Add("@pnServiceId", AnyAuth.pnServiceId);
                }
                if (AnyAuth.psCurrencyIso != null)
                {
                    param.Add("@psCcy", AnyAuth.psCurrencyIso);
                }
                if (AnyAuth.psDrAcctNo != null)
                {
                    param.Add("@psDrAcctNo", AnyAuth.psDrAcctNo);
                }
                if (AnyAuth.psCrAcctNo != null)
                {
                    param.Add("@psCrAcctNo", AnyAuth.psCrAcctNo);
                }
                if (AnyAuth.psTranRef != null)
                {
                    param.Add("@psRefNo", AnyAuth.psTranRef);
                }

                var rtn = new DapperDATAImplementation<OprAmortizationScheduleDTO>();

                var _response = await rtn.LoadData("Isp_InstrumentRetirmentList", param, db);

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

        [HttpPost("GetAllHistory")]
        public async Task<IActionResult> GetAllHistory(InstrumentDTO AnyAuth)
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

                param.Add("@pdtCurrentDate", _Formatter.FormatToDateYearMonthDay(AnyAuth.pdtCurrentDate));
                param.Add("@psBranchNo", AnyAuth.psBranchNo);
                param.Add("@pnDeptId", AnyAuth.pnDeptId);
                param.Add("@pnGlobalView", IsGlobalView);
                param.Add("@ServiceId", AnyAuth.ServiceId);

                var rtn = new DapperDATAImplementation<OprInstrumentDTO>();

                var _response = await rtn.LoadData("isp_GetInstrumentHistory", param, db);

                if (_response != null)
                {
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

                    var admService = await _repoadmService.GetAsync(c => c.ServiceId == AnyAuth.ServiceId);


                    var res = new
                    {
                        _response = _response,
                        charge = listoprServiceCharges,
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
        public async Task<IActionResult> GetAllSearchHistory(InstrumentDTO AnyAuth)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                // if(!string.IsNullOrEmpty(AnyAuth.UserId.ToString()))
                // {
                //     param.Add("@pnUserId", AnyAuth.UserId);
                // }
                
                var RoleAssign = await _RoleAssignImplementation.GetRoleAssign(AnyAuth.MenuId,AnyAuth.RoleId);
                string IsGlobalView = "N";
                if(RoleAssign != null)
                {
                    IsGlobalView = RoleAssign.IsGlobalSupervisor == true ?  "Y" : "N"; 
                }
                if(!string.IsNullOrEmpty(AnyAuth.TransactionDate))
                {
                    param.Add("@transactionDate", _Formatter.FormatToDateYearMonthDay(AnyAuth.TransactionDate));
                }
               
                if(!string.IsNullOrEmpty(AnyAuth.psStatus))
                {
                    param.Add("@psStatus", AnyAuth.psStatus);
                }
                if(!string.IsNullOrEmpty(AnyAuth.referenceNo) || !string.IsNullOrWhiteSpace(AnyAuth.referenceNo))
                {
                    param.Add("@referenceNo", AnyAuth.referenceNo);
                }
                if(!string.IsNullOrEmpty(AnyAuth.CcyCode))
                {
                    param.Add("@psCcyCode", AnyAuth.CcyCode);
                }
                if(!string.IsNullOrEmpty(AnyAuth.AccountName))
                {
                    param.Add("@AccountName", AnyAuth.AccountName);
                }
                if(!string.IsNullOrEmpty(AnyAuth.AcctNo))
                {
                    param.Add("@AcctNo", AnyAuth.AcctNo);
                }
                if(!string.IsNullOrEmpty(AnyAuth.AcctType))
                {
                    param.Add("@AcctType", AnyAuth.AcctType);
                }
                
                if(!string.IsNullOrEmpty(AnyAuth.Amount))
                {
                    param.Add("@pnAmount", Convert.ToDecimal(AnyAuth.Amount));
                }
                if(!string.IsNullOrEmpty(AnyAuth.ServiceId.ToString()))
                {
                    param.Add("@ServiceId", Convert.ToInt32(AnyAuth.ServiceId));
                }
                if(!string.IsNullOrEmpty(AnyAuth.pnDeptId)){
                    param.Add("@psDeptId", Convert.ToInt32(AnyAuth.pnDeptId));
                }
                if(!string.IsNullOrEmpty(AnyAuth.psBranchNo))
                {
                    param.Add("@psBranchNo", Convert.ToInt32(AnyAuth.psBranchNo));
                }
                // param.Add("@pnGlobalView", IsGlobalView);
                
                
              
                var rtn = new DapperDATAImplementation<OprInstrumentDTO>();
                
                var _response = await rtn.LoadData("Isp_GetInstrumentSearchHis", param, db);

                if(_response != null)
                {
                   var getCharge = await _ChargeImplementation.GetChargeDetails(AnyAuth.ServiceId);

                   var admService = await _repoadmService.GetAsync(c=> c.ServiceId == AnyAuth.ServiceId) ;
                 
                   var res = new {
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
                 ApiResponse.ResponseMessage =  ex == null ? ex.InnerException.Message : ex.Message;
                
                 ApiResponse.ResponseCode = -99;
                 return BadRequest(ApiResponse); 
            }

           
        }


        [HttpPost("Add")]
        public async Task<IActionResult> Add(InstrumentDTO p)
        {
            try
            {
                OprInstrument OprInstrument = new OprInstrument();

                OprInstrument = p.OprInstrument;

                string UnauthorizedStatus = _configuration["Statuses:UnauthorizedStatus"];
                OprInstrument.InstrumentStatus = UnauthorizedStatus;
                OprInstrument.DateCreated = DateTime.Now;
                OprInstrument.TransactionDate = Convert.ToDateTime(p.TransactionDate);
                OprInstrument.ValueDate = Convert.ToDateTime(p.ValueDate);

                var serviceRef = await _ComputeChargesImplementation.GenServiceRef(p.ServiceId);
                OprInstrument.ReferenceNo = serviceRef.nReference;
                var narCont = OprInstrument.ContCrAcctNarration + " Ref: " +
                 serviceRef.nReference;

                var getConNar = await _repoadmService.GetAsync(c => c.ServiceId == p.ServiceId);
                if (getConNar != null)
                {
                    if (getConNar.ContCrAcctNarr != null)
                    {
                        OprInstrument.ContCrAcctNarration = getConNar.ContCrAcctNarr
                                                           .Replace("{RefPrefix}", getConNar.RefPrefix)
                                                           .Replace("{BeneficiaryName}", OprInstrument.Beneficiary)
                                                           .Replace("DrAcctName}", OprInstrument.ContDrAcctName)
                                                           .Replace("{ServiceReference}", serviceRef.nReference);
                    }

                    if (getConNar.ContDrAcctNarr != null)
                    {
                        OprInstrument.ContDrAcctNarration = getConNar.ContDrAcctNarr
                                                           .Replace("{RefPrefix}", getConNar.RefPrefix)
                                                           .Replace("{BeneficiaryName}", OprInstrument.Beneficiary)
                                                           .Replace("DrAcctName}", OprInstrument.ContDrAcctName)
                                                           .Replace("{ServiceReference}", serviceRef.nReference);
                    }
                }


                await _repoOprInstrument.AddAsync(OprInstrument);
                int rev = await _ApplicationDbContext.SaveChanges(p.LoginUserId);
                int SaveServiceChg = 0;
                int updateInstWithTempId = 0;

                oprInstrmentTemp tem = new oprInstrmentTemp();


                tem.ServiceId = p.ServiceId;
                tem.ServiceItbId = OprInstrument.ItbId;
                tem.UserId = (int)p.LoginUserId;
                tem.DateCreated = DateTime.Now;
                tem.TemplateContent = p.TemplateContent;

                await _repooprInstrmentTem.AddAsync(tem);

                var SaveTem = await _ApplicationDbContext.SaveChanges(p.LoginUserId);
                if (SaveTem > 0)
                {
                    OprInstrument.TemplateContentIds = tem.ItbId.ToString();
                    _repoOprInstrument.Update(OprInstrument);
                    updateInstWithTempId = await _ApplicationDbContext.SaveChanges(p.LoginUserId);
                }


                if (rev > 0)
                {
                    int SeqNo = 0;
                    foreach (var b in p.ListoprServiceCharge)
                    {
                        SeqNo += 1;
                        b.ServiceItbId = OprInstrument.ItbId;
                        b.ServiceId = p.ServiceId;
                        b.Status = null;
                        b.DateCreated = DateTime.Now;
                        var nar = b.ChgNarration + " Ref: " + serviceRef.nReference;
                        b.ChgNarration = nar;
                        var taxnar = b.TaxNarration + " Ref: " + serviceRef.nReference;
                        b.TaxNarration = taxnar;
                        b.SeqNo = SeqNo;
                        b.TemplateId = _Formatter.ValIntergers(tem.ItbId.ToString());
                        var Innar = b.IncAcctNarr + " Ref: " + serviceRef.nReference;

                        b.IncAcctNarr = Innar;
                        SaveServiceChg = await _ServiceChargeImplementation.SaveServiceCharges(b, (int)p.LoginUserId);
                    }




                    foreach (var opCol in p.ListoprCollateral)
                    {
                        if (opCol.AcctNo != null)
                        {
                            opCol.Status = UnauthorizedStatus;
                            opCol.DateCreated = DateTime.Now;
                            opCol.UserId = (int)p.LoginUserId;
                            opCol.CollStatus = "Active";
                            opCol.ServiceId = p.ServiceId;
                            opCol.ServiceItbId = OprInstrument.ItbId;
                            opCol.ItbId = null;
                            await _repooprCollateral.AddAsync(opCol);

                            int savCol = await _ApplicationDbContext.SaveChanges(p.UserId);

                            if (opCol.PlaceHold == "Y")
                            {
                                CollateralCoreBankingDTOParam par = new CollateralCoreBankingDTOParam();

                                var getService = await _repoadmService.GetAsync(c => c.ServiceId == opCol.ServiceId);

                                string reason = "Collateral for {{ServiceType}} Ref {{Ref}}";

                                par.TransactionRef = serviceRef.nReference;
                                par.AccountType = opCol.AcctType;
                                par.AccountNo = opCol.AcctNo;
                                //par.HoldID =  string.Empty;
                                par.HoldAmt = opCol.LienAmount;
                                par.UserName = p.LoginUserName;
                                par.Modify = "N";

                                if (getService != null)
                                {
                                    if (getService.Channel != null)
                                        par.HoldReason = reason.Replace("{{ServiceType}}", getService.Channel)
                                                    .Replace("{{Ref}}", OprInstrument.ReferenceNo);
                                }


                                var getColHoldId = await _AccountValidationImplementation.CollateralHold(par);
                                opCol.HoldId = getColHoldId.HoldId;
                                _repooprCollateral.Update(opCol);
                                int savColHoldId = await _ApplicationDbContext.SaveChanges(p.UserId);
                            }



                        }
                    }
                    if (rev > 0 && SaveServiceChg > 0 && SaveTem > 0 && updateInstWithTempId > 0)
                    {
                        ApiResponse.ResponseCode = 0;
                        ApiResponse.ResponseMessage = "Processed Successfully";

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
            finally
            {

            }

            return BadRequest(ApiResponse);
        }

        [HttpPost("AmmendOrReprint")]
        public async Task<IActionResult> AmmendOrReprint(InstrumentDTO p)
        {
            try
            {
                if (p.admAmendReprintReason.Chargeable == false)
                {
                    OprAmmendAndReprint oprAmmendAndReprint = new OprAmmendAndReprint();

                    oprAmmendAndReprint.ServiceItbId = p.OprInstrument.ItbId;
                    oprAmmendAndReprint.ServiceId = p.OprInstrument.ServiceId;
                    oprAmmendAndReprint.Action = p.AmmendmentOrReprintTxt;
                    oprAmmendAndReprint.ReasonId = p.admAmendReprintReason.ReasonId;
                    oprAmmendAndReprint.RequireCharge = false;
                    oprAmmendAndReprint.UserId = p.LoginUserId;
                    oprAmmendAndReprint.DateCreated = DateTime.Now;

                    await _repoOprAmmendAndReprint.AddAsync(oprAmmendAndReprint);

                    var SaveTem = await _ApplicationDbContext.SaveChanges(p.LoginUserId);

                    if (SaveTem > 0)
                    {
                        ApiResponse.ResponseCode = 0;
                        ApiResponse.ResponseMessage = "Processed Successfully";
                        return Ok(ApiResponse);
                    }
                }
                if (p.admAmendReprintReason.Chargeable == true)
                {
                    OprInstrument OprInstrument = new OprInstrument();

                    OprInstrument = p.OprInstrument;

                    string UnauthorizedStatus = _configuration["Statuses:UnauthorizedStatus"];
                    OprInstrument.InstrumentStatus = UnauthorizedStatus;
                    OprInstrument.DateCreated = DateTime.Now;
                    OprInstrument.TransactionDate = Convert.ToDateTime(p.TransactionDate);
                    OprInstrument.ValueDate = Convert.ToDateTime(p.ValueDate);

                    var serviceRef = await _ComputeChargesImplementation.GenServiceRef(p.ServiceId);
                    OprInstrument.ReferenceNo = serviceRef.nReference;
                    var narCont = OprInstrument.ContCrAcctNarration + " Ref: " +
                     serviceRef.nReference;

                    var getConNar = await _repoadmService.GetAsync(c => c.ServiceId == p.ServiceId);
                    if (getConNar != null)
                    {
                        if (getConNar.ContCrAcctNarr != null)
                        {
                            OprInstrument.ContCrAcctNarration = getConNar.ContCrAcctNarr
                                                               .Replace("{RefPrefix}", getConNar.RefPrefix)
                                                               .Replace("{BeneficiaryName}", OprInstrument.Beneficiary)
                                                               .Replace("DrAcctName}", OprInstrument.ContDrAcctName)
                                                               .Replace("{ServiceReference}", serviceRef.nReference);
                        }

                        if (getConNar.ContDrAcctNarr != null)
                        {
                            OprInstrument.ContDrAcctNarration = getConNar.ContDrAcctNarr
                                                               .Replace("{RefPrefix}", getConNar.RefPrefix)
                                                               .Replace("{BeneficiaryName}", OprInstrument.Beneficiary)
                                                               .Replace("DrAcctName}", OprInstrument.ContDrAcctName)
                                                               .Replace("{ServiceReference}", serviceRef.nReference);
                        }
                    }

                    await _repoOprInstrument.AddAsync(OprInstrument);
                    int rev = await _ApplicationDbContext.SaveChanges(p.LoginUserId);
                    int SaveServiceChg = 0;
                    int updateInstWithTempId = 0;

                    if (rev > 0)
                    {
                        int SeqNo = 0;
                        foreach (var b in p.ListoprServiceCharge)
                        {
                            SeqNo += 1;
                            b.ServiceItbId = OprInstrument.ItbId;
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

                        oprInstrmentTemp tem = new oprInstrmentTemp();


                        tem.ServiceId = p.ServiceId;
                        tem.ServiceItbId = OprInstrument.ItbId;
                        tem.UserId = (int)p.LoginUserId;
                        tem.DateCreated = DateTime.Now;
                        tem.TemplateContent = p.TemplateContent;

                        await _repooprInstrmentTem.AddAsync(tem);

                        var SaveTem = await _ApplicationDbContext.SaveChanges(p.LoginUserId);
                        if (SaveTem > 0)
                        {
                            OprInstrument.TemplateContentIds = tem.ItbId.ToString();
                            _repoOprInstrument.Update(OprInstrument);
                            updateInstWithTempId = await _ApplicationDbContext.SaveChanges(p.LoginUserId);
                        }



                        foreach (var opCol in p.ListoprCollateral)
                        {
                            if (opCol.AcctNo != null)
                            {
                                opCol.Status = UnauthorizedStatus;
                                opCol.DateCreated = DateTime.Now;
                                opCol.UserId = (int)p.LoginUserId;
                                opCol.CollStatus = "Active";
                                opCol.ServiceId = p.ServiceId;
                                opCol.ServiceItbId = OprInstrument.ItbId;
                                opCol.ItbId = null;
                                await _repooprCollateral.AddAsync(opCol);

                                int savCol = await _ApplicationDbContext.SaveChanges(p.UserId);

                                if (opCol.PlaceHold == "Y")
                                {
                                    CollateralCoreBankingDTOParam par = new CollateralCoreBankingDTOParam();

                                    var getService = await _repoadmService.GetAsync(c => c.ServiceId == opCol.ServiceId);

                                    string reason = "Collateral for {{ServiceType}} Ref {{Ref}}";

                                    par.TransactionRef = serviceRef.nReference;
                                    par.AccountType = opCol.AcctType;
                                    par.AccountNo = opCol.AcctNo;
                                    //par.HoldID =  string.Empty;
                                    par.HoldAmt = opCol.LienAmount;
                                    par.UserName = p.LoginUserName;
                                    par.Modify = "N";

                                    if (getService != null)
                                    {
                                        if (getService.Channel != null)
                                            par.HoldReason = reason.Replace("{{ServiceType}}", getService.Channel)
                                                        .Replace("{{Ref}}", OprInstrument.ReferenceNo);
                                    }


                                    var getColHoldId = await _AccountValidationImplementation.CollateralHold(par);
                                    opCol.HoldId = getColHoldId.HoldId;
                                    _repooprCollateral.Update(opCol);
                                    int savColHoldId = await _ApplicationDbContext.SaveChanges(p.UserId);
                                }



                            }
                        }
                        if (rev > 0 && SaveServiceChg > 0 && SaveTem > 0 && updateInstWithTempId > 0)
                        {
                            ApiResponse.ResponseCode = 0;
                            ApiResponse.ResponseMessage = "Processed Successfully";

                            return Ok(ApiResponse);
                        }

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


        [HttpPost("Update")]
        public async Task<IActionResult> Update(InstrumentDTO p)
        {
            int saveOprInst = -1; int saveChg = -1; int SaveTem = -1; int savCol = -1;
            try
            {
                long itbId = p.OprInstrument.ItbId != null ? (int)p.OprInstrument.ItbId : 0;

                OprInstrument OprInstrument = new OprInstrument();
                OprInstrument = p.OprInstrument;

                if (OprInstrument != null)
                {
                    // getToken.AcctNo = p.OprInstrument.AcctNo;
                    _repoOprInstrument.Update(OprInstrument);

                    saveOprInst = await _ApplicationDbContext.SaveChanges(p.UserId);

                    foreach (var b in p.ListoprServiceCharge)
                    {

                        oprServiceCharges serviceCharges = new oprServiceCharges();
                        serviceCharges = b;

                        _repooprServiceCharge.Update(serviceCharges);

                        saveChg = await _ApplicationDbContext.SaveChanges(p.UserId);
                    }



                    var getTem = await _repooprInstrmentTem.GetAsync(c => c.ServiceId == OprInstrument.ServiceId && c.ServiceItbId == OprInstrument.ItbId);

                    if (getTem != null)
                    {
                        getTem.TemplateContent = p.TemplateContent;
                        _repooprInstrmentTem.Update(getTem);
                        SaveTem = await _ApplicationDbContext.SaveChanges(p.LoginUserId);
                    }

                    // oprCollateral oprCollateral = new oprCollateral();

                    foreach (var opCol in p.ListoprCollateral)
                    {

                        _repooprCollateral.Update(opCol);
                        savCol = await _ApplicationDbContext.SaveChanges(p.UserId);

                        if (opCol.PlaceHold == "N" && opCol.HoldId > 0)
                        {
                            CollateralCoreBankingDTOParam par = new CollateralCoreBankingDTOParam();



                            par.TransactionRef = OprInstrument.ReferenceNo;
                            par.AccountType = opCol.AcctType;
                            par.AccountNo = opCol.AcctNo;
                            par.HoldID = opCol.HoldId;
                            par.HoldAmt = opCol.LienAmount;
                            par.UserName = p.LoginUserName;
                            par.Modify = "Y";

                            var getColHoldId = await _AccountValidationImplementation.CollateralHold(par);
                            opCol.HoldId = getColHoldId.HoldId;
                            _repooprCollateral.Update(opCol);
                            int savColHoldId = await _ApplicationDbContext.SaveChanges(p.UserId);
                        }

                        if (opCol.PlaceHold == "Y")
                        {
                            CollateralCoreBankingDTOParam par = new CollateralCoreBankingDTOParam();


                            var getService = await _repoadmService.GetAsync(c => c.ServiceId == opCol.ServiceId);

                            string reason = "Collateral for {{ServiceType}} Ref {{Ref}}";

                            var serviceRef = await _ComputeChargesImplementation.GenServiceRef(p.ServiceId);


                            par.TransactionRef = serviceRef.nReference;
                            par.AccountType = opCol.AcctType;
                            par.AccountNo = opCol.AcctNo;
                            //par.HoldID =  string.Empty;
                            par.HoldAmt = opCol.LienAmount;
                            par.UserName = p.LoginUserName;
                            par.Modify = "N";
                            if (getService != null)
                            {
                                if (getService.Channel != null)
                                    par.HoldReason = reason.Replace("{{ServiceType}}", getService.Channel)
                                                .Replace("{{Ref}}", OprInstrument.ReferenceNo);
                            }


                            var getColHoldId = await _AccountValidationImplementation.CollateralHold(par);

                            opCol.HoldId = getColHoldId.HoldId;
                            _repooprCollateral.Update(opCol);
                            int savColHoldId = await _ApplicationDbContext.SaveChanges(p.UserId);
                        }

                    }

                    if (saveOprInst > 0 && saveChg > 0 && SaveTem > 0 && savCol > 0)
                    {
                        ApiResponse.ResponseCode = 0;
                        ApiResponse.ResponseMessage = "Request Updated Successfully";

                        var res = new
                        {
                            ApiResponse = ApiResponse,
                            //  oprCollateral = opCol
                        };
                        return Ok(res);



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

        [HttpPost("GetEndDate121")]
        public async Task<IActionResult> GetEndDate121(EndDateDTO p)
        {

            string expiryDate = string.Empty;
            DateTime dt = new DateTime();
            int Tenur = 0;
            try
            {
                string gg = p.TenorPeriod;
                if (DateTime.TryParse(p.EffectiveDate.ToString(), out dt) && int.TryParse(p.TenorPeriod, out Tenur))
                {
                    if (p.TimeBasis == "DAY(S)")
                    {
                        Tenur = Tenur * 1;
                    }
                    else if (p.TimeBasis == "WEEKS")
                    {
                        Tenur = Tenur * 7;
                    }
                    else if (p.TimeBasis == "MONTH(S)")
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
                    else if (p.TimeBasis == "YEAR(S)")
                    {

                        Tenur = Tenur * 365;

                    }

                    //  string hhh = string.Format("{0:dd-MMM-yy }", dt);
                    DateTime dt1 = dt.AddDays(Tenur);
                    expiryDate = string.Format("{0:dd-MMM-yyyy }", dt1);
                    var res = new
                    {
                        expiryDate = expiryDate
                    };
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                ApiResponse.ResponseMessage = ex == null ? ex.InnerException.Message : ex.Message;
                return BadRequest(ApiResponse);
            }

            return BadRequest(ApiResponse);
        }

        [HttpPost("GetEndDate")]
        public async Task<IActionResult> GetEndDate(EndDateDTO p)
        {
            string expiryDate = string.Empty;

            try
            {
                DateTime EffectiveDate = _Formatter.ValidateDate(p.EffectiveDate.ToString());

                string effDateString = _Formatter.FormatDateCurrProcessing(EffectiveDate);

                string script = string.Empty;

                if (p.TimeBasis == "DAY(S)")
                {
                    script = $"select dateadd(dd,{p.TenorPeriod},dateadd(dd,-1,'{effDateString}')) DateCreated";

                }
                else if (p.TimeBasis == "WEEKS")
                {

                }
                else if (p.TimeBasis == "MONTH(S)")
                {
                    script = $"select dateadd(mm,{p.TenorPeriod},dateadd(dd,-1,'{effDateString}')) DateCreated";
                }
                else if (p.TimeBasis == "YEAR(S)")
                {
                    script = $"select dateadd(yy,{p.TenorPeriod},dateadd(dd,-1,'{effDateString}')) DateCreated";
                }

                var rtn = new DapperDATAImplementation<admBankServiceSetup>();

                var _response = await rtn.LoadListNoParam(script, db);

                expiryDate = string.Format("{0:dd-MMM-yyyy }", _response[0].DateCreated);
                var res = new
                {
                    expiryDate = expiryDate
                };
                return Ok(res);

            }
            catch (Exception ex)
            {
                ApiResponse.ResponseMessage = ex == null ? ex.InnerException.Message : ex.Message;
                return BadRequest(ApiResponse);
            }

            return BadRequest(ApiResponse);
        }




        [HttpPost("ViewTemp")]
        public async Task<IActionResult> ViewTemp(EndDateDTO p)//public string GetEndDate(string Tenure, string StartDate, string TimeBasis)
        {
            try
            {
                var template = new admTemplate();

                template = await _repoadmTemplate.GetAsync(c => c.ServiceId == p.ServiceId && c.ItbId == p.TemplateId);

                if (template == null)
                {
                    var tempexist = await _repooprInstrmentTem.GetAsync(c => c.ServiceItbId == p.ServiceItbId && c.ServiceId == p.ServiceId);
                    if (tempexist != null)
                    {
                        var templateNew = new admTemplate();
                        templateNew.TemplateContent = tempexist.TemplateContent;

                        var result = new
                        {
                            template = templateNew
                        };
                        return Ok(result);

                    }

                    template = await _repoadmTemplate.GetAsync(c => c.ServiceId == p.ServiceId);

                }

                if (p.ServiceId == 13)
                {
                    string OverDrfatImg = _ImagesPathSettings.GetImagePath(p.ServiceId);

                   // string img = "<img class=" + "imgLoader" + " src=" + "../../assets/img/template/overDraftMemo.PNG" + "/>";

                    string img2 = "<img class=" + "imgLoader" + " src=" + "{IMGDB}" + ">";
                    string img = img2.Replace("{IMGDB}", template.ImageUrl);

                    string replace = template.TemplateContent.Replace("{{IMG}}", img);
                    template.TemplateContent = replace;
                }

                var res = new
                {
                    template = template
                };
                return Ok(res
                );
            }
            catch (Exception ex)
            {
                ApiResponse.ResponseMessage = ex == null ? ex.InnerException.Message : ex.Message;
                return BadRequest(ApiResponse);
            }

            return BadRequest(ApiResponse);
        }



        //ProcessListww
        [HttpPost("ProcessList")]
        public async Task<IActionResult> ProcessList(InstrumentDTO p)
        {
            try
            {
                // int _response  = -1;

                string Ids = string.Empty, tranIds = string.Empty;
                foreach (var b in p.ListOprInstrumentDTO)
                {
                    Ids += b.ItbId + ",";
                }

                tranIds = _Formatter.RemoveLast(Ids);

                string UnauthorizedStatus = _configuration["Statuses:UnauthorizedStatus"]; ;


                var admCharge = await _repoadmCharge.GetAsync(c => c.ServiceId == p.ServiceId);
                if (admCharge != null)
                {


                    var cbs = new DapperDATAImplementation<OprInstrument>();

                    string script = $"select * from OprInstrument where itbid in({tranIds})";

                    var transactions = await cbs.LoadListNoParam(script, db);
                    int SeqNo = 0;
                    foreach (var i in transactions)
                    {
                        SeqNo += 1;
                        oprServiceCharges oprServiceCharge = new oprServiceCharges();
                        OperationViewModel OperationViewModel = new OperationViewModel();

                        //Cal Charges
                        if (i.BranchNo != null)
                        {
                            if (i.ServiceId != null)
                            {
                                OperationViewModel.serviceId = i.ServiceId;
                            }

                            OperationViewModel.TransAmount = "0";

                            int? brch = 0;
                            if (i.OriginatingBranchId != null)
                            {
                                brch = i.OriginatingBranchId; // _Formatter.ValIntergers(i.OriginatingBranchId);
                            }

                            var getBranchCode = await _repoadmBankBranch.GetAsync(c => c.BranchNo == brch);

                            OperationViewModel.InstrumentAcctNo = i.AcctNo;


                            OperationViewModel.TempType = 0;
                            OperationViewModel.InstrumentCcy = i.CcyCode;
                            OperationViewModel.ChargeCode = admCharge.ChargeCode;


                            //Charge Details
                            AccountValParam AccountValParam = new AccountValParam();

                            AccountValParam.AcctType = i.AcctType;
                            AccountValParam.AcctNo = i.AcctNo;
                            AccountValParam.CrncyCode = i.CcyCode;
                            AccountValParam.Username = p.LoginUserName;

                            var ValChg = await _AccountValidationImplementation.ValidateAccountCall(AccountValParam);

                            if (ValChg != null)
                            {
                                if (ValChg.nErrorCode != 0)
                                {
                                    int erroCode = 20010;
                                    var msg = await _ApplicationReturnMessageImplementation.GetAppReturnMsg(erroCode);

                                    ApiResponse.ResponseCode = (int)msg.ErrorId;
                                    ApiResponse.sErrorText = $"{msg.ErrorText.Replace("{ErrorCode}", erroCode.ToString())}. Validation Message: {ValChg.sErrorText}";

                                    return BadRequest(ApiResponse);


                                }

                            }

                            if (ValChg == null)
                            {
                                int erroCode = 20010;
                                var msg = await _ApplicationReturnMessageImplementation.GetAppReturnMsg(erroCode);
                                ApiResponse.ResponseCode = (int)msg.ErrorId;
                                ApiResponse.sErrorText = $"{msg.ErrorText.Replace("{ErrorCode}", erroCode.ToString())}. Validation Message: {ValChg.sErrorText}";

                                return BadRequest(ApiResponse);


                            }

                            OperationViewModel.serviceId = p.ServiceId;
                            OperationViewModel.TransAmount = "0";
                            OperationViewModel.InstrumentAcctNo = i.AcctNo;
                            OperationViewModel.InstrumentCcy = i.CcyCode;
                            OperationViewModel.ChargeCode = admCharge.ChargeCode;

                            var calCharge = await _ComputeChargesImplementation.CalChargeModel(OperationViewModel, i.BranchNo.ToString(), i.AcctType, i.CcyCode);


                            AccountValParam.AcctType = i.AcctType;
                            AccountValParam.AcctNo = i.AcctNo;
                            AccountValParam.CrncyCode = i.CcyCode;
                            AccountValParam.Username = p.LoginUserName;


                            var ValAcct = await _AccountValidationImplementation.ValidateAccountCall(AccountValParam);

                            //Acct Details
                            i.AvailBal = ValAcct.nBalanceDec;
                            i.AcctSic = ValAcct.sSector;
                            i.AcctStatus = ValAcct.sStatus;
                            i.RsmId = ValAcct.sRsmId;

                            var getServiceRef = await _ComputeChargesImplementation.GenServiceRef(p.ServiceId);

                            i.ReferenceNo = getServiceRef.nReference;



                            oprServiceCharge.ChargeRate = calCharge.nChargeRate;
                            oprServiceCharge.OrigChgAmount = calCharge.nOrigChargeAmount;
                            oprServiceCharge.EquivChgAmount = calCharge.nActualChgAmt;
                            oprServiceCharge.ExchangeRate = calCharge.nExchRate;
                            oprServiceCharge.ChargeCode = calCharge.nChargeCode;
                            oprServiceCharge.ChgAcctCcy = calCharge.sChgCurrency;
                            oprServiceCharge.TaxAmount = calCharge.nTaxAmt;
                            oprServiceCharge.ChgNarration = calCharge.sNarration;
                            oprServiceCharge.TaxNarration = calCharge.sTaxNarration;
                            oprServiceCharge.TaxRate = calCharge.nTaxRate;
                            oprServiceCharge.ChgAvailBal = ValChg.nBalanceDec;

                            oprServiceCharge.ChgAcctNo = i.AcctNo;
                            oprServiceCharge.ChgAcctType = i.AcctType;
                            oprServiceCharge.ChgAcctName = i.AcctName;
                            oprServiceCharge.ChgAcctStatus = i.AcctStatus;
                            oprServiceCharge.OrigChgCCy = calCharge.sChgCurrency;
                            oprServiceCharge.EquivChgCcy = calCharge.sChgCurrency;


                            /*Income Account Val*/

                            AccountValParam.AcctType = calCharge.sChargeIncAcctType;
                            AccountValParam.AcctNo = calCharge.sChargeIncAcctNo;
                            AccountValParam.CrncyCode = calCharge.sTransCurrency;
                            AccountValParam.Username = p.LoginUserName;
                            var valIncomeAcct = await _AccountValidationImplementation.ValidateAccountCall(AccountValParam);
                            if (valIncomeAcct != null)
                            {

                                oprServiceCharge.IncAcctNo = calCharge.sChargeIncAcctNo;
                                oprServiceCharge.IncAcctType = valIncomeAcct.sAccountType;
                                oprServiceCharge.IncAcctName = valIncomeAcct.sName;
                                oprServiceCharge.IncAcctNarr = calCharge.sNarration;
                                oprServiceCharge.IncBranch = _Formatter.ValIntergers(valIncomeAcct.nBranch);

                                oprServiceCharge.IncAcctStatus = valIncomeAcct.nErrorCode == 0 ? valIncomeAcct.sStatus : valIncomeAcct.sErrorText;
                                oprServiceCharge.IncAcctBalance = valIncomeAcct.nBalance;
                            }

                            i.ProcessingDeptId = p.DeptId;
                            i.InstrumentStatus = UnauthorizedStatus;
                            i.UserId = p.UserId;
                            i.OrigDeptId = p.DeptId;

                            //Val Tax  Details

                            string taxAcct = string.Empty;
                            if (getBranchCode != null)
                            {
                                // taxAcct = admCharge.TaxAcctNo.Replace("***", getBranchCode.BranchCode);
                            }

                            //AccountValParam.AcctType = admCharge.TaxAcctType;
                            AccountValParam.AcctNo = taxAcct;
                            // AccountValParam.CrncyCode = admCharge.TaxCurrency;
                            AccountValParam.Username = p.LoginUserName;

                            var ValTax = await _AccountValidationImplementation.ValidateAccountCall(AccountValParam);
                            if (ValTax != null)
                            {
                                oprServiceCharge.TaxAcctNo = taxAcct;
                                oprServiceCharge.TaxAcctType = ValTax.sAccountType;

                            }

                            oprServiceCharge.Status = UnauthorizedStatus;
                            oprServiceCharge.ServiceId = p.ServiceId;
                            oprServiceCharge.ServiceItbId = i.ItbId;
                            oprServiceCharge.ServiceItbId = i.ItbId;


                            oprServiceCharge.DateCreated = DateTime.Now;
                            oprServiceCharge.SeqNo = SeqNo;


                            var SaveServiceChg = await _ServiceChargeImplementation.SaveServiceCharges(oprServiceCharge, p.UserId);

                            _repoOprInstrument.Update(i);
                            var rev = await _ApplicationDbContext.SaveChanges(p.UserId);
                            if (rev > 0)
                            {

                                ApiResponse.ResponseCode = 0;
                                ApiResponse.sErrorText = "Processed Successfully";

                                return Ok(ApiResponse);
                            }
                        }
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

        [HttpPost("DismissList")]
        public async Task<IActionResult> DismissList(InstrumentDTO p)
        {
            try
            {


                string Ids = string.Empty, tranIds = string.Empty;
                foreach (var b in p.ListOprInstrumentDTO)
                {
                    Ids += b.ItbId + ",";
                }

                tranIds = _Formatter.RemoveLast(Ids);

                string DimissedStatus = _configuration["Statuses:DimissedStatus"]; ;
                var cbs = new DapperDATAImplementation<OprInstrument>();

                string script = $"select * from OprCard where itbid in({tranIds})";

                var transactions = await cbs.LoadListNoParam(script, db);
                foreach (var i in transactions)
                {
                    i.InstrumentStatus = DimissedStatus;
                    i.DismissedBy = p.UserId;
                    i.DismissedDate = DateTime.Now;

                    _repoOprInstrument.Update(i);

                    int revToken = await _ApplicationDbContext.SaveChanges(p.UserId);

                    int upServiceCharge = -1;

                    var getSevice = await _repooprServiceCharge.GetManyAsync(c => c.ServiceId == i.ServiceId && c.ServiceItbId == i.ItbId);

                    if (getSevice != null)
                    {

                        foreach (var b in getSevice)
                        {

                            b.Status = DimissedStatus;
                            _repooprServiceCharge.Update(b);
                            upServiceCharge = await _ApplicationDbContext.SaveChanges(p.UserId);

                        }
                    }

                    if (getSevice.Count() > 0)
                    {

                        if (revToken > 0)
                        {

                            ApiResponse.ResponseCode = 0;
                            ApiResponse.sErrorText = "Dismissed Successfully";

                            return Ok(ApiResponse);
                        }

                    }


                    if (getSevice.Count() > 0)
                    {

                        if (upServiceCharge > 0)
                        {

                            ApiResponse.ResponseCode = 0;
                            ApiResponse.sErrorText = "Dismissed Successfully";

                            return Ok(ApiResponse);
                        }

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

        [HttpPost("Dismiss")]
        public async Task<IActionResult> Dismiss(InstrumentDTO p)
        {
            try
            {
                string DimissedStatus = _configuration["Statuses:DimissedStatus"];

                int updateOprSer = 0;

                var getSer = await _repooprServiceCharge.GetManyAsync(c => c.ServiceItbId == p.OprInstrument.ItbId && c.ServiceId == p.ServiceId);

                foreach (var b in getSer)
                {

                    if (b != null)
                    {
                        b.Status = DimissedStatus;
                        _repooprServiceCharge.Update(b);
                        updateOprSer = await _ApplicationDbContext.SaveChanges(p.UserId);
                    }


                }
                var rev = -1;
                var getRec = await _repoOprInstrument.GetAsync(c => c.ItbId == p.OprInstrument.ItbId);
                if (getRec != null)
                {

                    getRec.ProcessingDeptId = p.DeptId;
                    getRec.InstrumentStatus = DimissedStatus;
                    _repoOprInstrument.Update(getRec);
                    rev = await _ApplicationDbContext.SaveChanges(p.UserId);

                }

                if (rev > 0)
                {

                    ApiResponse.ResponseCode = 0;
                    ApiResponse.sErrorText = "Dismissed Successfully!";

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

        [HttpPost("CalCulateCharge")]
        public async Task<IActionResult> CalCulateCharge(InstrumentDTO p)
        {
            try
            {
                AccountValidationDTO ColAcctDetails = new AccountValidationDTO();


                AccountValParam AccountValParam = new AccountValParam();

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


                    //end
                }



                /* Contingent Detail */
                OprInstrument OprInstrument = new OprInstrument();
                var getCon = await _repoadmService.GetAsync(c => c.ServiceId == p.ServiceId);

                if (getCon != null)
                {

                    int branchNo = Convert.ToInt32(InstrumentAcctDetails.nBranch);
                    var getBranch = await _repoadmBankBranch.GetAsync(c => c.BranchNo == branchNo);

                    string con = "***";

                    if (getCon.ContDrAcctNo.Contains(con))
                    {
                        string act = getBranch != null ? getCon.ContDrAcctNo.Replace(con, getBranch.BranchCode) : getCon.ContDrAcctNo;
                        getCon.ContDrAcctNo = null;
                        getCon.ContDrAcctNo = act;

                    }

                    AccountValParam AccountValContigencyDr = new AccountValParam();
                    //, , OperationViewModel.InstrumentCcyIni, _userName
                    AccountValContigencyDr.AcctNo = getCon.ContDrAcctNo;
                    AccountValContigencyDr.AcctType = getCon.ContDrAcctType;

                    AccountValContigencyDr.CrncyCode = InstrumentAcctDetails.sCrncyIso.Trim();
                    AccountValContigencyDr.Username = p.LoginUserName;

                    var valDrContingentAcct = await _AccountValidationImplementation.ValidateAccountCall(AccountValContigencyDr);


                    if (valDrContingentAcct != null)
                    {
                        OprInstrument.ContDrAcctType = valDrContingentAcct.sAccountType;
                        OprInstrument.ContDrAcctNo = getCon.ContDrAcctNo;
                        OprInstrument.ContDrAcctName = valDrContingentAcct.sName;
                        OprInstrument.ContDrCcyCode = valDrContingentAcct.sCrncyIso;
                        OprInstrument.ContDrAvailBal = valDrContingentAcct.nBalanceDec;
                        OprInstrument.ContDrAcctStatus = valDrContingentAcct.nErrorCode == 0 ? valDrContingentAcct.sStatus : valDrContingentAcct.sErrorText;
                    }

                    if (getCon.ContCrAcctNo.Contains(con))
                    {
                        string act = getBranch != null ? getCon.ContCrAcctNo.Replace(con, getBranch.BranchCode) : getCon.ContCrAcctNo;
                        getCon.ContCrAcctNo = null;
                        getCon.ContCrAcctNo = act;

                    }

                    AccountValParam AccountValContigencyCR = new AccountValParam();

                    AccountValContigencyCR.AcctType = getCon.ContCrAcctType;
                    AccountValContigencyCR.AcctNo = getCon.ContCrAcctNo;
                    AccountValContigencyCR.CrncyCode = InstrumentAcctDetails.sCrncyIso.Trim();
                    AccountValContigencyCR.Username = p.LoginUserName;


                    var valCrContingentAcct = await _AccountValidationImplementation.ValidateAccountCall(AccountValContigencyCR);

                    if (valCrContingentAcct != null)
                    {
                        OprInstrument.ContCrAcctType = valCrContingentAcct.sAccountType;
                        OprInstrument.ContCrAcctNo = getCon.ContCrAcctNo;
                        OprInstrument.ContCrAcctName = valCrContingentAcct.sName;
                        OprInstrument.ContCrCcyCode = valCrContingentAcct.sCrncyIso;
                        OprInstrument.ContCrAvailBal = valCrContingentAcct.nBalanceDec;

                        OprInstrument.ContCrAcctStatus = valCrContingentAcct.nErrorCode == 0 ? valCrContingentAcct.sStatus : valCrContingentAcct.sErrorText;
                    }
                }



                var chg = new
                {
                    InstrumentAcctDetails = InstrumentAcctDetails,
                    chgList = chgList,
                    ColAcctDetails = ColAcctDetails,
                    OprInstrument = OprInstrument

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
                AccountValidationDTO ColAcctDetails = new AccountValidationDTO();

                AccountValParam AccountValParam = new AccountValParam();

                AccountValParam.AcctNo = p.AcctNo;
                AccountValParam.AcctType = p.AcctType == null ? null : p.AcctType.Trim();

                AccountValParam.CrncyCode = p.CcyCode;
                AccountValParam.Username = p.LoginUserName;

                var InstrumentAcctDetails = await _AccountValidationImplementation.ValidateAccountCall(AccountValParam);
                InstrumentAcctDetails.sRsmId = InstrumentAcctDetails.sCustomerId != null ? Convert.ToInt32(InstrumentAcctDetails.sCustomerId) : 0;

                List<RevCalChargeModel> chgList = new List<RevCalChargeModel>();

                var getOrigin = await _repoOprInstrument.GetAsync(c => c.ItbId == p.OprInstrument.ItbId);

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
                    OperationViewModel.TransAmount = "0"; // the procedure will take care of it as amt.ToString() ;
                    OperationViewModel.InstrumentAcctNo = i.ChgAcctNo == null ? p.AcctNo : i.ChgAcctNo;
                    OperationViewModel.InstrumentCcy = InstrumentAcctDetails.sCrncyIso;
                    OperationViewModel.ChargeCode = i.ChargeCode;

                    int? TempId = i.TemplateId != null ? _Formatter.ValIntergers(i.TemplateId.ToString()) : 0;

                    OperationViewModel.TempTypeId = TempId == 0 ? null : TempId;

                    // string ChgAcctBr, string AcctType, 
                    // string AcctCcy, string Ammendment =  "N", 
                    // string Reprint = "N", decimal IChgAmount = 0
                    //New 

                    //ChgDetails.nBranch.ToString(), ChgDetails.sAccountType, ChgDetails.sCrncyIso
                    string ammend = "N", reprint = "N";
                    if (p.IsAmmendment)
                    {
                        ammend = "Y";
                    }
                    if (!p.IsAmmendment)
                    {
                        reprint = "Y";

                    }


                    var InitialchargeceAmount = i.EquivChgAmount.ToString() != null ? _Formatter.ValDecimal(i.EquivChgAmount.ToString()) : 0;

                    var calCharge = await _ComputeChargesImplementation
                    .CalChargeModel(OperationViewModel,
                        ChgDetails.nBranch.ToString(), ChgDetails.sAccountType,
                    ChgDetails.sCrncyIso, ammend, reprint, InitialchargeceAmount);


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


                    //end
                }



                /* Contingent Detail */
                OprInstrument OprInstrument = new OprInstrument();
                var getCon = await _repoadmService.GetAsync(c => c.ServiceId == p.ServiceId);

                if (getCon != null)
                {

                    int branchNo = Convert.ToInt32(InstrumentAcctDetails.nBranch);
                    var getBranch = await _repoadmBankBranch.GetAsync(c => c.BranchNo == branchNo);

                    string con = "***";

                    if (getCon.ContDrAcctNo.Contains(con))
                    {
                        string act = getBranch != null ? getCon.ContDrAcctNo.Replace(con, getBranch.BranchCode) : getCon.ContDrAcctNo;
                        getCon.ContDrAcctNo = null;
                        getCon.ContDrAcctNo = act;

                    }

                    AccountValParam AccountValContigencyDr = new AccountValParam();
                    //, , OperationViewModel.InstrumentCcyIni, _userName
                    AccountValContigencyDr.AcctNo = getCon.ContDrAcctNo;
                    AccountValContigencyDr.AcctType = getCon.ContDrAcctType;

                    AccountValContigencyDr.CrncyCode = InstrumentAcctDetails.sCrncyIso.Trim();
                    AccountValContigencyDr.Username = p.LoginUserName;

                    var valDrContingentAcct = await _AccountValidationImplementation.ValidateAccountCall(AccountValContigencyDr);


                    if (valDrContingentAcct != null)
                    {
                        OprInstrument.ContDrAcctType = valDrContingentAcct.sAccountType;
                        OprInstrument.ContDrAcctNo = getCon.ContDrAcctNo;
                        OprInstrument.ContDrAcctName = valDrContingentAcct.sName;
                        OprInstrument.ContDrCcyCode = valDrContingentAcct.sCrncyIso;
                        OprInstrument.ContDrAvailBal = valDrContingentAcct.nBalanceDec;
                        OprInstrument.ContDrAcctStatus = valDrContingentAcct.nErrorCode == 0 ? valDrContingentAcct.sStatus : valDrContingentAcct.sErrorText;
                    }

                    if (getCon.ContCrAcctNo.Contains(con))
                    {
                        string act = getBranch != null ? getCon.ContCrAcctNo.Replace(con, getBranch.BranchCode) : getCon.ContCrAcctNo;
                        getCon.ContCrAcctNo = null;
                        getCon.ContCrAcctNo = act;

                    }

                    AccountValParam AccountValContigencyCR = new AccountValParam();

                    AccountValContigencyCR.AcctType = getCon.ContCrAcctType;
                    AccountValContigencyCR.AcctNo = getCon.ContCrAcctNo;
                    AccountValContigencyCR.CrncyCode = InstrumentAcctDetails.sCrncyIso.Trim();
                    AccountValContigencyCR.Username = p.LoginUserName;


                    var valCrContingentAcct = await _AccountValidationImplementation.ValidateAccountCall(AccountValContigencyCR);

                    if (valCrContingentAcct != null)
                    {
                        OprInstrument.ContCrAcctType = valCrContingentAcct.sAccountType;
                        OprInstrument.ContCrAcctNo = getCon.ContCrAcctNo;
                        OprInstrument.ContCrAcctName = valCrContingentAcct.sName;
                        OprInstrument.ContCrCcyCode = valCrContingentAcct.sCrncyIso;
                        OprInstrument.ContCrAvailBal = valCrContingentAcct.nBalanceDec;

                        OprInstrument.ContCrAcctStatus = valCrContingentAcct.nErrorCode == 0 ? valCrContingentAcct.sStatus : valCrContingentAcct.sErrorText;
                    }
                }



                var chg = new
                {
                    InstrumentAcctDetails = InstrumentAcctDetails,
                    chgList = chgList,
                    ColAcctDetails = ColAcctDetails,
                    OprInstrument = OprInstrument

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
        public async Task<IActionResult> GetById(InstrumentDTO p)
        {
            try
            {
                var serviceChargeslist = new List<oprServiceCharges>();
                var instrumentDetails = await _repoOprInstrument.GetAsync(c => c.ItbId == p.OprInstrument.ItbId);


                AccountValParam AccountValParam = new AccountValParam();

                AccountValParam.AcctNo = instrumentDetails.AcctNo;
                AccountValParam.AcctType = instrumentDetails.AcctType == null ? null : instrumentDetails.AcctType.Trim();

                AccountValParam.CrncyCode = instrumentDetails.CcyCode;
                AccountValParam.Username = "System";

                var valInstrumentAcct = await _AccountValidationImplementation.ValidateAccountCall(AccountValParam);

                if (instrumentDetails != null)
                {
                    serviceChargeslist = await _ServiceChargeImplementation.GetServiceChargesByServIdAndItbId(instrumentDetails.ServiceId, (int)instrumentDetails.ItbId);

                    var template = await _repooprInstrmentTem.GetAsync(c => c.ServiceId == instrumentDetails.ServiceId && c.ServiceItbId == (int)instrumentDetails.ItbId);


                    var collateral = await _repooprCollateral.GetManyAsync(c => c.ServiceId == instrumentDetails.ServiceId && c.ServiceItbId == instrumentDetails.ItbId);


                    var allUsers = await _UsersImplementation.GetAllUser(instrumentDetails.UserId, instrumentDetails.RejectedBy, instrumentDetails.DismissedBy, instrumentDetails.RejectedIds, instrumentDetails.SupervisorId, instrumentDetails.OriginatingBranchId);

                    if (instrumentDetails.RejectedDate != null)
                    {
                        allUsers.RejectedDate = instrumentDetails.RejectedDate.ToString();
                    }

                    if (instrumentDetails.DismissedDate != null)
                    {
                        allUsers.DismissedDate = instrumentDetails.DismissedDate.ToString();
                    }


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

                    var _response = await _AmmendReprintReasonImplementation.GetAllReasons();

                    var res = new
                    {
                        instrumentDetails = instrumentDetails,
                        serviceChargeslist = serviceChargeslist,
                        allUsers = allUsers,
                        collateral = collateral,
                        valInstrumentAcct = valInstrumentAcct,
                        template = template,
                        _response = _response

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

        [HttpPost("RetireInstrument")]
        public async Task<IActionResult> RetireInstrument(InstrumentDTO p)
        {
            try
            {
                string Ids = string.Empty, tranIds = string.Empty;

                string ArmStatusActive = _configuration["Statuses:ArmStatusActive"];
                string ArmClosedStatus = _configuration["Statuses:ArmClosedStatus"];
                string UnPostedStatus = _configuration["Statuses:UnPostedStatus"];

                int savCBS = 0, successSave = 0;
                foreach (var b in p.ListOprAmortizationScheduleDTO)
                {
                    var i = await _repoOprAmortizationSchedule.GetAsync(c => c.PrimaryId == b.PrimaryId);

                    if (i != null)
                    {
                        if (i.Status == ArmStatusActive)
                        {
                            CbsTransaction cbs = new CbsTransaction();

                            AccountValParam DrAccountIncomeVal = new AccountValParam();

                            DrAccountIncomeVal.AcctType = i.DrAcctType;
                            DrAccountIncomeVal.AcctNo = i.DrAcctNo;
                            DrAccountIncomeVal.CrncyCode = i.CurrencyCode;
                            DrAccountIncomeVal.Username = p.LoginUserName;

                            var ValDrAcct = await _AccountValidationImplementation.ValidateAccountCall(DrAccountIncomeVal);

                            AccountValParam CrAccountIncomeVal = new AccountValParam();

                            CrAccountIncomeVal.AcctType = i.CrAcctType;
                            CrAccountIncomeVal.AcctNo = i.CrAcctNo;
                            CrAccountIncomeVal.CrncyCode = i.CurrencyCode;
                            CrAccountIncomeVal.Username = p.LoginUserName;

                            var ValCrAcct = await _AccountValidationImplementation.ValidateAccountCall(DrAccountIncomeVal);

                            var CalAmtRem = i.InstlmtAmtRem - i.InstlmtAmount;

                            cbs.Amount = CalAmtRem;
                            cbs.ServiceId = i.ServiceId;
                            cbs.DrAcctBranchCode = i.OriginBranch;
                            cbs.DrAcctNo = i.DrAcctNo;
                            cbs.DrAcctType = i.DrAcctType;
                            cbs.DrAcctTC = i.DrAcctTC;
                            if (i.InstlmtProcessed != null)
                            {
                                int? prc = i.InstlmtProcessed + 1;
                                cbs.DrAcctNarration = i.DrAcctNarration.Replace("{TransTracer}", i.TransTracer)
                                                    .Replace("{InstalmentProcessed}", prc.ToString());
                            }

                            cbs.CrAcctNo = i.CrAcctNo;
                            cbs.CrAcctType = i.CrAcctType;

                            if (ValDrAcct != null)
                            {
                                cbs.DrAcctName = ValDrAcct.sName;
                                cbs.DrAcctBalance = ValDrAcct.nBalanceDec;
                                cbs.DrAcctStatus = ValDrAcct.sStatus;
                                cbs.DrAcctAddress = ValDrAcct.sAddress;
                                cbs.DrAcctClassCode = ValDrAcct.sProductCode;
                                cbs.DrAcctCity = ValDrAcct.sCity;
                                cbs.DrAcctIncBranch = ValDrAcct.nBranch;
                                cbs.DrAcctValErrorCode = ValDrAcct.nErrorCode;
                                cbs.DrAcctValErrorMsg = ValDrAcct.sErrorText;
                                cbs.DrAcctBranchCode = ValDrAcct.nBranch;
                                cbs.DrAcctIndusSector = ValDrAcct.sSector;
                                cbs.DrAcctCustType = ValDrAcct.sCustomerType;
                                cbs.DrAcctCustNo = _Formatter.ValIntergers(ValDrAcct.sCustomerId);
                                cbs.DrAcctCashBalance = _Formatter.ValDecimal(ValDrAcct.nCashBalance);
                            }

                            if (ValCrAcct != null)
                            {
                                cbs.CrAcctName = ValCrAcct.sName;
                                cbs.CrAcctBalance = ValCrAcct.nBalanceDec;
                                cbs.CrAcctStatus = ValCrAcct.sStatus;
                                cbs.CrAcctAddress = ValCrAcct.sAddress;
                                cbs.CrAcctCity = ValCrAcct.sCity;
                                cbs.CrAcctIncBranch = ValCrAcct.nBranch;
                                cbs.CrAcctValErrorCode = ValCrAcct.nErrorCode;
                                cbs.CrAcctValErrorMsg = ValCrAcct.sErrorText;
                                cbs.CrAcctBranchCode = ValCrAcct.nBranch;
                                cbs.CrAcctIndusSector = ValCrAcct.sSector;
                                cbs.CrAcctCustType = ValCrAcct.sCustomerType;
                                cbs.CrAcctCustNo = _Formatter.ValIntergers(ValCrAcct.sCustomerId);
                                cbs.CrAcctProdCode = ValCrAcct.sProductCode;
                                cbs.CrAcctValUserId = p.LoginUserId;
                                cbs.CrAcctCashBalance = _Formatter.ValDecimal(ValCrAcct.nCashBalance);
                            }

                            if (i.InstlmtProcessed != null)
                            {
                                cbs.DrAcctChargeNarr = cbs.DrAcctNarration;
                            }

                            var oprServiceModel = await _repoadmService.GetAsync(c => c.ServiceId == i.ServiceId);

                            cbs.CcyCode = i.CurrencyCode;
                            cbs.CrAcctNarration = cbs.DrAcctNarration;
                            cbs.CrAcctChargeNarr = cbs.DrAcctNarration;
                            cbs.DrAcctTC = i.DrAcctType != "GL" ? oprServiceModel.CustAcctDrTC : oprServiceModel.GLAcctDrTC;
                            cbs.CrAcctTC = i.CrAcctType != "GL" ? oprServiceModel.CustAcctCrTC : oprServiceModel.GLAcctCrTC;
                            cbs.BatchId = 1;
                            cbs.BatchSeqNo = 1;
                            cbs.PostingDate = _Formatter.ValidateDate(p.BankingDate);
                            cbs.TransactionDate = DateTime.Now;
                            cbs.ValueDate = i.NextInstlmtDate;
                            cbs.PrimaryId = i.PrimaryId;
                            cbs.DateCreated = DateTime.Now;

                            cbs.Status = UnPostedStatus;

                            var serviceTransRef = await _ComputeChargesImplementation.GenTranRef(i.ServiceId);
                            cbs.TransReference = serviceTransRef.nReference;

                            var ClientProfile = await _repoadmClientProfile.GetAsync(null);

                            cbs.TransTracer = i.TransTracer;
                            cbs.ChannelId = i.Channel;
                            cbs.DeptId = i.DeptId;
                            cbs.ProcessingDept = i.ProcessingDept;
                            cbs.UserId = p.LoginUserId;
                            cbs.Direction = 1;
                            cbs.PrimaryId = i.PrimaryId;
                            cbs.OriginatingBranchId = i.OriginBranch;
                            cbs.ChannelId = ClientProfile.ChannelId;
                            cbs.DateCreated = DateTime.Now;

                            await _repoCbsTransaction.AddAsync(cbs);
                            savCBS = await _ApplicationDbContext.SaveChanges(p.UserId);

                            if (savCBS > 0)
                            {
                                i.Status = ArmClosedStatus;
                                _repoOprAmortizationSchedule.Update(i);
                                int savAmor = await _ApplicationDbContext.SaveChanges(p.UserId);

                                //check for Amortization Leg

                                DynamicParameters param = new DynamicParameters();

                                param.Add("@primaryid", i.PrimaryId);
                                param.Add("@pnServiceId", i.ServiceId);

                                var rtn = new DapperDATAImplementation<CbsTransaction>();

                                var CBSContigencyLeg = await rtn.LoadData("Isp_GetContigencyLeg", param, db);

                                staticCbsTransaction = CBSContigencyLeg.FirstOrDefault();

                                if (CBSContigencyLeg != null)
                                {
                                    CbsTransaction cbs1 = new CbsTransaction();
                                    cbs1 = _CBSTransImplementation.CbsExchangeDrForCr(staticCbsTransaction, staticCbsTransaction);

                                    if (cbs1 != null)
                                    {

                                        await _repoCbsTransaction.AddAsync(cbs1);

                                        int savForContingencyLeg = await _ApplicationDbContext.SaveChanges(p.UserId);
                                        if (savForContingencyLeg > 0)
                                            successSave++;
                                    }
                                    else
                                    {
                                        successSave++;
                                    }





                                }
                            }
                        }
                    }
                }

                if (p.ListOprAmortizationScheduleDTO.Count == successSave)
                {

                    ApiResponse.ResponseMessage = "Instrument Retired Successfully!";
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

            return BadRequest(ApiResponse);
        }



    }
}