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
    public class StopChequeController : ControllerBase
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
          IRepository<OprStopChqRequest> _repoOprStopChqRequest;
           IRepository<admService> _repoadmService;

          IRepository<admCharge> _repoadmCharge;

          IRepository<admBankBranch> _repoadmBankBranch;
           AccountValidationImplementation _AccountValidationImplementation;
           ApplicationReturnMessageImplementation _ApplicationReturnMessageImplementation;
           ComputeChargesImplementation _ComputeChargesImplementation;
           UsersImplementation _UsersImplementation;
           HeaderLogin _HeaderLogin;
            RoleAssignImplementation _RoleAssignImplementation;

     
    

        public StopChequeController(
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
                                           IRepository<OprStopChqRequest> repoOprStopChqRequest
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
             _repoOprStopChqRequest = repoOprStopChqRequest;
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

                var rtn = new DapperDATAImplementation<OprStopChqRequestDTO>();
                
                var _response = await rtn.LoadData("isp_GetStopCheque", param, db);



            
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

        [HttpPost("Add")]
        public async Task<IActionResult> Add(StopChequeDTO p)
        {
            try
            {
                OprStopChqRequest oprStopChq = new OprStopChqRequest();
               
                oprStopChq = p.OprStopChqRequest;

                 string UnauthorizedStatus =   _configuration["Statuses:UnauthorizedStatus"];
                oprStopChq.ServiceStatus = UnauthorizedStatus;
                oprStopChq.DateCreated= DateTime.Now;
                oprStopChq.TransactionDate = Convert.ToDateTime(p.TransactionDate);
                oprStopChq.ValueDate = Convert.ToDateTime(p.ValueDate);
                
                var serviceRef = await _ComputeChargesImplementation.GenServiceRef(p.ServiceId);
                oprStopChq.ReferenceNo = serviceRef.nReference;
         

                await _repoOprStopChqRequest.AddAsync(oprStopChq);
               int rev = await _ApplicationDbContext.SaveChanges(p.LoginUserId);
               int SaveServiceChg = 0;
               int updateInstWithTempId = 0;

               if(rev > 0)
               {
                    int SeqNo = 0;
                    foreach(var b in p.ListoprServiceCharge)
                    {
                        SeqNo += 1;
                        b.ServiceItbId =  oprStopChq.ItbId;
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
                    if (rev > 0 && SaveServiceChg  > 0)
                    {
                        ApiResponse.ResponseCode =   0;
                        ApiResponse.ResponseMessage=  "Processed Successfully";
                            
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


        [HttpPost("Update")]
        public async Task<IActionResult> Update(StopChequeDTO p)
        {
            try
            {
                OprStopChqRequest OprStopChqRequest = new OprStopChqRequest();
               
                OprStopChqRequest = p.OprStopChqRequest;

                _repoOprStopChqRequest.Update(OprStopChqRequest);
               int rev = await _ApplicationDbContext.SaveChanges(p.LoginUserId);
               int SaveServiceChg = 0;
              

               if(rev > 0)
               {
                   
                    foreach(var b in p.ListoprServiceCharge)
                    {
                       
                          _repooprServiceCharge.Update(b);
                         SaveServiceChg =  await _ApplicationDbContext.SaveChanges(p.LoginUserId);
                    }
                 

                    if (rev > 0 && SaveServiceChg > 0 )
                    {
                        ApiResponse.ResponseCode =   0;
                        ApiResponse.ResponseMessage =  "Processed Successfully";
                            
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


         [HttpPost("CalCulateCharge")]
        public async Task<IActionResult> CalCulateCharge(StopChequeDTO p)
        {
            try
            {
                    AccountValParam AccountValParam = new AccountValParam();

                    AccountValParam.AcctNo = p.AcctNo;
                    AccountValParam.AcctType = p.AcctType == null ? null : p.AcctType.Trim();
                    
                    AccountValParam.CrncyCode = p.CcyCode;
                    AccountValParam.Username = p.LoginUserName;
                    
                    var InstrumentAcctDetails = await _AccountValidationImplementation.ValidateAccountCall(AccountValParam);

                    List<RevCalChargeModel> chgList = new List<RevCalChargeModel>();

                    foreach(var i in p.ListoprServiceCharge)
                    {
                            AccountValParam AccountVal = new AccountValParam();

                            AccountVal.AcctType = i.ChgAcctType == null ? InstrumentAcctDetails.sAccountType : i.ChgAcctType;
                            AccountVal.AcctNo = i.ChgAcctNo == null ? p.AcctNo : i.ChgAcctNo;
                            AccountVal.CrncyCode = InstrumentAcctDetails.sCrncyIso.Trim();
                            AccountVal.Username = p.LoginUserName;

                            var ChgDetails = await _AccountValidationImplementation.ValidateAccountCall(AccountVal);

                            OperationViewModel OperationViewModel = new OperationViewModel();

                            OperationViewModel.serviceId = p.ServiceId;
                            OperationViewModel.TransAmount = p.TransAmout;
                            OperationViewModel.InstrumentAcctNo = i.ChgAcctNo == null ? p.AcctNo : i.ChgAcctNo;
                            OperationViewModel.InstrumentCcy = InstrumentAcctDetails.sCrncyIso;
                            OperationViewModel.ChargeCode = i.ChargeCode;

                            var calCharge = await _ComputeChargesImplementation.CalChargeModel(OperationViewModel, 
                                                                                                ChgDetails.nBranch.ToString(), ChgDetails.sAccountType, ChgDetails.sCrncyIso);
                            calCharge.chgAcctNo	= AccountVal.AcctNo;
                            calCharge.chgAcctType = AccountVal.AcctType;
                            calCharge.chgAcctName = ChgDetails.sName;
                            calCharge.chgAvailBal = ChgDetails.nBalance;
                            calCharge.chgAcctCcy = ChgDetails.sCrncyIso;
                            calCharge.chgAcctStatus	= ChgDetails.sStatus;


                            AccountValParam AccountIncomeVal = new AccountValParam();

                            AccountIncomeVal.AcctType = calCharge.sChargeIncAcctType;
                            AccountIncomeVal.AcctNo = calCharge.sChargeIncAcctNo;
                            AccountIncomeVal.CrncyCode = calCharge.sChgCurrency;
                            AccountIncomeVal.Username = p.LoginUserName;



                            var IncomeDetails = await _AccountValidationImplementation.ValidateAccountCall(AccountIncomeVal);


                            calCharge.incBranch = IncomeDetails.nBranch;
                            calCharge.incBranchString = IncomeDetails.sBranchName;
                            calCharge.incAcctNo	= calCharge.sChargeIncAcctNo;
                            calCharge.incAcctType = calCharge.sChargeIncAcctType;
                            calCharge.incAcctName	= IncomeDetails.sName;
                            calCharge.incAcctBalance = IncomeDetails.nBalanceDec;
                            calCharge.incAcctBalanceString = IncomeDetails.nBalance;
                            calCharge.incAcctStatus	= IncomeDetails.sStatus;
                            calCharge.incAcctNarr = calCharge.sNarration;	
                            
                            calCharge.chargeCode = i.ChargeCode;
                            
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
                 ApiResponse.ResponseMessage =  ex == null ? ex.InnerException.Message : ex.Message;
                
                 ApiResponse.ResponseCode = -99;
                 return BadRequest(ApiResponse); 
            }
        }
    
        [HttpPost("GetById")]
        public async Task<IActionResult> GetById(StopChequeDTO p)
        {
            try
            {
            
                var get = await _repoOprStopChqRequest.GetAsync(c=> c.ItbId ==  p.OprStopChqRequest.ItbId);
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