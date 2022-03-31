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

namespace RevAssuranceApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [EnableCors("CorsPolicy")]
    //[Authorize]
    public class ReferenceLetterController : ControllerBase
    {
        IConfiguration _configuration;
        ApiResponse ApiResponse = new ApiResponse();
        TokenGenerator TokenGenerator;
        AppSettingsPath AppSettingsPath;
        IDbConnection db = null;
        ApplicationDbContext _ApplicationDbContext;
        AmmendReprintReasonImplementation _AmmendReprintReasonImplementation;
        Formatter _Formatter = new Formatter();
        ChargeImplementation _ChargeImplementation;
        ServiceChargeImplementation _ServiceChargeImplementation;
        IRepository<oprServiceCharges> _repooprServiceCharge;
        IRepository<OprReferenceLetter> _repoOprReferenceLetter;
        IRepository<admService> _repoadmService;

        IRepository<admCharge> _repoadmCharge;

        IRepository<admBankBranch> _repoadmBankBranch;
        AccountValidationImplementation _AccountValidationImplementation;
        ApplicationReturnMessageImplementation _ApplicationReturnMessageImplementation;
        ComputeChargesImplementation _ComputeChargesImplementation;
        UsersImplementation _UsersImplementation;
        HeaderLogin _HeaderLogin;
        RoleAssignImplementation _RoleAssignImplementation;
        IRepository<oprInstrmentTemp> _repooprInstrmentTem;
        IRepository<OprAmmendAndReprint> _repoOprAmmendAndReprint;
        
        public ReferenceLetterController(
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
                                        IRepository<OprAmmendAndReprint> repoOprAmmendAndReprint,
                                        IRepository<OprInstrument> repoOprInstrument,
                                        RoleAssignImplementation RoleAssignImplementation,
                                        IRepository<admService> repoadmService,
                                        IRepository<OprReferenceLetter> repoOprReferenceLetter,
                                        IRepository<oprInstrmentTemp> repooprInstrmentTem
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
            _repoOprReferenceLetter = repoOprReferenceLetter;
            _repoadmCharge = repoadmCharge;
            _repoadmBankBranch = repoadmBankBranch;
            _AccountValidationImplementation = AccountValidationImplementation;
            _ApplicationReturnMessageImplementation = ApplicationReturnMessageImplementation;
            _ComputeChargesImplementation = ComputeChargesImplementation;
            _UsersImplementation = UsersImplementation;
            _HeaderLogin = HeaderLogin;
            _RoleAssignImplementation = RoleAssignImplementation;
            _repoadmService = repoadmService;
            _repooprInstrmentTem = repooprInstrmentTem;
            _AmmendReprintReasonImplementation = AmmendReprintReasonImplementation;
            _repoOprAmmendAndReprint = repoOprAmmendAndReprint;
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
                param.Add("@pnDeptId", AnyAuth.pnDeptId);
                param.Add("@pnGlobalView", IsGlobalView);
                param.Add("@ServiceId", AnyAuth.ServiceId);

                var rtn = new DapperDATAImplementation<OprReferenceLetterDTO>();

                var _response = await rtn.LoadData("Isp_GetRefLetter", param, db);

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
                
                
                if(!string.IsNullOrEmpty(AnyAuth.AcctStatus))
                {
                    param.Add("@AccountStatus", AnyAuth.AcctStatus);
                }
                if(!string.IsNullOrEmpty(AnyAuth.ServiceStatus))
                {
                    param.Add("@ServiceStatus", Convert.ToDecimal(AnyAuth.ServiceStatus));
                }
                if(!string.IsNullOrEmpty(AnyAuth.ServiceId.ToString()))
                {
                    param.Add("@ServiceId", Convert.ToInt16(AnyAuth.ServiceId));
                }
                if(!string.IsNullOrEmpty(AnyAuth.pnDeptId))
                {
                    param.Add("@psDeptId", Convert.ToInt32(AnyAuth.pnDeptId));
                }
                if(!string.IsNullOrEmpty(AnyAuth.psBranchNo))
                {
                    param.Add("@psBranchNo", Convert.ToInt32(AnyAuth.psBranchNo));
                }

                // param.Add("@pnGlobalView", IsGlobalView);
                
                
              
                var rtn = new DapperDATAImplementation<OprReferenceLetterDTO>();
                
                var _response = await rtn.LoadData("Isp_GetRefLetterSearchHis", param, db);

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
        public async Task<IActionResult> Add(ReferenceLetterDTO p)
        {
            try
            {
                OprReferenceLetter _OprReferenceLetter = new OprReferenceLetter();

                _OprReferenceLetter = p.OprReferenceLetter;

                string UnauthorizedStatus = _configuration["Statuses:UnauthorizedStatus"];
                _OprReferenceLetter.ServiceStatus = UnauthorizedStatus;
                _OprReferenceLetter.DateCreated = DateTime.Now;
                _OprReferenceLetter.UserId = p.LoginUserId;
                _OprReferenceLetter.OrigDeptId = p.OprReferenceLetter.ProcessingDeptId;
                _OprReferenceLetter.TransactionDate = Convert.ToDateTime(p.TransactionDate);
                _OprReferenceLetter.ValueDate = Convert.ToDateTime(p.ValueDate);
                var serviceRef = await _ComputeChargesImplementation.GenServiceRef(p.ServiceId);
                _OprReferenceLetter.ReferenceNo = serviceRef.nReference;

                await _repoOprReferenceLetter.AddAsync(_OprReferenceLetter);
                int rev = await _ApplicationDbContext.SaveChanges(p.LoginUserId);
                int SaveServiceChg = 0, updateInstWithTempId = 0;


                if (rev > 0)
                {
                    int SeqNo = 0;
                    foreach (var b in p.ListoprServiceCharge)
                    {
                        SeqNo += 1;
                        b.ServiceItbId = _OprReferenceLetter.ItbId;
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
                    tem.ServiceItbId = _OprReferenceLetter.ItbId;
                    tem.UserId = (int)p.LoginUserId;
                    tem.DateCreated = DateTime.Now;
                    tem.TemplateContent = p.TemplateContent;

                    await _repooprInstrmentTem.AddAsync(tem);

                    var SaveTem = await _ApplicationDbContext.SaveChanges(p.LoginUserId);
                    if (SaveTem > 0)
                    {
                        _OprReferenceLetter.TemplateContentIds = tem.ItbId.ToString();
                        _repoOprReferenceLetter.Update(_OprReferenceLetter);
                        updateInstWithTempId = await _ApplicationDbContext.SaveChanges(p.LoginUserId);
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

        [HttpPost("AmmendOrReprint")]
        public async Task<IActionResult> AmmendOrReprint(ReferenceLetterDTO p)
        {
            try
            {
                OprReferenceLetter _OprReferenceLetter = new OprReferenceLetter();

                if (p.admAmendReprintReason.Chargeable == false)
                {
                    OprAmmendAndReprint oprAmmendAndReprint = new OprAmmendAndReprint();

                    oprAmmendAndReprint.ServiceItbId = p.OprReferenceLetter.ItbId;
                    oprAmmendAndReprint.ServiceId = p.OprReferenceLetter.ServiceId;
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

                    _OprReferenceLetter = p.OprReferenceLetter;


                    string UnauthorizedStatus = _configuration["Statuses:UnauthorizedStatus"];
                    _OprReferenceLetter.ServiceStatus = UnauthorizedStatus;
                    _OprReferenceLetter.DateCreated = DateTime.Now;
                    _OprReferenceLetter.UserId = p.LoginUserId;
                    _OprReferenceLetter.TransactionDate = Convert.ToDateTime(p.TransactionDate);
                    _OprReferenceLetter.ValueDate = Convert.ToDateTime(p.ValueDate);
                    var serviceRef = await _ComputeChargesImplementation.GenServiceRef(p.ServiceId);
                    _OprReferenceLetter.ReferenceNo = serviceRef.nReference;

                    await _repoOprReferenceLetter.AddAsync(_OprReferenceLetter);
                    int rev = await _ApplicationDbContext.SaveChanges(p.LoginUserId);
                    int SaveServiceChg = 0, updateInstWithTempId = 0;


                    if (rev > 0)
                    {
                        int SeqNo = 0;
                        foreach (var b in p.ListoprServiceCharge)
                        {
                            SeqNo += 1;
                            b.ServiceItbId = _OprReferenceLetter.ItbId;
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
                        tem.ServiceItbId = _OprReferenceLetter.ItbId;
                        tem.UserId = (int)p.LoginUserId;
                        tem.DateCreated = DateTime.Now;
                        tem.TemplateContent = p.TemplateContent;

                        await _repooprInstrmentTem.AddAsync(tem);

                        var SaveTem = await _ApplicationDbContext.SaveChanges(p.LoginUserId);
                        if (SaveTem > 0)
                        {
                            _OprReferenceLetter.TemplateContentIds = tem.ItbId.ToString();
                            _repoOprReferenceLetter.Update(_OprReferenceLetter);
                            updateInstWithTempId = await _ApplicationDbContext.SaveChanges(p.LoginUserId);
                        }

                        if (rev > 0 && SaveServiceChg > 0 && SaveTem > 0)
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
        public async Task<IActionResult> Update(ReferenceLetterDTO p)
        {
            try
            {
                OprReferenceLetter _OprReferenceLetter = new OprReferenceLetter();


                _OprReferenceLetter = p.OprReferenceLetter;

                long itbId = p.OprReferenceLetter.ItbId != null ? (int)p.OprReferenceLetter.ItbId : 0;

                var get = await _repoOprReferenceLetter.GetAsync(x => x.ItbId == itbId);
                if(get != null)
                {
                    if(get.AcctNo == p.OprReferenceLetter.AcctNo || get.AcctType == p.OprReferenceLetter.AcctType
                        && get.AcctName == p.OprReferenceLetter.AcctName && get.AcctSic == p.OprReferenceLetter.AcctSic
                        && get.CcyCode == p.OprReferenceLetter.CcyCode && get.AddresseeName == p.OprReferenceLetter.AddresseeName
                        && get.Addr1 == p.OprReferenceLetter.Addr1 && get.Addr2 == p.OprReferenceLetter.Addr2 && get.RsmId == p.OprReferenceLetter.RsmId
                        && get.ReferenceReason == p.OprReferenceLetter.ReferenceReason && get.BidNo == p.OprReferenceLetter.BidNo 
                        && get.CreditAmount == p.OprReferenceLetter.CreditAmount
                     )
                    {
                        ApiResponse.ResponseCode = -99;
                        ApiResponse.ResponseMessage = "No changes was made";

                        return Ok(ApiResponse);
                    }
                }
                _repoOprReferenceLetter.Update(_OprReferenceLetter);
                int rev = await _ApplicationDbContext.SaveChanges(p.LoginUserId);
                int SaveServiceChg = 0;
                if (rev > 0)
                {

                    foreach (var b in p.ListoprServiceCharge)
                    {

                        _repooprServiceCharge.Update(b);
                        SaveServiceChg = await _ApplicationDbContext.SaveChanges(p.LoginUserId);
                    }

                    var tem = await _repooprInstrmentTem.GetAsync(c => c.ServiceItbId == _OprReferenceLetter.ItbId && c.ServiceId == p.ServiceId);
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
        public async Task<IActionResult> GetById(ReferenceLetterDTO p)
        {
            try
            {


                var serviceChargeslist = new List<oprServiceCharges>();
                var instrumentDetails = await _repoOprReferenceLetter.GetAsync(c => c.ItbId == p.OprReferenceLetter.ItbId);


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