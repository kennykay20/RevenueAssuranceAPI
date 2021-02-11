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
    public class AuthTransController : ControllerBase
    {
         IConfiguration _configuration;
        ApiResponse ApiResponse = new ApiResponse();
        TokenGenerator TokenGenerator;  
        AppSettingsPath AppSettingsPath ;
         IDbConnection db = null;
         ApplicationDbContext   _ApplicationDbContext;

         Formatter _Formatter =  new Formatter();
         ChargeImplementation _ChargeImplementation;
         ServiceChargeImplementation _ServiceChargeImplementation;
          IRepository<oprServiceCharges> _repooprServiceCharge;
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
        public AuthTransController(     IConfiguration configuration,
                                        ApplicationDbContext   ApplicationDbContext,
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
                                         IRepository<CbsTransaction> repoCbsTransaction) 
        {
            _configuration = configuration;   
            AppSettingsPath = new AppSettingsPath(_configuration);
            TokenGenerator = new TokenGenerator(_configuration);
            db = new SqlConnection(AppSettingsPath.GetDefaultCon());
            _ApplicationDbContext =  ApplicationDbContext;
            _ChargeImplementation = ChargeImplementation;
            _ServiceChargeImplementation = ServiceChargeImplementation;
            _repooprServiceCharge = repooprServiceCharge;
            _repoprStatementReq = repoprStatementReq;
            _repoadmCharge = repoadmCharge;
            _repoadmBankBranch = repoadmBankBranch;
            _AccountValidationImplementation = AccountValidationImplementation;
            _ApplicationReturnMessageImplementation = ApplicationReturnMessageImplementation;
            _ComputeChargesImplementation  = ComputeChargesImplementation;
            _UsersImplementation = UsersImplementation;
            _HeaderLogin =  HeaderLogin;
            _RoleAssignImplementation = RoleAssignImplementation;
            _repoadmService = repoadmService;
            _CBSTransImplementation = CBSTransImplementation;
            _CoreBankingImplementation = CoreBankingImplementation;
            _ApprovalValidation = ApprovalValidation;
            _ApplicationReturnMessage = ApplicationReturnMessage;
            _repoadmClientProfile = repoadmClientProfile;
            _repoadmBankServiceSetup = repoadmBankServiceSetup;
            _repoCbsTransaction = repoCbsTransaction;
        }
  
        [HttpPost("GetAll")]
        public async Task<IActionResult> GetAll(ParamLoadPage AnyAuth)
        {
            try
            {
                var RoleAssign = await _RoleAssignImplementation.GetRoleAssign(AnyAuth.MenuId,AnyAuth.RoleId);
                bool IsGlobalView = false;
                if(RoleAssign != null)
                {
                    IsGlobalView = RoleAssign.IsGlobalSupervisor == true ?  true : false; 
                }

                DynamicParameters param = new DynamicParameters();

                param.Add("@pnUserId", AnyAuth.UserId);
                param.Add("@pnDeptId", AnyAuth.pnDeptId);
                param.Add("@psbranchCode", AnyAuth.psBranchNo);
                param.Add("@psBranches", AnyAuth.psBranchNo);
                param.Add("@psIsGlobalSupervisor", IsGlobalView);

                var rtn = new DapperDATAImplementation<PostingObject>();
                var _response = await rtn.LoadData("Isp_FetchAuthorizationList", param, db);
                if(_response != null)
                {
                    var res = new {
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
                 ApiResponse.ResponseMessage =  ex == null ? ex.InnerException.Message : ex.Message;
                
                 ApiResponse.ResponseCode = -99;
                 return BadRequest(ApiResponse); 
            }
        }

        [HttpPost("AuthTrans")]
        public async Task<IActionResult> AuthPostTransaction(PostTransDTO p)
        {
            int success = 0;
            int failure = 0;
            var apiResponse = new ApiResponse();
               var SubtringClass = new List<SubtringClassDTO>();
            StringBuilder sb = new StringBuilder("<ul>");
            string styleWarning = @"""color: red""";
            string styleSuccess  = @"""color: green""";

            string tranIds = string.Empty, Ids = string.Empty; 

             foreach(var b in p.ListCbsTransactionDTO)
                {
                        Ids +=  b.ItbId +",";
                }

                  tranIds = _Formatter.RemoveLast(Ids);

           
            string PostedStatus = _configuration["Statuses:PostedStatus"];  
            if (!string.IsNullOrEmpty(tranIds))
            {
                try
                {
                    var profile = await _repoadmClientProfile.GetAsync(null);

                     var rtn = new DapperDATAImplementation<CbsTransaction>();

                    string script = $"select * from CbsTransaction where itbid in ({tranIds})";
                
                    var transactions = await rtn.LoadListNoParam(script, db);
                    
                   // var SubtringClass = new List<SubtringClassDTO>();
                    int  tracerCountSucc = 0;
                    foreach (var k in transactions)
                    {
                        var i = await _repoCbsTransaction.GetAsync(c=> c.ItbId == k.ItbId);
                        var get = await  _CBSTransImplementation.GetTransTrancer(i.TransTracer, p.UserId);
                        int getCountTransTracer = get.Count;
                        int NotChkFound = 0;
                        string NotSelectString = string.Empty;
                        int numberSelected = 0;
                        foreach (var b in get)
                        {
                            if (tranIds.Contains(b.ItbId.ToString()))
                            {
                                numberSelected += 1;
                                continue;
                            }
                            else
                            {
                                NotChkFound = 1;
                                continue;
                            }
                        }

                        if (NotChkFound == 1)
                        {

                            string style1 = @"""color:red""";
                            string style2 = @"""color: green""";
                            sb.Append($"<li><b>Note:</b> Trans Tracer:<b> { i.TransTracer } </b>. This Transaction is Batch of transactions, kindly make sure they are all selected <b style = {style1}> Selected Count: { numberSelected }</b>. <b style ={style2}>Trans Count: { getCountTransTracer }</b> </li>");

                          
                            break;
                        }
                      //  i.TransReference = i.TransReference + "A8"; //hard code
                        if (!string.IsNullOrEmpty(i.DrAcctNo))
                        {
                            try
                            {
                                var checkTransExit = await _CBSTransImplementation.ChectTransRefExist(i.ItbId, i.TransReference);
                                
                                if (checkTransExit.nReturnCode == -1)
                                {
                                    sb.Append($"<li><b style = {styleWarning}> Failed:</b> Trans. Ref: {i.TransReference} Failed. Reason: { checkTransExit.sReturnMsg } </li>");

                                    failure++;
                                    apiResponse.ResponseCode = checkTransExit.nReturnCode;
                                    apiResponse.ResponseMessage = checkTransExit.sReturnMsg;
                                    continue;
                                }
                                var postResp = await _CBSTransImplementation.PostTransactionsEthix(i, p.LoginUserName);
                                if (postResp != null)
                                {
                                    tracerCountSucc += 1;
                                    if (postResp.nErrorCode == 0)
                                    {
                                        i.PostingErrorCode = postResp.nErrorCode;
                                        i.PostingErrorDescr = postResp.sErrorText;
                                        i.UserId = p.UserId;
                                        i.CbsUserId = p.LoginUserName;
                                        i.DrAcctCashAmt = !string.IsNullOrEmpty(postResp.drCashAmt) ? decimal.Parse(postResp.drCashAmt) : 0.00m;
                                        i.CrAcctCbsTranId = _Formatter.ValDecimal(postResp.nCbsTranId);
                                        i.DrAcctBalAfterPost = !string.IsNullOrEmpty(postResp.nBalance) ? decimal.Parse(postResp.nBalance) : 0.00m;
                                        i.Status = PostedStatus;
                                        i.PostingDate = DateTime.Now;
                                        _repoCbsTransaction.Update(i);
                                        int SaveA = await _ApplicationDbContext.SaveChanges(p.UserId);

                                        var update = await _CBSTransImplementation.CbsTransUpdate(i, p.UserId);
                                        success++;
                                        //UpdatePrimaryTBL(int ServiceId, string status, decimal? PrimaryId, int UserId)
                                        var  updatePrimary = await _CBSTransImplementation.UpdatePrimaryTBL((int)i.ServiceId, PostedStatus, i.PrimaryId, p.UserId);
                                        if(updatePrimary.ResponseCode == 0)
                                        continue;

                                    }
                                    else
                                    {
                                        var deleteTransRef = await _CBSTransImplementation.ChectTransRefExist(i.ItbId, i.TransReference);

                                        i.PostingErrorCode = postResp.nErrorCode;
                                        i.PostingErrorDescr = postResp.sErrorText;
                                        apiResponse.ResponseCode = postResp.nErrorCode;
                                        apiResponse.ResponseMessage = postResp.sErrorText;

                                        var update = await _CBSTransImplementation.CbsTransUpdate(i, p.UserId);
                                        sb.Append($"<li> <b style = {styleWarning}> Failed:</b> Tracer Reference: { i.TransTracer }  Post Trans Message: {postResp.sErrorText}</li>");
                                        failure++;
                                        continue;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                var exM = ex;
                                failure++;
                                //LogManager.SaveLog(ex.Message == null ? ex.InnerException.ToString() : ex.Message.ToString());
                                continue;
                            }
                        }
                    }

                 if(tracerCountSucc == transactions.Count())
                 {
                    var SubtringClass1 = new List<SubtringClassDTO>();
                     foreach(var b in SubtringClass) {

                    var getDefault = SubtringClass1.FirstOrDefault(c => c.TransTracer == b.TransTracer);
                    if(getDefault == null)
                    {
                         var getAllDefault = SubtringClass.Where(c => c.TransTracer == b.TransTracer);
                        
                        sb.Append($"<li> Success: Trans Tracer: { b.TransTracer } was Successful! Trans Count: { getAllDefault.Count() } </li>");

                         SubtringClass1.Add(new SubtringClassDTO { TransTracer = b.TransTracer });
                    }
                }

                 sb.Append("</ul>");
                 apiResponse.ResponseMessage = sb.ToString();
              
                return Ok(apiResponse);
            }


                }
                catch (Exception ex)
                {
                    var exM = ex == null ? ex.InnerException.Message : ex.Message;
                }
            }
            else
            {
                apiResponse.ResponseCode = -2;
                apiResponse.ResponseMessage = "No Transaction(s) were no Selected";
                sb.Append($"<li>{apiResponse.sErrorText}</li>");
                return BadRequest(apiResponse);
            }

         
            sb.Append("</ul>");
            apiResponse.StringbuilderMessage = sb.ToString();
            apiResponse.ResponseMessage = sb.ToString();
            return Ok(apiResponse);
        }

          [HttpPost("RejectTrans")]
        public async Task<IActionResult> RejectTrans(PostTransDTO p)
        {
            try
            {
                string RejectedStatus =  _configuration["Statuses:RejectedStatus"];

                string Ids = string.Empty,  tranIds = string.Empty, IdsRej = string.Empty;

               
                
                foreach(var b in p.ListRejectionReasonDTO)
                {
                        IdsRej +=  b.ItbId +",";
                }

               string RejIds = _Formatter.RemoveLast(IdsRej);
                var ApiResponse = new ApiResponse();

               
                foreach(var b in p.ListCbsTransactionDTO)
                {
                        Ids +=  b.ItbId +",";
                }

                tranIds = _Formatter.RemoveLast(Ids);

                string[]  splitTransId  = tranIds.Split(",");

                foreach(var b in splitTransId){
                    Int64 itb = Convert.ToInt64(b);
                    var get = await _repoCbsTransaction.GetAsync(c=> c.ItbId == itb);
                    if(get != null){
                        get.Status = RejectedStatus;
                        get.Rejected = "Y";
                        get.RejectedBy = p.UserId;
                        get.RejectionDate = DateTime.Now;

                        _repoCbsTransaction.Update(get);

                        ApiResponse.ResponseCode = await _ApplicationDbContext.SaveChanges(p.UserId);

                    }
                }

              
                if (ApiResponse.ResponseCode > 0)
                {
                    ApiResponse.ResponseCode =   0;
                    ApiResponse.sErrorText=  "Rejected Successfully!";
                        
                    return Ok(ApiResponse);
                }
        }
        catch (Exception ex)
        {
                ApiResponse.ResponseMessage =  ex == null ? ex.InnerException.Message : ex.Message;
            
                ApiResponse.ResponseCode = -99;
                return BadRequest(ApiResponse); 
        }
        
            return BadRequest(ApiResponse); 
        }

        
        
        
        [HttpPost("GetById")]
        public async Task<IActionResult> GetById(PostTransDTO p)
        {
            try
            {
                var get = await _repoCbsTransaction.GetAsync(c=> c.ItbId ==  p.ItbId);
                if(get != null)
                {
               
                    var allUsers = await _UsersImplementation.GetAllUser(get.UserId, get.RejectedBy, 0);
                    var res = new {
                        get = get,
                        allUsers = allUsers,
                    };

                    return Ok(res);
                 }
            }
            catch(Exception ex)
            {
                var exM = ex;
               
            }
                return BadRequest();

        }

       

    }
}