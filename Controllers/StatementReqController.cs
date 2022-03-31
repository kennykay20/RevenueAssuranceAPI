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
    public class StatementReqController : ControllerBase
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
           IRepository<admService> _repoadmService;

          IRepository<admCharge> _repoadmCharge;

          IRepository<admBankBranch> _repoadmBankBranch;
           AccountValidationImplementation _AccountValidationImplementation;
           ApplicationReturnMessageImplementation _ApplicationReturnMessageImplementation;
           ComputeChargesImplementation _ComputeChargesImplementation;
           UsersImplementation _UsersImplementation;
           HeaderLogin _HeaderLogin;
            RoleAssignImplementation _RoleAssignImplementation;

        public StatementReqController(
                                        IConfiguration configuration,
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
                                           IRepository<oprStatementReq> repoprStatementReq
                                          ) 
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
        }
   
        [HttpPost("GetAll")]
        public async Task<IActionResult> GetAll(ParamLoadPage AnyAuth)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();

                var RoleAssign = await _RoleAssignImplementation.GetRoleAssign(AnyAuth.MenuId,AnyAuth.RoleId);
                string IsGlobalView = "N";
                if(RoleAssign != null)
                {
                    IsGlobalView = RoleAssign.IsGlobalSupervisor == true ?  "Y" : "N"; 
                }

                param.Add("@pdtCurrentDate", _Formatter.FormatToDateYearMonthDay(AnyAuth.pdtCurrentDate));
                param.Add("@psBranchNo", AnyAuth.psBranchNo); 
                param.Add("@pnDeptId", AnyAuth.pnDeptId);
                param.Add("@pnGlobalView", IsGlobalView);
                param.Add("@ServiceId", AnyAuth.ServiceId);
              
                var rtn = new DapperDATAImplementation<oprStatementReqDTO>();
                
                var _response = await rtn.LoadData("isp_GetTradeRef", param, db);

                if(_response != null)
                {
                   var getCharge = await _ChargeImplementation.GetChargeDetails(AnyAuth.ServiceId);

                 
                   var res = new {
                       _response = _response,
                       charge = getCharge,
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

      
        [HttpPost("Add")]
        public async Task<IActionResult> Add(StatementReqDTO p)
        {
            try
            {
                string UnauthorizedStatus =  _configuration["Statuses:UnauthorizedStatus"];

                oprStatementReq _oprStatementReq = new oprStatementReq();

                _oprStatementReq.InstrumentStatus  = UnauthorizedStatus;
                _oprStatementReq = p.oprStatementReq;
                _oprStatementReq.DateCreated= DateTime.Now;

                await _repoprStatementReq.AddAsync(_oprStatementReq);
                int rev = await _ApplicationDbContext.SaveChanges(p.UserId);

                int SeqNo = 0;
            
                foreach(var b in p.ListoprServiceCharge)
                {
                    SeqNo += 1;
                    b.Status = UnauthorizedStatus;
                    b.DateCreated= DateTime.Now;
                    b.SeqNo = SeqNo;
                    var SaveServiceChg = await _ServiceChargeImplementation.SaveServiceCharges(b, p.UserId);
                }

            if (rev > 0)
            {
                ApiResponse.ResponseCode =   0;
                ApiResponse.sErrorText=  "Processed Successfully";
                    
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

        [HttpPost("Update")]
        public async Task<IActionResult> Update(StatementReqDTO p)
        {
            try
            {
                   int revToken = -1;
                   int itbId  = p.oprStatementReq.ItbId;

                   var get = await _repoprStatementReq.GetAsync(c=> c.ItbId == itbId);

                   if(get != null)
                   {
                       get.AcctNo = p.oprStatementReq.AcctNo;
                       //Do for others
                      _repoprStatementReq.Update(get);

                      revToken =   await    _ApplicationDbContext.SaveChanges(p.UserId);
                   } 


                    int upServiceCharge = -1;

                     foreach(var b in p.ListoprServiceCharge)
                     {
                        var getSevice = await  _repooprServiceCharge.GetAsync(c=> c.ServiceId == p.oprStatementReq.ServiceId 
                        && c.ServiceItbId == p.oprStatementReq.ItbId);
              
                        _repooprServiceCharge.Update(getSevice);
                        upServiceCharge =   await  _ApplicationDbContext.SaveChanges(p.UserId);
                        
                     }
                    if(revToken > 0) {

                        if (revToken > 0)
                            {
                            
                                ApiResponse.ResponseCode =   0;
                                ApiResponse.sErrorText=  "Record Updated Successfully";
                                    
                                return Ok(ApiResponse);
                            }
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
        public async Task<IActionResult> GetById(StatementReqDTO p)
        {
            try
            {
            
                var get = await _repoprStatementReq.GetAsync(c=> c.ItbId ==  p.oprStatementReq.ItbId);
                if(get != null)
                {
                    var getSer = await  
                _ServiceChargeImplementation.GetServiceChargesByServIdAndItbId(get.ServiceId, get.ItbId);
                      var allUsers = await _UsersImplementation.GetAllUser(get.UserId, get.RejectedBy, get.DismissedBy);
            var chargeSetUp = new List<admCharge>();
                if(getSer.Count() == 0)
                {
                     chargeSetUp = await  _ChargeImplementation.GetChargeDetails(get.ServiceId);
                }
                
                 var res = new {
                     get = get,
                     getSer = getSer,
                     allUsers = allUsers,
                     chargeSetUp = chargeSetUp
                     
                 };

                 return Ok(res);

                }
                else
                {
                   var  chargeSetUp = await  _ChargeImplementation.GetChargeDetails(p.ServiceId);
                   
                   var list = new List<oprServiceCharges>();

                   
                   foreach(var b in chargeSetUp)
                   {
                       list.Add(new oprServiceCharges
                       {
                            ServiceId = p.ServiceId,
                            ServiceItbId = 0,
                            ChgAcctNo = null, //b.ChargeAcctNo,
                            ChgAcctType = null ,//b.ChargeAcctType,
                            ChgAcctName = null,
                            ChgAvailBal = null,
                            ChgAcctCcy = null,// b.CurrencyIso,
                            ChgAcctStatus  = null,
                            ChargeCode  = b.ChargeCode,
                            ChargeRate  = null,
                            OrigChgAmount  = null,
                            OrigChgCCy  = null,
                            ExchangeRate  = null,
                            EquivChgAmount  = null,
                            EquivChgCcy  = null,
                            ChgNarration  = null,
                            TaxAcctNo  = null,
                            TaxAcctType  = null,
                            TaxRate = null,
                            TaxAmount  = null,
                            TaxNarration  = null,
                            IncBranch  = null,
                            IncAcctNo  = null,
                            IncAcctType  = null,
                            IncAcctName  = null,
                            IncAcctBalance  = null,
                            IncAcctStatus  = null,
                            IncAcctNarr  = null,
                            SeqNo  = null,
                            Status  = null,
                            DateCreated  = null,

                       }); 
                   }
                    
                   
                    var res = new {
                     chargeSetUp = list
                     
                 };

                 return Ok(res);

                }
                }
                catch(Exception ex)
                {
                    var exM = ex;
                    return BadRequest();
                }
               

        }
   

   
    
    }
}