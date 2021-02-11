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

/*
  Overdraft has 3 Overdraft(OdType) types which are 
  1. 'NEW', 2. 'RENEWAL', 3. 'TOD'
  All these has the same work flow except TOD which has background service that 
  run in order to close the Overdraft when its dues. You can RevAssService project to see 
  the condition using in closing this Overdraft
*/

namespace RevAssuranceApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [EnableCors("CorsPolicy")]
    //[Authorize]
    public class OverDraftController : ControllerBase
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
        AmmendReprintReasonImplementation _AmmendReprintReasonImplementation;
        IRepository<oprServiceCharges> _repooprServiceCharge;
        IRepository<OprOverDraft> _repoOprOverDraft;
        IRepository<oprCollateral> _repooprCollateral;
        IRepository<admService> _repoadmService;
        IRepository<oprInstrmentTemp> _repooprInstrmentTem;
        IRepository<admCharge> _repoadmCharge;
        IRepository<admBankBranch> _repoadmBankBranch;
        IRepository<admDepartment> _repoadmDepartment;
        AccountValidationImplementation _AccountValidationImplementation;
        ApplicationReturnMessageImplementation _ApplicationReturnMessageImplementation;
        ComputeChargesImplementation _ComputeChargesImplementation;
        UsersImplementation _UsersImplementation;
        HeaderLogin _HeaderLogin;
        RoleAssignImplementation _RoleAssignImplementation;


        public OverDraftController(
                                        IConfiguration configuration,
                                        ApplicationDbContext ApplicationDbContext,
                                        ChargeImplementation ChargeImplementation,
                                        ServiceChargeImplementation ServiceChargeImplementation,
                                         IRepository<oprServiceCharges> repooprServiceCharge,
                                       AmmendReprintReasonImplementation AmmendReprintReasonImplementation,
                                         IRepository<admCharge> repoadmCharge,
                                         IRepository<admBankBranch> repoadmBankBranch,
                                         AccountValidationImplementation AccountValidationImplementation,
                                         ApplicationReturnMessageImplementation ApplicationReturnMessageImplementation,
                                         ComputeChargesImplementation ComputeChargesImplementation,
                                          UsersImplementation UsersImplementation,
                                           HeaderLogin HeaderLogin,
                                           IRepository<OprOverDraft> repoOprOverDraft,
                                           RoleAssignImplementation RoleAssignImplementation,
                                           IRepository<oprCollateral> repooprCollateral,
                                           IRepository<admService> repoadmService,
                                             IRepository<oprInstrmentTemp> repooprInstrmentTem, IRepository<admDepartment> repoadmDepartment
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
            _repoOprOverDraft = repoOprOverDraft;
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
            _repooprInstrmentTem = repooprInstrmentTem;
            _AmmendReprintReasonImplementation = AmmendReprintReasonImplementation;
            _repoadmDepartment = repoadmDepartment;
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

                param.Add("@pdtCurrentDate", _Formatter.FormatToDateYearMonthDay(AnyAuth.pdtCurrentDate));
                param.Add("@psBranchNo", AnyAuth.psBranchNo);
                param.Add("@pnDeptId", Convert.ToInt32(AnyAuth.pnDeptId));
                param.Add("@pnGlobalView", IsGlobalView);
                param.Add("@ServiceId", AnyAuth.ServiceId);

                var rtn = new DapperDATAImplementation<OprOverDraftDTO>();

                var _response = await rtn.LoadData("Isp_GetOverDraft", param, db);

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
        public async Task<IActionResult> GetAllSearchHistory(ParamLoadPage AnyAuth)
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
               
                // if(!string.IsNullOrEmpty(AnyAuth.psStatus))
                // {
                //     param.Add("@psStatus", AnyAuth.psStatus);
                // }
                if(!string.IsNullOrEmpty(AnyAuth.referenceNo) || !string.IsNullOrWhiteSpace(AnyAuth.referenceNo))
                {
                    param.Add("@referenceNo", AnyAuth.referenceNo);
                }
                if(!string.IsNullOrEmpty(AnyAuth.psCcyCode))
                {
                    param.Add("@psCcyCode", AnyAuth.psCcyCode);
                }
                if(!string.IsNullOrEmpty(AnyAuth.AccountName))
                {
                    param.Add("@AccountName", AnyAuth.AccountName);
                }
                if(!string.IsNullOrEmpty(AnyAuth.psAcctNo))
                {
                    param.Add("@AcctNo", AnyAuth.psAcctNo);
                }
                if(!string.IsNullOrEmpty(AnyAuth.psAcctType))
                {
                    param.Add("@AcctType", AnyAuth.psAcctType);
                }
                
                if(!string.IsNullOrEmpty(AnyAuth.AvailBal))
                {
                    param.Add("@AvailableBal", Convert.ToDecimal(AnyAuth.AvailBal));
                }
                if(!string.IsNullOrEmpty(AnyAuth.AcctStatus))
                {
                    param.Add("@AccountStatus", AnyAuth.AcctStatus);
                }
                if(!string.IsNullOrEmpty(AnyAuth.approvedLimit))
                {
                    param.Add("@apprLimit", Convert.ToDecimal(AnyAuth.approvedLimit));
                }
                if(!string.IsNullOrEmpty(AnyAuth.ServiceId.ToString()))
                {
                    param.Add("@ServiceId", Convert.ToInt16(AnyAuth.ServiceId));
                }
                // param.Add("@pnGlobalView", IsGlobalView);
                
                
              
                var rtn = new DapperDATAImplementation<OprOverDraftDTO>();
                
                var _response = await rtn.LoadData("Isp_GetOverDraftSearchHis", param, db);

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
        public async Task<IActionResult> Add(OverDraftDTO p)
        {
            try
            {
                OprOverDraft _OprOverDraft = new OprOverDraft();

                _OprOverDraft = p.OprOverDraft;

                string UnauthorizedStatus = _configuration["Statuses:UnauthorizedStatus"];
                _OprOverDraft.ServiceStatus = UnauthorizedStatus;
                _OprOverDraft.DateCreated = DateTime.Now;
                _OprOverDraft.UserId = p.LoginUserId;
                _OprOverDraft.TransactionDate = Convert.ToDateTime(p.TransactionDate);
                _OprOverDraft.ValueDate = Convert.ToDateTime(p.ValueDate);
                var serviceRef = await _ComputeChargesImplementation.GenServiceRef(p.ServiceId);
                _OprOverDraft.ReferenceNo = serviceRef.nReference;
                _OprOverDraft.ServiceCcy = _OprOverDraft.CcyCode;

                await _repoOprOverDraft.AddAsync(_OprOverDraft);
                int rev = await _ApplicationDbContext.SaveChanges(p.LoginUserId);
                int SaveServiceChg = 0, updateInstWithTempId = 0;

                oprInstrmentTemp tem = new oprInstrmentTemp();

                tem.ServiceId = p.ServiceId;
                tem.ServiceItbId = _OprOverDraft.ItbId;
                tem.UserId = (int)p.LoginUserId;
                tem.DateCreated = DateTime.Now;

                var getDept = await _repoadmDepartment.GetAsync(c => c.DeptId == _OprOverDraft.OrigDeptId);

                string FromDeptName = string.IsNullOrWhiteSpace(getDept.Deptname) != null ? getDept.Deptname : "{{FROMDEPT}}";
                string ToDeptName = string.IsNullOrWhiteSpace(getDept.Deptname) != null ? getDept.Deptname : "{{FROMDEPT}}";
                string Date = _Formatter.FormatDateCurrProcessing(DateTime.Now);
                string Tenor = $"{_OprOverDraft.Tenor} {_OprOverDraft.TenorPeriod}";
                string EffectiveDate = _Formatter.FormatTransDate(_OprOverDraft.StartDate);
                string ExpiryDate = _Formatter.FormatTransDate(_OprOverDraft.NewExpiryDate);

                var getBranch = await _repoadmBankBranch.GetAsync(c => c.BranchNo == _OprOverDraft.BranchNo);
                string IncomeBranch = string.IsNullOrWhiteSpace(getBranch.BranchName) != null ? getBranch.BranchName : "{{INCOMEBRANCH}}";

                if (_OprOverDraft.ApprovedLimit == null)
                    _OprOverDraft.ApprovedLimit = 0;
                if (_OprOverDraft.ApprovedOdRate == null)
                    _OprOverDraft.ApprovedOdRate = 0;

                string replaceTemplateContent = p.TemplateContent.Replace("{{FROMDEPT}}", FromDeptName)
                                                                 .Replace("{{TODEPT}}", ToDeptName)
                                                                 .Replace("{{DATE}}", Date)
                                                                 .Replace("{{ACTYPE}}", _OprOverDraft.AcctType)
                                                                 .Replace("{{FROMACCT}}", _OprOverDraft.AcctNo)
                                                                 .Replace("{{TENOR}}", Tenor)
                                                                 .Replace("{{EFFECTIVEDAY}}", EffectiveDate)
                                                                 .Replace("{{EXPIRYDATE}}", ExpiryDate)
                                                                 .Replace("{{INCOMEBRANCH}}", IncomeBranch)
                                                                 .Replace("{{ODAMOUNT}}", _Formatter.FormattedAmount((decimal)_OprOverDraft.ApprovedLimit))
                                                                 .Replace("{{INTERESTRATE}}", _OprOverDraft.ApprovedOdRate.ToString())
                                                                 .Replace("{{MGNTFEE}}", string.Empty)
                                                                 .Replace("{{FACIFEE}}", string.Empty)
                                                                 .Replace("{{MGNTGSTFEE}}", string.Empty)
                                                                 .Replace("{{FACIGSTFEE}}", string.Empty);

                tem.TemplateContent = replaceTemplateContent;//p.TemplateContent;

                await _repooprInstrmentTem.AddAsync(tem);

                var SaveTem = await _ApplicationDbContext.SaveChanges(p.LoginUserId);
                if (SaveTem > 0)
                {
                    _OprOverDraft.TemplateContentIds = tem.ItbId.ToString();
                    _repoOprOverDraft.Update(_OprOverDraft);
                    updateInstWithTempId = await _ApplicationDbContext.SaveChanges(p.LoginUserId);
                }


                if (rev > 0)
                {
                    int SeqNo = 0;
                    foreach (var b in p.ListoprServiceCharge)
                    {
                        SeqNo += 1;
                        b.ServiceItbId = _OprOverDraft.ItbId;
                        b.ServiceId = p.ServiceId;
                        b.Status = null;
                        b.DateCreated = DateTime.Now;
                        var nar = b.ChgNarration + " Ref: " + serviceRef.nReference;
                        b.ChgNarration = nar;
                        var taxnar = b.TaxNarration + " Ref: " + serviceRef.nReference;
                        b.TaxNarration = taxnar;
                        b.SeqNo = SeqNo;
                        var Innar = b.IncAcctNarr + " Ref: " + serviceRef.nReference;
                        b.TemplateId = _Formatter.ValIntergers(tem.ItbId.ToString());

                        b.IncAcctNarr = Innar;
                        SaveServiceChg = await _ServiceChargeImplementation.SaveServiceCharges(b, (int)p.LoginUserId);
                    }



                    if (rev > 0 && SaveServiceChg > 0 && SaveTem > 0)
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

            return BadRequest(ApiResponse);
        }



        [HttpPost("Update")]
        public async Task<IActionResult> Update(OverDraftDTO p)
        {
            try
            {
                OprOverDraft _oprTraderef = new OprOverDraft();

                _oprTraderef = p.OprOverDraft;

                _repoOprOverDraft.Update(_oprTraderef);
                int rev = await _ApplicationDbContext.SaveChanges(p.LoginUserId);
                int SaveServiceChg = 0;
                if (rev > 0)
                {

                    foreach (var b in p.ListoprServiceCharge)
                    {

                        _repooprServiceCharge.Update(b);
                        SaveServiceChg = await _ApplicationDbContext.SaveChanges(p.LoginUserId);
                    }

                    var tem = await _repooprInstrmentTem.GetAsync(c => c.ServiceItbId == _oprTraderef.ItbId && c.ServiceId == p.ServiceId);
                    tem.TemplateContent = p.TemplateContent;

                    _repooprInstrmentTem.Update(tem);

                    var SaveTem = await _ApplicationDbContext.SaveChanges(p.LoginUserId);


                    if (rev > 0 && SaveServiceChg > 0)
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

            return BadRequest(ApiResponse);
        }


        [HttpPost("GetById")]
        public async Task<IActionResult> GetById(OverDraftDTO p)
        {
            try
            {
                var serviceChargeslist = new List<oprServiceCharges>();
                var instrumentDetails = await _repoOprOverDraft.GetAsync(c => c.ItbId == p.OprOverDraft.ItbId);

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
                    var template = await _repooprInstrmentTem.GetAsync(c => c.ServiceId == instrumentDetails.ServiceId && c.ServiceItbId == (int)instrumentDetails.ItbId);
                    var _response = await _AmmendReprintReasonImplementation.GetAllReasons();
                    var res = new
                    {
                        instrumentDetails = instrumentDetails,
                        serviceChargeslist = serviceChargeslist,
                        allUsers = allUsers,
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