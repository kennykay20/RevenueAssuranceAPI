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

namespace RevAssuranceApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [EnableCors("CorsPolicy")]
    //[Authorize]
    public class ApproveTransController : ControllerBase
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
        IRepository<oprStatementReq> _repoprStatementReq;
        IRepository<admService> _repoadmService;
        IRepository<admCharge> _repoadmCharge;
        IRepository<admBankBranch> _repoadmBankBranch;
        AccountValidationImplementation _AccountValidationImplementation;
        ApplicationReturnMessageImplementation _ApplicationReturnMessageImplementation;
        ComputeChargesImplementation _ComputeChargesImplementation;
        UsersImplementation _UsersImplementation;
        HeaderLogin _HeaderLogin;
        RoleAssignImplementation _RoleAssignImplementation;
        ApproveImplementation _ApproveImplementation;

        ////////
        IRepository<OprToken> _repoOprToken;
        IRepository<OprChqBookRequest> _repoOprChqBookRequest;
        IRepository<oprCounterChq> _repoOprCounterChq;
        IRepository<OprStopChqRequest> _repoOprStopChqRequest;
        IRepository<OprBusinessSearch> _repoOprBusinessSearch;
        IRepository<OprAcctClosure> _repoOprAcctClosure;
        IRepository<OprCard> _repoOprCard;
        IRepository<OprAcctStatment> _repoOprAcctStatment;
        IRepository<OprTradeReference> _repoOprTradeReference;
        IRepository<OprReferenceLetter> _repoOprReferenceLetter;
        IRepository<OprInstrument> _repoOprInstrument;
        IRepository<OprOverDraft> _repoOprOverDraft;
        IRepository<OprAmortizationSchedule> _repoOprAmortizationSchedule;
        IRepository<oprDocRetrieval> _repooprDocRetrieval;
        IRepository<oprPinReset> _repooprPinReset;
        ////

        public ApproveTransController(
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
                                        IRepository<oprStatementReq> repoprStatementReq,
                                        ApproveImplementation ApproveImplementation,
                                        //
                                        IRepository<OprToken> repoOprToken,
                                        IRepository<OprChqBookRequest> repoOprChqBookRequest,
                                        IRepository<oprCounterChq> repoOprCounterChq,
                                        IRepository<OprStopChqRequest> repoOprStopChqRequest,
                                        IRepository<OprBusinessSearch> repoOprBusinessSearch,
                                        IRepository<OprAcctClosure> repoOprAcctClosure,
                                        IRepository<OprCard> repoOprCard,
                                        IRepository<OprAcctStatment> repoOprAcctStatment,
                                        IRepository<OprTradeReference> repoOprTradeReference,
                                        IRepository<OprReferenceLetter> repoOprReferenceLetter,
                                        IRepository<OprOverDraft> repoOprOverDraft,
                                        IRepository<OprAmortizationSchedule> repoOprAmortizationSchedule,
                                        IRepository<oprDocRetrieval> repooprDocRetrieval,
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
            _ApproveImplementation = ApproveImplementation;

            ///
            _repoOprToken = repoOprToken;
            _repoOprChqBookRequest = repoOprChqBookRequest;
            _repoOprCounterChq = repoOprCounterChq;
            _repoOprStopChqRequest = repoOprStopChqRequest;
            _repoOprBusinessSearch = repoOprBusinessSearch;
            _repoOprAcctClosure = repoOprAcctClosure;
            _repoOprCard = repoOprCard;
            _repoOprAcctStatment = repoOprAcctStatment;
            _repoOprTradeReference = repoOprTradeReference;
            _repoOprReferenceLetter = repoOprReferenceLetter;
            _repoOprInstrument = repoOprInstrument;
            _repoOprOverDraft = repoOprOverDraft;
            _repoOprAmortizationSchedule = repoOprAmortizationSchedule;
            _repooprDocRetrieval = repooprDocRetrieval;
            _repooprPinReset = repooprPinReset;
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

            var rtn = new DapperDATAImplementation<GetApproveServiceDTO>();

            var _response = await rtn.LoadData("isp_GetAppoveService", param, db);

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

    [HttpPost("ApproveTrans")]
    public async Task<IActionResult> ApproveTrans(PostTransDTO p)
    {
        try
        {
            string Ids = string.Empty, tranIds = string.Empty;
            int Success = 0;
            foreach (var b in p.ListGetApproveServiceDTO)
            {    
                    var rtn = new DapperDATAImplementation<CbsTransInsertBatchDTO>();
                    DynamicParameters param = new DynamicParameters();

                    string TransDate = _Formatter.FormatToDateYearMonthDay(p.TransactionDate);

                    param.Add("@pnServiceId", b.ServiceId);
                    param.Add("@pnServiceItbId", b.ServiceItbId);
                    param.Add("@pdtTransnDate", TransDate);

                    var Batchitems = await rtn.LoadData("Isp_CbsTransInsert", param, db);

                    if (Batchitems.Count() > 0)
                        Success++;
            }

            var ApiResponse = new ApiResponse();
            if (Success == p.ListGetApproveServiceDTO.Count())
            {
                ApiResponse.ResponseMessage = "Approved Successfully!";
                return Ok(ApiResponse);

            }
            if (Success == 0)
            {
                ApiResponse.ResponseMessage = "Non of the Transaction(s) was Approved!";
                return BadRequest(ApiResponse);
            }
            
            ApiResponse.ResponseMessage = "Approved Was not Successfully!";
            return BadRequest(ApiResponse);

        }
        catch (Exception ex)
        {
            ApiResponse.ResponseMessage = "Error Occured!"; // ex == null ? ex.InnerException.Message : ex.Message;

            ApiResponse.ResponseCode = -99;
            return BadRequest(ApiResponse);
        }
    }


    [HttpPost("RejectTrans")]
    public async Task<IActionResult> RejectTrans(PostTransDTO p)
    {
        try
        {
            string PostedStatus = _configuration["Statuses:PostedStatus"];

            string Ids = string.Empty, tranIds = string.Empty, IdsRej = string.Empty;

            foreach (var b in p.ListGetApproveServiceDTO)
            {
                Ids += b.ItbId + ",";
            }

            tranIds = _Formatter.RemoveLast(Ids);


            foreach (var b in p.ListRejectionReasonDTO)
            {
                IdsRej += b.ItbId + ",";
            }

            string RejIds = _Formatter.RemoveLast(IdsRej);

            var dappertbl = new DapperDATAImplementation<oprServiceCharges>();
            string scr = $"select * from oprServiceCharges where ServiceItbId in ({tranIds})";
            List<oprServiceCharges> data = await dappertbl.LoadListNoParam(scr, db);

            var ApiResponse = new ApiResponse();
            if (data.Count > 0)
            {
                ApiResponse = await _ApproveImplementation.RejAllChecked(data, p.LoginUserName, p.UserId, RejIds);

            }
            if (ApiResponse.ResponseCode == 0)
            {
                ApiResponse.ResponseCode = 0;
                ApiResponse.sErrorText = "Rejected Successfully!";

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

    [HttpPost("Update")]
    public async Task<IActionResult> Update(StatementReqDTO p)
    {
        try
        {
            int revToken = -1;
            int itbId = p.oprStatementReq.ItbId;

            var get = await _repoprStatementReq.GetAsync(c => c.ItbId == itbId);

            if (get != null)
            {
                get.AcctNo = p.oprStatementReq.AcctNo;
                //Do for others
                _repoprStatementReq.Update(get);

                revToken = await _ApplicationDbContext.SaveChanges(p.UserId);
            }


            int upServiceCharge = -1;

            foreach (var b in p.ListoprServiceCharge)
            {
                var getSevice = await _repooprServiceCharge.GetAsync(c => c.ServiceId == p.oprStatementReq.ServiceId
               && c.ServiceItbId == p.oprStatementReq.ItbId);

                _repooprServiceCharge.Update(getSevice);
                upServiceCharge = await _ApplicationDbContext.SaveChanges(p.UserId);

            }
            if (revToken > 0)
            {

                if (revToken > 0)
                {

                    ApiResponse.ResponseCode = 0;
                    ApiResponse.sErrorText = "Record Updated Successfully";

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
    public async Task<IActionResult> GetById(StatementReqDTO p)
    {
        try
        {

            var get = await _repoprStatementReq.GetAsync(c => c.ItbId == p.oprStatementReq.ItbId);
            if (get != null)
            {
                var getSer = await
            _ServiceChargeImplementation.GetServiceChargesByServIdAndItbId(get.ServiceId, get.ItbId);
                var allUsers = await _UsersImplementation.GetAllUser(get.UserId, get.RejectedBy, get.DismissedBy);
                var chargeSetUp = new List<admCharge>();
                if (getSer.Count() == 0)
                {
                    chargeSetUp = await _ChargeImplementation.GetChargeDetails(get.ServiceId);
                }

                var res = new
                {
                    get = get,
                    getSer = getSer,
                    allUsers = allUsers,
                    chargeSetUp = chargeSetUp

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

}
}