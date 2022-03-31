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
    public class CardsController : ControllerBase
    {
         IConfiguration _configuration;
        ApiResponse ApiResponse = new ApiResponse();
        TokenGenerator TokenGenerator;  
        AppSettingsPath AppSettingsPath ;
         IDbConnection db = null;
         ApplicationDbContext   _ApplicationDbContext;

         Formatter _Formatter =  new Formatter();
         ChargeImplementation _ChargeImplementation;
         IRepository<admService> _repoadmService;
         ServiceChargeImplementation _ServiceChargeImplementation;
          IRepository<oprServiceCharges> _repooprServiceCharge;
          IRepository<OprCard> _repoOprCard;

          IRepository<admCharge> _repoadmCharge;
          RoleAssignImplementation _RoleAssignImplementation;

          IRepository<admBankBranch> _repoadmBankBranch;
           AccountValidationImplementation _AccountValidationImplementation;
           ApplicationReturnMessageImplementation _ApplicationReturnMessageImplementation;
           ComputeChargesImplementation _ComputeChargesImplementation;
           UsersImplementation _UsersImplementation;
           HeaderLogin _HeaderLogin;

        public CardsController( 
                                        IConfiguration configuration,
                                        ApplicationDbContext   ApplicationDbContext,
                                        ChargeImplementation ChargeImplementation,
                                        ServiceChargeImplementation ServiceChargeImplementation,
                                         IRepository<oprServiceCharges> repooprServiceCharge,
                                         RoleAssignImplementation RoleAssignImplementation,
                                         IRepository<OprCard> repoOprCard,
                                         IRepository<admCharge> repoadmCharge,
                                         IRepository<admService> repoadmService,
                                         IRepository<admBankBranch> repoadmBankBranch,
                                         AccountValidationImplementation AccountValidationImplementation,
                                         ApplicationReturnMessageImplementation ApplicationReturnMessageImplementation,
                                         ComputeChargesImplementation ComputeChargesImplementation,
                                          UsersImplementation UsersImplementation,
                                           HeaderLogin HeaderLogin) 
        {
           _configuration = configuration;
           AppSettingsPath = new AppSettingsPath(_configuration);
           TokenGenerator = new TokenGenerator(_configuration);
           db = new SqlConnection(AppSettingsPath.GetDefaultCon());
           _ApplicationDbContext =  ApplicationDbContext;
           _ChargeImplementation = ChargeImplementation;
           _ServiceChargeImplementation = ServiceChargeImplementation;
           _repooprServiceCharge = repooprServiceCharge;
           _repoOprCard = repoOprCard;
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
        public async Task<IActionResult> GetAll(CardDTO AnyAuth)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();

                param.Add("@pdtCurrentDate", _Formatter.FormatToDateYearMonthDay(AnyAuth.pdtCurrentDate));
                param.Add("@psBranchNo", AnyAuth.psBranchNo);
                param.Add("@pnDeptId", AnyAuth.pnDeptId);
                param.Add("@pnGlobalView", AnyAuth.pnGlobalView);
              
                var rtn = new DapperDATAImplementation<OprCardDTO>();
                
                var _response = await rtn.LoadData("isp_GetCardlist", param, db);

            
                if(_response != null)
                {
                   var getCharge = await _ChargeImplementation.GetChargeDetails(AnyAuth.ServiceId);

                   //var servicCharge = await _ServiceChargeImplementation.GetServiceCharges()
                   var res = new {
                       _response = _response,
                       charge = getCharge
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

        [HttpPost("GetAllCardSearchHistory")]
        public async Task<IActionResult> GetAllCardSearchHistory(InstrumentDTO AnyAuth)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                // if(!string.IsNullOrEmpty(AnyAuth.UserId.ToString()))
                // {
                //     param.Add("@pnUserId", AnyAuth.UserId);
                // }
                
                // var RoleAssign = await _RoleAssignImplementation.GetRoleAssign(AnyAuth.MenuId,AnyAuth.RoleId);
                // string IsGlobalView = "N";
                // if(RoleAssign != null)
                // {
                //     IsGlobalView = RoleAssign.IsGlobalSupervisor == true ?  "Y" : "N"; 
                // }
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
                
                if(!string.IsNullOrEmpty(AnyAuth.serialNo))
                {
                    param.Add("@serialNo", AnyAuth.serialNo);
                }
                if(!string.IsNullOrEmpty(AnyAuth.cardNo))
                {
                    param.Add("@CardNo", AnyAuth.cardNo);
                }
                if(!string.IsNullOrEmpty(AnyAuth.ServiceId.ToString()))
                {
                    param.Add("@ServiceId", AnyAuth.ServiceId);
                }
                if(!string.IsNullOrEmpty(AnyAuth.DeptId.ToString()))
                {
                    param.Add("@psDeptId", Convert.ToInt32(AnyAuth.DeptId));
                }
                if(!string.IsNullOrEmpty(AnyAuth.psBranchNo))
                {
                    param.Add("@psBranchNo", Convert.ToInt32(AnyAuth.psBranchNo));
                }

                //param.Add("@pnGlobalView", IsGlobalView);
                
                
              
                var rtn = new DapperDATAImplementation<OprInstrumentDTO>();
                
                var _response = await rtn.LoadData("Isp_GetCardRequestSearchHis", param, db);

                if(_response != null)
                {
                   var getCharge = await _ChargeImplementation.GetChargeDetails(AnyAuth.ServiceId);

                   var admService = await _repoadmService.GetAsync(c=> c.ServiceId == AnyAuth.ServiceId) ;
                 
                   var res = new {
                       _response = _response,
                       charge = getCharge,
                    // RoleAssign = RoleAssign,
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

      //Process SIngle
        [HttpPost("Process")]
        public async Task<IActionResult> Process(CardDTO p)
        {
            try
            {

                string UnauthorizedStatus =  _configuration["Statuses:UnauthorizedStatus"];

                int SeqNo = 0;
            
                foreach(var b in p.ListoprServiceCharge)
                {
                    SeqNo += 1;
                    b.Status = UnauthorizedStatus;
                    b.DateCreated= DateTime.Now;
                    b.SeqNo = SeqNo;
                    var SaveServiceChg = await _ServiceChargeImplementation.SaveServiceCharges(b, p.UserId);

                }
                var rev = -1; 
                foreach(var val in p.ListOprCard)
                {
                    var getRec = await  _repoOprCard.GetAsync(c=> c.ItbId == val.ItbId);
                    if(getRec != null)
                    {
                        getRec.ProcessingDeptId = p.DeptId;
                        getRec.ServiceStatus = UnauthorizedStatus;          
                        _repoOprCard.Update(getRec);
                        rev = await _ApplicationDbContext.SaveChanges(p.UserId);
                    }
                }
                
                if (rev > 0)
                {
                    ApiResponse.ResponseCode = 0;
                    ApiResponse.sErrorText = "Processed Successfully";
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

         //Process List
        [HttpPost("ProcessList")]
        public async Task<IActionResult> ProcessList(CardDTO p)
        {
            try
            {
               
              
                string Ids = string.Empty,  tranIds = string.Empty;
                foreach(var b in p.ListOprCard)
                {
                    Ids +=  b.ItbId +",";
                }

                tranIds = _Formatter.RemoveLast(Ids);
                 
                string UnauthorizedStatus =  _configuration["Statuses:UnauthorizedStatus"];;
            
                var admCharge = await _repoadmCharge.GetAsync(c=> c.ServiceId == p.ServiceId);
                if (admCharge != null)
                {

                    var cbs = new DapperDATAImplementation<OprCard>();

                    string script = $"select * from OprCard where itbid in({tranIds})";

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

                           // OperationViewModel.TransAmount = 0;
                            
                            int brch = 0;
                            if (!string.IsNullOrWhiteSpace(i.OriginatingBranchId))
                            {
                                brch = _Formatter.ValIntergers(i.OriginatingBranchId);
                            }

                            var getBranchCode = await _repoadmBankBranch.GetAsync(c=> c.BranchNo == brch);
                          
                            OperationViewModel.InstrumentAcctNo = i.AcctNo;


                            OperationViewModel.TempType  = 0;
                            OperationViewModel.InstrumentCcy  = i.CcyCode;
                            OperationViewModel.ChargeCode  = admCharge.ChargeCode;


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
                                  
                                    ApiResponse.ResponseCode =   (int)msg.ErrorId;
                                    ApiResponse.sErrorText=  $"{msg.ErrorText.Replace("{ErrorCode}", erroCode.ToString())}. Validation Message: {ValChg.sErrorText}";
                                    
                                    return BadRequest(ApiResponse);
                                }
                                
                            }

                            if (ValChg == null)
                            {   
                                int erroCode = 20010;
                                var msg = await _ApplicationReturnMessageImplementation.GetAppReturnMsg(erroCode);
                                ApiResponse.ResponseCode =   (int)msg.ErrorId;
                                ApiResponse.sErrorText=  $"{msg.ErrorText.Replace("{ErrorCode}", erroCode.ToString())}. Validation Message: {ValChg.sErrorText}";
                                    
                                return BadRequest(ApiResponse);
                            }

                            OperationViewModel.serviceId = p.ServiceId;
                            //  OperationViewModel.TransAmount = 0;
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
                            oprServiceCharge.ChgAcctType= i.AcctType;  
                            oprServiceCharge.ChgAcctName= i.AcctName; 
                            oprServiceCharge.ChgAcctStatus= i.AcctStatus;
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
                                
                                oprServiceCharge.IncAcctNo  = calCharge.sChargeIncAcctNo;
                                oprServiceCharge.IncAcctType = valIncomeAcct.sAccountType;
                                oprServiceCharge.IncAcctName  = valIncomeAcct.sName;
                                oprServiceCharge.IncAcctNarr  = calCharge.sNarration;
                                oprServiceCharge.IncBranch  = _Formatter.ValIntergers(valIncomeAcct.nBranch);
                             
                                oprServiceCharge.IncAcctStatus  = valIncomeAcct.nErrorCode == 0 ? valIncomeAcct.sStatus : valIncomeAcct.sErrorText;
                                oprServiceCharge.IncAcctBalance  = valIncomeAcct.nBalance;
                            }

                            i.ProcessingDeptId = p.DeptId;
                            i.ServiceStatus = UnauthorizedStatus;
                            i.UserId = p.UserId;
                            i.OrigDeptId = p.DeptId;

                            //Val Tax  Details

                            string taxAcct = string.Empty;
                            if (getBranchCode != null)
                            {
                               // taxAcct = admCharge.TaxAcctNo.Replace("***", getBranchCode.BranchCode);
                            }

                            // AccountValParam.AcctType = admCharge.TaxAcctType;
                            AccountValParam.AcctNo = taxAcct;
                            //  AccountValParam.CrncyCode = admCharge.TaxCurrency;
                            AccountValParam.Username = p.LoginUserName;

                            var ValTax = await _AccountValidationImplementation.ValidateAccountCall(AccountValParam);
                            if(ValTax != null)
                            {
                                oprServiceCharge.TaxAcctNo = taxAcct;
                                oprServiceCharge.TaxAcctType = ValTax.sAccountType;
                            }

                            oprServiceCharge.Status = UnauthorizedStatus;
                            oprServiceCharge.ServiceId = p.ServiceId;
                            oprServiceCharge.ServiceItbId = i.ItbId;
                            oprServiceCharge.ServiceItbId = i.ItbId;
                             
                           
                            oprServiceCharge.DateCreated= DateTime.Now;
                            oprServiceCharge.SeqNo = SeqNo; 
                         

                            var SaveServiceChg = await _ServiceChargeImplementation.SaveServiceCharges(oprServiceCharge, p.UserId);

                            _repoOprCard.Update(i);
                            var rev = await _ApplicationDbContext.SaveChanges(p.UserId);
                            if (rev > 0)
                            {
                                ApiResponse.ResponseCode =   0;
                                ApiResponse.sErrorText=  "Processed Successfully";       
                                return Ok(ApiResponse);
                            }
                        }
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
    
         [HttpPost("DismissList")]
        public async Task<IActionResult> DismissList(TokenDTO p)
        {
            try
            {
               
              
                string Ids = string.Empty,  tranIds = string.Empty;
                foreach(var b in p.ListOprTokenDTO)
                {
                    Ids +=  b.ItbId +",";
                }

                tranIds = _Formatter.RemoveLast(Ids);
                 
                string DimissedStatus =  _configuration["Statuses:DimissedStatus"];;
                var cbs = new DapperDATAImplementation<OprCard>();

                string script = $"select * from OprCard where itbid in({tranIds})";

                var transactions = await cbs.LoadListNoParam(script, db);
                foreach (var i in transactions)
                {
                    i.ServiceStatus = DimissedStatus;
                    i.DismissedBy = p.UserId;
                    i.DismissedDate =  DateTime.Now;

                    _repoOprCard.Update(i);

                    int revToken =   await    _ApplicationDbContext.SaveChanges(p.UserId);
                            
                    int upServiceCharge = -1;
                           
                    var getSevice = await  _repooprServiceCharge.GetManyAsync(c=> c.ServiceId == i.ServiceId && c.ServiceItbId == i.ItbId);

                    if(getSevice != null) 
                    {
                        foreach(var b in getSevice){
                            b.Status = DimissedStatus;
                            _repooprServiceCharge.Update(b) ;
                            upServiceCharge =   await  _ApplicationDbContext.SaveChanges(p.UserId);        
                        }
                    }

                    if(getSevice.Count() > 0) 
                    {
                        if (revToken > 0)
                        {
                            ApiResponse.ResponseCode =   0;
                            ApiResponse.sErrorText=  "Dismissed Successfully";   
                            return Ok(ApiResponse);
                        }   
                    }
                               
                    if(getSevice.Count() > 0) 
                    {
                        if (upServiceCharge > 0)
                        {
                            ApiResponse.ResponseCode =   0;
                            ApiResponse.sErrorText=  "Dismissed Successfully";                
                            return Ok(ApiResponse);
                        }  
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

        [HttpPost("Dismiss")]
        public async Task<IActionResult> Dismiss(TokenDTO p)
        {
            try
            {
            string DimissedStatus =  _configuration["Statuses:DimissedStatus"];

            int updateOprSer = 0;

             var getSer = await _repooprServiceCharge.GetManyAsync(c=> c.ServiceItbId ==  p.OprToken.ItbId && c.ServiceId == p.ServiceId);
           
            foreach(var b in getSer)
            {

                if(b != null)
                {
                    b.Status = DimissedStatus;
                   _repooprServiceCharge.Update(b);
                    updateOprSer = await _ApplicationDbContext.SaveChanges(p.UserId);
                }
                
              
            }
            var rev = -1; 
            var getRec = await  _repoOprCard.GetAsync(c=> c.ItbId == p.OprToken.ItbId);
            if(getRec != null){

                   getRec.ProcessingDeptId = p.DeptId;
              getRec.ServiceStatus = DimissedStatus;          
            _repoOprCard.Update(getRec);
              rev = await _ApplicationDbContext.SaveChanges(p.UserId);

            }

            if (rev > 0)
            {
                
                ApiResponse.ResponseCode =   0;
                ApiResponse.sErrorText=  "Dismissed Successfully!";
                    
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
        public async Task<IActionResult> Update(TokenDTO p)
        {
            try
            {
                   int revToken = -1;
                   int itbId  = p.OprToken.ItbId;

                   var getToken = await _repoOprCard.GetAsync(c=> c.ItbId == itbId);

                   if(getToken != null)
                   {
                       getToken.AcctNo = p.OprToken.AcctNo;
                      _repoOprCard.Update(getToken);

                      revToken =   await    _ApplicationDbContext.SaveChanges(p.UserId);
                   } 


                    int upServiceCharge = -1;

                     foreach(var b in p.ListoprServiceCharge)
                     {
                        var getSevice = await  _repooprServiceCharge.GetAsync(c=> c.ServiceId == p.OprToken.ServiceId && c.ServiceItbId == p.OprToken.ItbId);
              
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
       


         [HttpPost("CalCulateCharge")]
        public async Task<IActionResult> CalCulateCharge(CardDTO p)
        {
            try
            {
                    AccountValParam AccountValParam = new AccountValParam();

                    AccountValParam.AcctType = p.AcctType == string.Empty ? null : p.AcctType.Trim();
                    AccountValParam.AcctNo = p.AcctNo;
                    AccountValParam.CrncyCode = p.CcyCode;
                    AccountValParam.Username = p.LoginUserName;
                    
                    var InstrumentAcctDetails = await _AccountValidationImplementation.ValidateAccountCall(AccountValParam);

                    List<RevCalChargeModel> chgList = new List<RevCalChargeModel>();

                    foreach(var i in p.ListoprServiceCharge)
                    {
                            AccountValParam AccountVal = new AccountValParam();

                            AccountVal.AcctType = i.ChgAcctType == null ? InstrumentAcctDetails.sAccountType : i.ChgAcctType;
                            AccountVal.AcctNo = i.ChgAcctNo == null ? p.AcctNo : i.ChgAcctNo;
                            AccountVal.CrncyCode = i.ChargeCode;
                            AccountVal.Username = p.LoginUserName;

                            var ChgDetails = await _AccountValidationImplementation.ValidateAccountCall(AccountVal);

                            OperationViewModel OperationViewModel = new OperationViewModel();

                            OperationViewModel.serviceId = p.ServiceId;
                           // OperationViewModel.TransAmount = "0";
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

                     var values = await _HeaderLogin.FetchStartOfDayCoreBanking();

                    var chg = new 
                    {
                        InstrumentAcctDetails = InstrumentAcctDetails,
                        chgList = chgList,
                        bankingDate = values.date
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
        public async Task<IActionResult> GetById(CardDTO p)
        {
                try
                {
              
                 var get = await _repoOprCard.GetAsync(c=> c.ItbId ==  p.OprCard.ItbId);
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
                catch(Exception ex){
                    var exM = ex;
                    return BadRequest();
                }
               

        }

    
    }
}