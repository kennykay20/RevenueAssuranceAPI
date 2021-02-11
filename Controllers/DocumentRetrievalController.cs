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
    public class DocumentRetrievalController : ControllerBase
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
        IRepository<oprDocRetrieval> _repooprDocRetrieval;
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
        IRepository<admDocumentChg> _repoadmDocumentChg;

        public DocumentRetrievalController(
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
                                          IRepository<oprDocRetrieval> repooprDocRetrieval,
                                           IRepository<oprInstrmentTemp> repooprInstrmentTem,
                                           IRepository<admCouterChqReason> repoadmCouterChqReason,
                                            IRepository<admDocumentChg> repoadmDocumentChg,
                                            IRepository<oprDocChgDetail> repoprDocChgDetail
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
            _repooprDocRetrieval = repooprDocRetrieval;
            _repoadmCharge = repoadmCharge;
            _repoadmBankBranch = repoadmBankBranch;
            _AccountValidationImplementation = AccountValidationImplementation;
            _ApplicationReturnMessageImplementation = ApplicationReturnMessageImplementation;
            _ComputeChargesImplementation = ComputeChargesImplementation;
            _UsersImplementation = UsersImplementation;
            _HeaderLogin = HeaderLogin;
            _RoleAssignImplementation = RoleAssignImplementation;
            _repoadmService = repoadmService;
            _repoadmDocumentChg = repoadmDocumentChg;
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


                var rtn = new DapperDATAImplementation<oprDocRetrievalDTO>();

                var _response = await rtn.LoadData("isp_GetDocRetrivalList", param, db);


                var admDocumentChg = await _repoadmDocumentChg.GetManyAsync(c => c.DocumentId > 0 && c.ServiceId == AnyAuth.ServiceId);


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
                        admDocumentChg = admDocumentChg
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
        public async Task<IActionResult> Add(DocumentRetrievalDTO p)
        {
            try
            {
                oprDocRetrieval oprDocRetrieval = new oprDocRetrieval();

                oprDocRetrieval = p.oprDocRetrieval;

                string UnauthorizedStatus = _configuration["Statuses:UnauthorizedStatus"];
                oprDocRetrieval.ServiceStatus = UnauthorizedStatus;
                oprDocRetrieval.DateCreated = DateTime.Now;
                oprDocRetrieval.TransactionDate = Convert.ToDateTime(p.TransactionDate);
                oprDocRetrieval.ValueDate = Convert.ToDateTime(p.ValueDate);

                var serviceRef = await _ComputeChargesImplementation.GenServiceRef(p.ServiceId);
                oprDocRetrieval.ReferenceNo = serviceRef.nReference;

                await _repooprDocRetrieval.AddAsync(oprDocRetrieval);
                int rev = await _ApplicationDbContext.SaveChanges(p.LoginUserId);
                int SaveServiceChg = 0, chgDetails = 0, numberOfprDocChgDetail = 0,
                     successAddChgDetails = 0, serviceChargeCount = 0, successServiceCharge = 0;

                serviceChargeCount = p.ListoprServiceCharge.Count();

                numberOfprDocChgDetail = p.ListoprDocChgDetail.Count();

                if (rev > 0)
                {
                    int SeqNo = 0;
                    foreach (var b in p.ListoprServiceCharge)
                    {
                        SeqNo += 1;
                        b.ServiceItbId = oprDocRetrieval.ItbId;
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

                    foreach (var i in p.ListoprDocChgDetail)
                    {
                        i.ServiceItbId = oprDocRetrieval.ItbId;
                        i.ServiceId = p.ServiceId;
                        i.DateCreated = DateTime.Now;
                        await _repoprDocChgDetail.AddAsync(i);

                        chgDetails = await _ApplicationDbContext.SaveChanges(p.LoginUserId);
                        if (chgDetails > 0)
                            successAddChgDetails++;
                    }

                    if (rev > 0 && SaveServiceChg > 0 && successAddChgDetails == numberOfprDocChgDetail && serviceChargeCount == successServiceCharge)
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
            ApiResponse.ResponseMessage = "Error Occured Successfully";
            return BadRequest(ApiResponse);
        }


        [HttpPost("Update")]
        public async Task<IActionResult> Update(DocumentRetrievalDTO p)
        {
            try
            {
                oprDocRetrieval oprDocRetrieval = new oprDocRetrieval();

                oprDocRetrieval = p.oprDocRetrieval;

                string UnauthorizedStatus = _configuration["Statuses:UnauthorizedStatus"];
                // oprDocRetrieval.ServiceStatus = UnauthorizedStatus;
                // oprDocRetrieval.DateCreated= DateTime.Now;
                // oprDocRetrieval.TransactionDate = Convert.ToDateTime(p.TransactionDate);
                // oprDocRetrieval.ValueDate = Convert.ToDateTime(p.ValueDate);

                //  var serviceRef = await _ComputeChargesImplementation.GenServiceRef(p.ServiceId);
                //  oprDocRetrieval.ReferenceNo = serviceRef.nReference;

                _repooprDocRetrieval.Update(oprDocRetrieval);
                int rev = await _ApplicationDbContext.SaveChanges(p.LoginUserId);
                int numberOfprDocChgDetail = 0,
                     successAddChgDetails = 0, serviceChargeCount = 0, successServiceCharge = 0;

                serviceChargeCount = p.ListoprServiceCharge.Count();

                numberOfprDocChgDetail = p.ListoprDocChgDetail.Count();

                if (rev > 0)
                {
                    foreach (var b in p.ListoprServiceCharge)
                    {
                        oprServiceCharges serviceCharges = new oprServiceCharges();
                        serviceCharges = b;

                        _repooprServiceCharge.Update(serviceCharges);

                        int SaveServiceChg = await _ApplicationDbContext.SaveChanges(p.UserId);

                        if (SaveServiceChg > 0)
                            successServiceCharge++;

                    }

                    foreach (var i in p.ListoprDocChgDetail)
                    {
                        
                        var getRec = await  _repoprDocChgDetail.GetAsync(c=> c.ItbId ==i.ItbId );
                         getRec.DocumentId  = i.DocumentId;
                         getRec.ServiceId  = i.ServiceId;
                         getRec.ServiceItbId  = i.ItbId;
                         getRec.Description  = i.Description;
                         getRec.ChargeRate  = i.ChargeRate;
                         getRec.Qty  = i.Qty;
                         getRec.TotalCharge  = i.TotalCharge;
                        _repoprDocChgDetail.Update(getRec);

                        int chgDetails = await _ApplicationDbContext.SaveChanges(p.LoginUserId);
                        if (chgDetails > 0)
                            successAddChgDetails++;
                    }

                    if (rev > 0 && successAddChgDetails == numberOfprDocChgDetail && serviceChargeCount == successServiceCharge)
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
        public async Task<IActionResult> GetById(DocumentRetrievalDTO p)
        {
            try
            {


                var serviceChargeslist = new List<oprServiceCharges>();
                var instrumentDetails = await _repooprDocRetrieval.GetAsync(c => c.ItbId == p.oprDocRetrieval.ItbId);


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

                    var admDocumentChg = await _repoprDocChgDetail.GetManyAsync(c => c.ServiceId == p.ServiceId && c.ServiceItbId == instrumentDetails.ItbId);

                    var getAlladmDocumentChg = await _repoadmDocumentChg.GetManyAsync(c=> c.DocumentId > 0);

                    var lisadmDoc = new List<admDocumentChgTemp>();
                    var lisadmDoc1 = new List<admDocumentChgTemp>();

                    foreach(var b in admDocumentChg)
                    {
                        var getOnlyOnlyOne = await _repoadmDocumentChg.GetAsync(c=> c.DocumentId  == b.DocumentId);
                        lisadmDoc.Add(new admDocumentChgTemp(){
                                ItbId = b.ItbId,
                                DocumentId = b.DocumentId,
                                ServiceId = b.ServiceId,
                                Description = b.Description,
                                ChgMetrix = getOnlyOnlyOne.ChgMetrix,
                                ChgBasis = getOnlyOnlyOne.ChgBasis,
                                PeriodStart = getOnlyOnlyOne.PeriodStart,
                                PeriodEnd = getOnlyOnlyOne.PeriodEnd,
                                ChgAmount = b.ChargeRate,
                                CcyCode = getOnlyOnlyOne.CcyCode,
                                Total = b.TotalCharge,
                                Qty = b.Qty
                                
                        });
                    } 

                    foreach(var b in getAlladmDocumentChg)
                    {
                          var getOnlyOnlyOne = await _repoadmDocumentChg.GetAsync(c=> c.DocumentId  == b.DocumentId);
                        lisadmDoc1.Add(new admDocumentChgTemp(){
                                ItbId = null,
                                DocumentId = b.DocumentId,
                                ServiceId = b.ServiceId,
                                Description = b.Description,
                                ChgMetrix = getOnlyOnlyOne.ChgMetrix,
                                ChgBasis = getOnlyOnlyOne.ChgBasis,
                                PeriodStart = getOnlyOnlyOne.PeriodStart,
                                PeriodEnd = getOnlyOnlyOne.PeriodEnd,
                                ChgAmount = b.ChgAmount,
                                CcyCode = getOnlyOnlyOne.CcyCode,
                                Total = null,
                                 Qty = null
                        });

                    }

                   var ggg = lisadmDoc.Concat(lisadmDoc1);

                   var result = ggg.GroupBy(test => test.DocumentId)
                   .Select(grp => grp.First())
                   .ToList();

                   

                    var res = new
                    {
                        instrumentDetails = instrumentDetails,
                        serviceChargeslist = serviceChargeslist,
                        allUsers = allUsers,
                        valInstrumentAcct = valInstrumentAcct,
                        admDocumentChg = result
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