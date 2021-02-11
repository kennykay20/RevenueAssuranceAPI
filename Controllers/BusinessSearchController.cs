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
    public class BusinessSearchController : ControllerBase
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
          IRepository<OprBusinessSearch> _repoOprBusinessSearch;
           IRepository<admService> _repoadmService;

          IRepository<admCharge> _repoadmCharge;

          IRepository<admBankBranch> _repoadmBankBranch;
           AccountValidationImplementation _AccountValidationImplementation;
           ApplicationReturnMessageImplementation _ApplicationReturnMessageImplementation;
           ComputeChargesImplementation _ComputeChargesImplementation;
           UsersImplementation _UsersImplementation;
           HeaderLogin _HeaderLogin;
            RoleAssignImplementation _RoleAssignImplementation;

        public BusinessSearchController( IConfiguration configuration,
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
                                         IRepository<OprBusinessSearch> repoOprBusinessSearch
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
            _repoOprBusinessSearch = repoOprBusinessSearch;
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
                param.Add("@pnDeptId", AnyAuth.pnDeptId);
                param.Add("@pnGlobalView", IsGlobalView);

              
                var rtn = new DapperDATAImplementation<OprBusinessSearchDTO>();
                
                var _response = await rtn.LoadData("isp_GetBusinessSearch", param, db);

                if(_response != null)
                {
                   var getCharge = await _ChargeImplementation.GetChargeDetails(AnyAuth.ServiceId);

                    var admService = await _repoadmService.GetAsync(c=> c.ServiceId == AnyAuth.ServiceId);
                    
                 
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
                if(!string.IsNullOrEmpty(AnyAuth.psBranchNo))
                {
                    param.Add("@psBranchNo", Convert.ToInt32(AnyAuth.psBranchNo));
                }
                if(!string.IsNullOrEmpty(AnyAuth.pnDeptId))
                {
                    param.Add("@psDeptId", Convert.ToInt32(AnyAuth.pnDeptId));
                }
                if(!string.IsNullOrEmpty(AnyAuth.psStatus))
                {
                    param.Add("@psStatus", AnyAuth.psStatus);
                }
                if(!string.IsNullOrEmpty(AnyAuth.referenceNo) || !string.IsNullOrWhiteSpace(AnyAuth.referenceNo))
                {
                    param.Add("@referenceNo", AnyAuth.referenceNo);
                }
                if(!string.IsNullOrEmpty(AnyAuth.psCcyCode))
                {
                    param.Add("@psCcyCode", AnyAuth.psCcyCode);
                }
                
                if(!string.IsNullOrEmpty(AnyAuth.AvailBal))
                {
                    param.Add("@AvailBal", Convert.ToDecimal(AnyAuth.AvailBal));
                }

                if(!string.IsNullOrEmpty(AnyAuth.psAcctNo))
                {
                    param.Add("@AcctNo", AnyAuth.psAcctNo);
                }

                if(!string.IsNullOrEmpty(AnyAuth.AccountName))
                {
                    param.Add("@AccountName", AnyAuth.AccountName);
                }
                
                if(!string.IsNullOrEmpty(AnyAuth.psAcctType)){
                    param.Add("@AcctType", AnyAuth.psAcctType);
                }
                if(!string.IsNullOrEmpty(AnyAuth.ServiceId.ToString()))
                {
                    param.Add("@ServiceId", Convert.ToInt32(AnyAuth.ServiceId));
                }
                if(!string.IsNullOrEmpty(AnyAuth.pnDeptId)){
                    param.Add("@psDeptId", Convert.ToInt32(AnyAuth.pnDeptId));
                }
                // param.Add("@pnGlobalView", IsGlobalView);
                
                
              
                var rtn = new DapperDATAImplementation<OprBusinessSearchDTO>();
                
                var _response = await rtn.LoadData("Isp_GetBusinessSearchHis", param, db);

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
        public async Task<IActionResult> Add(BusinessSearchDTO p)
        {
            try
            {
                int SaveServiceChg = -1;

                string UnauthorizedStatus =  _configuration["Statuses:UnauthorizedStatus"];

                OprBusinessSearch _oprBusinessSearch = new OprBusinessSearch();
                _oprBusinessSearch = p.OprBusinessSearch;


                _oprBusinessSearch.ServiceStatus  = UnauthorizedStatus;
                
               var serviceRef = await _ComputeChargesImplementation.GenServiceRef(p.ServiceId);
                _oprBusinessSearch.ReferenceNo = serviceRef.nReference;
                _oprBusinessSearch.DateCreated= DateTime.Now;

                await _repoOprBusinessSearch.AddAsync(_oprBusinessSearch);
               int rev = await _ApplicationDbContext.SaveChanges(p.UserId);

                int SeqNo = 0;
            
                foreach(var b in p.ListoprServiceCharge)
                {
                
                        SeqNo += 1;
                        b.ServiceItbId =  _oprBusinessSearch.ItbId;
                        b.ServiceId  = p.ServiceId;
                        b.Status = null;
                        b.DateCreated= DateTime.Now;
                        var nar =  b.ChgNarration +" Ref: "+ serviceRef.nReference;
                        b.ChgNarration = nar;
                        var taxnar =  b.TaxNarration +" Ref: "+ serviceRef.nReference;
                         b.TaxNarration = taxnar;
                        b.SeqNo = SeqNo;

                         var Innar =  b.IncAcctNarr +" Ref: "+ serviceRef.nReference;

                         b.IncAcctNarr = Innar;
                         SaveServiceChg = await _ServiceChargeImplementation.SaveServiceCharges(b, (int)p.LoginUserId);
                    
                }

            if (rev > 0)
            {
                ApiResponse.ResponseCode =   0;
                ApiResponse.ResponseMessage=  "Processed Successfully";
                    
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
        public async Task<IActionResult> Update(BusinessSearchDTO p)
        {
            try
            {
                int SaveServiceChg = -1;

                string UnauthorizedStatus =  _configuration["Statuses:UnauthorizedStatus"];

                OprBusinessSearch _oprBusinessSearch = new OprBusinessSearch();
                _oprBusinessSearch = p.OprBusinessSearch;

                _repoOprBusinessSearch.Update(_oprBusinessSearch);
               int rev = await _ApplicationDbContext.SaveChanges(p.LoginUserId);

                foreach(var b in p.ListoprServiceCharge)
                {
                    // oprServiceCharges b1 = new oprServiceCharges();
                     _repooprServiceCharge.Update(b);
                    SaveServiceChg= await _ApplicationDbContext.SaveChanges(p.LoginUserId);
                }

            if (rev > 0)
            {
                ApiResponse.ResponseCode =   0;
                ApiResponse.ResponseMessage=  "Processed Successfully";
                    
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
        public async Task<IActionResult> GetById(BusinessSearchDTO p)
        {
            try
            {
                var serviceChargeslist = new List<oprServiceCharges>();
                var instrumentDetails = await _repoOprBusinessSearch.GetAsync(c=> c.ItbId ==  p.OprBusinessSearch.ItbId);
               

                AccountValParam AccountValParam = new AccountValParam();

                    AccountValParam.AcctNo = instrumentDetails.AcctNo;
                    AccountValParam.AcctType = instrumentDetails.AcctType == null ? null : instrumentDetails.AcctType.Trim();
                    
                    AccountValParam.CrncyCode = instrumentDetails.CcyCode;
                    AccountValParam.Username = "System";
                    
                    var valInstrumentAcct = await _AccountValidationImplementation.ValidateAccountCall(AccountValParam);
                    
               
               
               
               
                if(instrumentDetails != null)
                {
                    serviceChargeslist = await _ServiceChargeImplementation.GetServiceChargesByServIdAndItbId(instrumentDetails.ServiceId, (int)instrumentDetails.ItbId);
                    var allUsers = await _UsersImplementation.GetAllUser(instrumentDetails.UserId, instrumentDetails.RejectedBy, instrumentDetails.DismissedBy);
                    
                    var chargeSetUp = new List<admCharge>();

                if(serviceChargeslist.Count() == 0)
                {
                     chargeSetUp = await  _ChargeImplementation.GetChargeDetails(instrumentDetails.ServiceId);
                    
                
                
                foreach(var b in chargeSetUp)
                   {
                       serviceChargeslist.Add(new oprServiceCharges
                       {
                            ServiceId = p.ServiceId,
                            ServiceItbId = 0,
                            ChgAcctNo = null, 
                            ChgAcctType = null ,
                            ChgAcctName = null,
                            ChgAvailBal = null,
                            ChgAcctCcy = null,
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
                }
                
                 var res = new {
                     instrumentDetails = instrumentDetails,
                     serviceChargeslist = serviceChargeslist,
                     allUsers = allUsers,
                     valInstrumentAcct = valInstrumentAcct
                     
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
                catch(Exception ex){
                    var exM = ex;
                    return BadRequest();
                }
               

        }


   
    
    }
}