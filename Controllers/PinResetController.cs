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
    public class PinResetController : ControllerBase
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

        IRepository<oprDocChgDetail> _repoprDocChgDetail;


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

        IRepository<admCouterChqReason> _repoadmCouterChqReason;
        IRepository<oprPinReset> _repooprPinReset;
        IRepository<admPinItems> _repooprPinItems;

        public PinResetController(
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
                                           IRepository<oprInstrmentTemp> repooprInstrmentTem,
                                           IRepository<admCouterChqReason> repoadmCouterChqReason,
                                            IRepository<admPinItems> repooprPinItems,
                                            IRepository<oprDocChgDetail> repoprDocChgDetail,
                                            IRepository<oprPinReset> repooprPinReset
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
            _repoadmCharge = repoadmCharge;
            _repoadmBankBranch = repoadmBankBranch;
            _AccountValidationImplementation = AccountValidationImplementation;
            _ApplicationReturnMessageImplementation = ApplicationReturnMessageImplementation;
            _ComputeChargesImplementation = ComputeChargesImplementation;
            _UsersImplementation = UsersImplementation;
            _HeaderLogin = HeaderLogin;
            _RoleAssignImplementation = RoleAssignImplementation;
            _repoadmService = repoadmService;
            _repooprPinItems = repooprPinItems;
            _repooprPinReset = repooprPinReset;
            _repooprInstrmentTem = repooprInstrmentTem;
            _repoprDocChgDetail = repoprDocChgDetail;
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


                var rtn = new DapperDATAImplementation<oprPinResetDTO>();

                var _response = await rtn.LoadData("isp_GetPinReset", param, db);


                var oprPinItems = await _repooprPinItems.GetManyAsync(c => c.PinId > 0);


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
                        charge = getCharge,
                        RoleAssign = RoleAssign,
                        admService = admService,
                        oprPinItems = oprPinItems
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

        [HttpPost("GetAllPinSearchHistory")]
        public async Task<IActionResult> GetAllPinSearchHistory(ParamLoadPage AnyAuth)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                if(!string.IsNullOrEmpty(AnyAuth.UserId.ToString()))
                {
                    param.Add("@pnUserId", AnyAuth.UserId);
                }
                
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
                    param.Add("@AvailBal", Convert.ToDecimal(AnyAuth.AvailBal));
                }

                if(!string.IsNullOrEmpty(AnyAuth.AcctStatus))
                {
                    param.Add("@acctStatus", AnyAuth.AcctStatus);
                }
                
                if(!string.IsNullOrEmpty(AnyAuth.ServiceId.ToString()))
                {
                    param.Add("@ServiceId", AnyAuth.ServiceId);
                }
                if(!string.IsNullOrEmpty(AnyAuth.pnDeptId))
                {
                    param.Add("@psDeptId", Convert.ToInt32(AnyAuth.pnDeptId));
                }
                if(!string.IsNullOrEmpty(AnyAuth.psBranchNo))
                {
                    param.Add("@psDeptId", Convert.ToInt32(AnyAuth.psBranchNo));
                }
                if(!string.IsNullOrEmpty(AnyAuth.pinIds))
                {
                    param.Add("@pinIds", Convert.ToInt32(AnyAuth.pinIds));
                }

                // param.Add("@pnGlobalView", IsGlobalView);

                var rtn = new DapperDATAImplementation<oprPinResetDTO>();

                var _response = await rtn.LoadData("isp_GetPinResetHistory", param, db);

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
            catch(Exception ex)
            {

            }
            return Ok();
        }
        
        //this method gets list of pin that a re ready for reset
        [HttpPost("GetAllAfterPost")]
        public async Task<IActionResult> GetAllAfterPost(ParamLoadPage AnyAuth)
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
                param.Add("@pnIsPosted", "Y");


                var rtn = new DapperDATAImplementation<oprPinResetDTO>();

                var _response = await rtn.LoadData("isp_GetPinResetAfterChgPost", param, db);


                var oprPinItems = await _repooprPinItems.GetManyAsync(c => c.PinId > 0);


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
                        charge = getCharge,
                        RoleAssign = RoleAssign,
                        admService = admService,
                        oprPinItems = oprPinItems
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


        [HttpPost("UpdateAfterCharge")]
        public async Task<IActionResult> UpdateAfterCharge(PinResetDTO p)
        {
            try
            {
                int rev = 0, success = 0;
                foreach (var b in p.ListoprPinReset)
                {
                    var get = await _repooprPinReset.GetAsync(c=> c.ItbId == b.ItbId);
                     get.ResetCompletedBy = p.LoginUserId;
                     get.ResetCompletedDate = DateTime.Now;
                    _repooprPinReset.Update(get);
                     rev = await _ApplicationDbContext.SaveChanges(p.LoginUserId);
                    if(rev > 0)
                    success++;
                }
                if (rev > 0 && p.ListoprPinReset.Count() == success)
                {
                        ApiResponse.ResponseCode = 0;
                        ApiResponse.ResponseMessage = "Pin Reset Successful!";

                        return Ok(ApiResponse);
               }
            }
            catch (Exception ex)
            {
                ApiResponse.ResponseMessage = ex == null ? ex.InnerException.Message : ex.Message;

                ApiResponse.ResponseCode = -99;
                return BadRequest(ApiResponse);
            }
            ApiResponse.ResponseMessage = "Error Occured Successfully";
            return BadRequest(ApiResponse);
        }


        [HttpPost("Add")]
        public async Task<IActionResult> Add(PinResetDTO p)
        {
            try
            {
                oprPinReset oprPinReset = new oprPinReset();

                oprPinReset = p.oprPinReset;

                string PinIds = string.Empty;

                foreach (var b in p.ListoprPinItems)
                {


                    PinIds += b.PinId + ",";

                }

                PinIds = _Formatter.RemoveLast(PinIds);


                string UnauthorizedStatus = _configuration["Statuses:UnauthorizedStatus"];
                oprPinReset.ServiceStatus = UnauthorizedStatus;
                oprPinReset.DateCreated = DateTime.Now;
                oprPinReset.TransactionDate = Convert.ToDateTime(p.TransactionDate);
                oprPinReset.ValueDate = Convert.ToDateTime(p.ValueDate);
                oprPinReset.PinIds = PinIds;

                var serviceRef = await _ComputeChargesImplementation.GenServiceRef(p.ServiceId);
                oprPinReset.ReferenceNo = serviceRef.nReference;

                await _repooprPinReset.AddAsync(oprPinReset);
                int rev = await _ApplicationDbContext.SaveChanges(p.LoginUserId);

                int SaveServiceChg = 0, serviceChargeCount = 0, successServiceCharge = 0;

                serviceChargeCount = p.ListoprServiceCharge.Count();



                if (rev > 0)
                {
                    int SeqNo = 0;
                    foreach (var b in p.ListoprServiceCharge)
                    {
                        SeqNo += 1;
                        b.ServiceItbId = oprPinReset.ItbId;
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

                        if (SaveServiceChg > 0)
                            successServiceCharge++;

                    }

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
            ApiResponse.ResponseMessage = "Error Occured";
            return BadRequest(ApiResponse);
        }


        [HttpPost("Update")]
        public async Task<IActionResult> Update(PinResetDTO p)
        {
            try
            {
                oprPinReset oprPinReset = new oprPinReset();

                var get = await _repooprPinReset.GetAsync(x => x.ItbId == p.oprPinReset.ItbId);

                


                oprPinReset = p.oprPinReset;


                string PinIds = string.Empty;

                foreach (var b in p.ListoprPinItems)
                {


                    PinIds += b.PinId + ",";

                }

                if(get != null)
                {
                    if(PinIds.Contains(get.PinIds))
                    {
                        Console.WriteLine("Yes it contains");
                    }

                    if(get.AcctType == p.oprPinReset.AcctType && get.AcctNo == p.oprPinReset.AcctNo 
                        && get.CcyCode == p.oprPinReset.CcyCode && get.ProcessingDeptId == p.oprPinReset.ProcessingDeptId
                        && get.RsmId == p.oprPinReset.RsmId && PinIds.Contains(get.PinIds))
                    {
                        ApiResponse.ResponseCode = -99;
                        ApiResponse.ResponseMessage = "No changes was made";

                        return Ok(ApiResponse);
                    }
                }


                PinIds = _Formatter.RemoveLast(PinIds);

                oprPinReset.PinIds = PinIds;

                oprPinReset.ItbId = get.ItbId;

                get.AcctType = p.oprPinReset.AcctType;
                get.AcctNo = p.oprPinReset.AcctNo;

                get.CcyCode = p.oprPinReset.CcyCode;
                 get.ProcessingDeptId = p.oprPinReset.ProcessingDeptId;
                get.RsmId = p.oprPinReset.RsmId;
                get.PinIds =PinIds;

                _repooprPinReset.Update(get);
                int rev = await _ApplicationDbContext.SaveChanges(p.LoginUserId);
                int serviceChargeCount = 0, successServiceCharge = 0;

                serviceChargeCount = p.ListoprServiceCharge.Count();

                string refNo = p.oprPinReset.ReferenceNo;
                Int64? ItbId = p.oprPinReset.ItbId;

                if (rev > 0)
                {
                    int SeqNo = 0, SaveServiceChg = 0;
                    foreach (var b in p.ListoprServiceCharge)
                    {
                        //    oprServiceCharges serviceCharges = new oprServiceCharges();
                        //    serviceCharges = b;

                        var getExist = await _repooprServiceCharge.GetAsync(c => c.ServiceId == p.ServiceId && c.ServiceItbId == oprPinReset.ItbId && c.ItbId == b.ItbId);

                        if (getExist != null)
                        {
                            getExist.ServiceId = b.ServiceId;
                            getExist.ServiceItbId = b.ServiceItbId;
                            getExist.ChgAcctNo = b.ChgAcctNo;
                            getExist.ChgAcctType = b.ChgAcctType;
                            getExist.ChgAcctName = b.ChgAcctName;
                            getExist.ChgAvailBal = b.ChgAvailBal;
                            getExist.ChgAcctCcy = b.ChgAcctCcy;
                            getExist.ChgAcctStatus = b.ChgAcctStatus;
                            getExist.ChargeCode = b.ChargeCode;
                            getExist.ChargeRate = b.ChargeRate;
                            getExist.OrigChgAmount = b.OrigChgAmount;
                            getExist.OrigChgCCy = b.OrigChgCCy;
                            getExist.ExchangeRate = b.ExchangeRate;
                            getExist.EquivChgAmount = b.EquivChgAmount;
                            getExist.EquivChgCcy = b.EquivChgCcy;
                            getExist.ChgNarration = b.ChgNarration;
                            getExist.TaxAcctNo = b.TaxAcctNo;
                            getExist.TaxAcctType = b.TaxAcctType;
                            getExist.TaxRate = b.TaxRate;
                            getExist.TaxAmount = b.TaxAmount;
                            getExist.TaxNarration = b.TaxNarration;
                            getExist.IncBranch = b.IncBranch;
                            getExist.IncAcctNo = b.IncAcctNo;
                            getExist.IncAcctType = b.IncAcctType;
                            getExist.IncAcctName = b.IncAcctName;
                            getExist.IncAcctBalance = b.IncAcctBalance;
                            getExist.IncAcctStatus = b.IncAcctStatus;
                            getExist.IncAcctNarr = b.IncAcctNarr;
                            getExist.SeqNo = b.SeqNo;
                            getExist.Status = b.Status;
                            getExist.TemplateId = b.TemplateId;
                            _repooprServiceCharge.Update(getExist);

                            SaveServiceChg = await _ApplicationDbContext.SaveChanges(p.LoginUserId);

                            if (SaveServiceChg > 0)
                                successServiceCharge++;

                        }
                        if (getExist == null)
                        {
                            SeqNo += 1;
                            b.ServiceItbId = ItbId;
                            b.ServiceId = p.ServiceId;
                            b.Status = null;
                            b.DateCreated = DateTime.Now;
                            var nar = b.ChgNarration + " Ref: " + refNo;
                            b.ChgNarration = nar;
                            var taxnar = b.TaxNarration + " Ref: " + refNo;
                            b.TaxNarration = taxnar;
                            b.SeqNo = SeqNo;
                            var Innar = b.IncAcctNarr + " Ref: " + refNo;

                            b.IncAcctNarr = Innar;
                            SaveServiceChg = await _ServiceChargeImplementation.SaveServiceCharges(b, (int)p.LoginUserId);

                            if (SaveServiceChg > 0)
                                successServiceCharge++;

                        }



                    }

                    if (rev > 0 && serviceChargeCount == successServiceCharge)
                    {
                        ApiResponse.ResponseCode = 0;
                        ApiResponse.ResponseMessage = "Updated Successfully";

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
            ApiResponse.ResponseMessage = "Error Occured Successfully";
            return BadRequest(ApiResponse);
        }

        
        
        [HttpPost("GetById")]
        public async Task<IActionResult> GetById(PinResetDTO p)
        {
            try
            {
                var serviceChargeslist = new List<oprServiceCharges>();
                var instrumentDetails = await _repooprPinReset.GetAsync(c => c.ItbId == p.oprPinReset.ItbId);


                AccountValParam AccountValParam = new AccountValParam();

                AccountValParam.AcctNo = instrumentDetails.AcctNo;
                AccountValParam.AcctType = instrumentDetails.AcctType == null ? null : instrumentDetails.AcctType.Trim();

                AccountValParam.CrncyCode = instrumentDetails.CcyCode;
                AccountValParam.Username = "System";

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


                    var rtn = new DapperDATAImplementation<admPinItems>();

                    string script = $"Select * from admPinItems where PinId in ({instrumentDetails.PinIds})";

                    var pinSelected = await rtn.LoadListNoParam(script, db);

                    var getAlladmDocumentChg = await _repooprPinItems.GetManyAsync(c => c.PinId > 0);

                    var lisadmDoc = new List<oprPinItemsTemporal>();
                    var lisadmDoc1 = new List<oprPinItemsTemporal>();

                    foreach (var b in pinSelected)
                    {
                        var getOnlyOnlyOne = await _repooprPinItems.GetAsync(c => c.PinId == b.PinId);
                        lisadmDoc.Add(new oprPinItemsTemporal()
                        {
                            PinId = b.PinId,
                            Description = b.Description,
                            Status = b.Status,
                            UserId = b.UserId,
                            Select = true
                        });
                    }

                    foreach (var b in getAlladmDocumentChg)
                    {
                        var getOnlyOnlyOne = await _repooprPinItems.GetAsync(c => c.PinId == b.PinId);
                        lisadmDoc1.Add(new oprPinItemsTemporal()
                        {
                            PinId = b.PinId,
                            Description = b.Description,
                            Status = b.Status,
                            UserId = b.UserId,
                            Select = false
                        });

                    }

                    var ggg = lisadmDoc.Concat(lisadmDoc1);

                    var result = ggg.GroupBy(test => test.PinId)
                    .Select(grp => grp.First())
                    .ToList();

                    /////

                    var getCharge = await _ChargeImplementation.GetChargeDetails(instrumentDetails.ServiceId);

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
                        instrumentDetails = instrumentDetails,
                        serviceChargeslist = serviceChargeslist,
                        allUsers = allUsers,
                        valInstrumentAcct = valInstrumentAcct,
                        oprPinItems = result,
                        charge = getCharge
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


        // [HttpPost("ResetPassword")]
        // public async Task<IActionResult> ResetPassord(int UserId)
        // {
            
                
        // }


    }
}