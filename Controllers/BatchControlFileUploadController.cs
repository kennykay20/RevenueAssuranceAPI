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
using Dapper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RevAssuranceWebAPi.AnythingGood.DATA.Models;
using Microsoft.EntityFrameworkCore;
using RevAssuranceApi.Response;
using RevAssuranceApi.TokenGen;
using RevAssuranceApi.AppSettings;
using RevAssuranceApi.RevenueAssurance.DATA.Models;
using RevAssuranceApi.RevenueAssurance.Repository.DapperDAL;
using  RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO;
using RevAssuranceApi.OperationImplemention;
using RevAssuranceApi.RevenueAssurance.Repository.Interface;

namespace RevAssuranceApi.Controllers
{
     [Route("api/v1/[controller]")]
    [ApiController]
    [EnableCors("CorsPolicy")]
    public class BatchControlFileUploadController : ControllerBase
    {
         IConfiguration _configuration;
        ApiResponse ApiResponse = new ApiResponse();
        TokenGenerator TokenGenerator;  
        AppSettingsPath AppSettingsPath ;
         IDbConnection db = null;
         ApplicationDbContext   _ApplicationDbContext;
         RoleAssignImplementation _RoleAssignImplementation;
          IRepository<admService> _repoadmService;
          ChargeImplementation _ChargeImplementation;
         IRepository<BatchControl> _repoBatchControl;
         IRepository<BatchItems> _repoBatchItems;

         IRepository<BatchItemsTemp> _repoBatchItemsTemp;
         IRepository<BatchControlTemp> _repoBatchControlTemp;
         IRepository<admAccountType> _repoadmAccountType;
         IRepository<admErrorMsg> _repoadmErrorMsg;

         Formatter _Formatter =  new Formatter();
         UsersImplementation _UsersImplementation;
         AccountValidationImplementation _AccountValidationImplementation;
         ComputeChargesImplementation _ComputeChargesImplementation;
         ServiceChargeImplementation _ServiceChargeImplementation;
         IRepository<admTransactionConfiguration> _repoadmTransactionConfiguration;
        public BatchControlFileUploadController(IConfiguration configuration,
                                      ApplicationDbContext   ApplicationDbContext,
                                      RoleAssignImplementation RoleAssignImplementation,
                                      IRepository<BatchControl> repoBatchControl,
                                      IRepository<admService> repoadmService,
                                      ChargeImplementation ChargeImplementation,
                                      IRepository<BatchItems> repoBatchItems,
                                      UsersImplementation UsersImplementation,
                                      AccountValidationImplementation AccountValidationImplementation,
                                      ComputeChargesImplementation ComputeChargesImplementation,
                                      ServiceChargeImplementation ServiceChargeImplementation,
                                      IRepository<BatchItemsTemp> repoBatchItemsTemp,
                                      IRepository<BatchControlTemp> repoBatchControlTemp,
                                      IRepository<admAccountType> repoadmAccountType,
                                      IRepository<admErrorMsg> repoadmErrorMsg,
                                      IRepository<admTransactionConfiguration> repoadmTransactionConfiguration
                            )
        {
           _configuration = configuration;
           AppSettingsPath = new AppSettingsPath(_configuration);
           TokenGenerator = new TokenGenerator(_configuration);
           db = new SqlConnection(AppSettingsPath.GetDefaultCon());
           _ApplicationDbContext =  ApplicationDbContext;
           _RoleAssignImplementation = RoleAssignImplementation;
           _repoBatchControl = repoBatchControl;
           _repoadmService = repoadmService;
           _ChargeImplementation = ChargeImplementation;
           _repoBatchItems = repoBatchItems;
           _UsersImplementation = UsersImplementation;
           _AccountValidationImplementation = AccountValidationImplementation;
           _ComputeChargesImplementation = ComputeChargesImplementation;
           _ServiceChargeImplementation = ServiceChargeImplementation;
           _repoBatchItemsTemp = repoBatchItemsTemp;
           _repoBatchControlTemp = repoBatchControlTemp;
           _repoadmAccountType = repoadmAccountType;
           _repoadmErrorMsg = repoadmErrorMsg;
           _repoadmTransactionConfiguration = repoadmTransactionConfiguration;
        }


        [HttpPost("GetAll")]
        public async Task<IActionResult> GetAll(ParamLoadPage AnyAuth)
        {
            try
            {
                var rtn = new DapperDATAImplementation<BatchControlDTONew>();
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

                var _response = await rtn.LoadData("isp_BatchControlTemp", param, db);

            
                if(_response != null)
                {
                    var roleAssign = await _RoleAssignImplementation.GetRoleAssign(AnyAuth.MenuId,AnyAuth.RoleId);
                    var admService = await _repoadmService.GetAsync(c=> c.ServiceId == AnyAuth.ServiceId);
                    
                     var getCharge = await _ChargeImplementation.GetChargeDetails(AnyAuth.ServiceId);
                  
                   var listoprServiceCharges =  new List<oprServiceCharges>();
                   
                    if(getCharge.Count > 0)
                    {
                        foreach(var b in getCharge)
                        {
                           listoprServiceCharges.Add(new oprServiceCharges
                           {

                                ItbId				 = 0,
                                ServiceId			 = (int)b.ServiceId,
                                ServiceItbId		 = 0,
                                ChgAcctNo			 = null,
                                ChgAcctType			 = null,
                                ChgAcctName			 = null,
                                ChgAvailBal			 = null,
                                ChgAcctCcy			 = null,
                                ChgAcctStatus		 = null,
                                ChargeCode			 = b.ChargeCode,
                                ChargeRate			 = null,
                                OrigChgAmount		 = null,
                                OrigChgCCy			 = null,
                                ExchangeRate		 = null,
                                EquivChgAmount		 = null,
                                EquivChgCcy			 = null,
                                ChgNarration		 = null,
                                TaxAcctNo			 = null,
                                TaxAcctType			 = null,
                                TaxRate				 = null,
                                TaxAmount			 = null,
                                TaxNarration		 = null,
                                IncBranch			 = null,
                                IncAcctNo			 = null,
                                IncAcctType			 = null,
                                IncAcctName			 = null,
                                IncAcctBalance		 = null,
                                IncAcctStatus		 = null,
                                IncAcctNarr			 = null,
                                SeqNo				 = null,
                                Status				 = null,
                                DateCreated			 = null,
                                TemplateId = b.TemplateId
                          });
                        }
                    }

                     var res = new {
                        _response = _response,
                        roleAssign = roleAssign,
                        admService = admService,
                        charge = listoprServiceCharges,
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
                 ApiResponse.ResponseMessage =  ex.InnerException.Message ?? ex.Message;
                 ApiResponse.ResponseCode = -99;
                 return BadRequest(ApiResponse); 
            }
        }

        [HttpPost("Add")]
        public async Task<IActionResult> Add(BatchControlDTO p)
        {
            try
            {
                int rev = -1; int batchInsert = -1;
                var getOne  = p.ListBatchItemsTemp.FirstOrDefault();
                var dr = p.ListBatchItemsTemp.Where(c => c.DrCr == "DR");
                var cr = p.ListBatchItemsTemp.Where(c => c.DrCr == "CR");
                var getTolDr = dr.Sum(c=> c.Amount);
                var getTolCr = cr.Sum(c=> c.Amount);
                
                 var dif = getTolDr - getTolCr;

                 BatchControlTemp batc = new BatchControlTemp();

                 batc = p.BatchControlTemp;
                 batc.RecordCount = p.ListBatchItemsTemp.Count;
                 batc.TotalDrCount = dr.Count();
                 batc.TotalCrCount = cr.Count();
                 batc.CcyCode = getOne.CcyCode;
                  if(getTolDr == getTolCr)
                  {
                       batc.IsBalanced = "Y";
                  }
                  else
                  {
                       batc.IsBalanced = "N";
                  }

                 batc.TotalDr = getTolDr;
                 batc.TotalCr = getTolCr;
                 batc.TDifference = dif;
                 batc.DateCreated = DateTime.Now;
                 batc.LoadedBy = p.LoginUserId;
                 batc.ServiceId = p.ServiceId;
                 batc.OriginBranchNo = p.OriginBranchNo;
                 batc.Status = "Loaded";
               
                await _repoBatchControlTemp.AddAsync(batc);
                batchInsert = await _ApplicationDbContext.SaveChanges(p.LoginUserId);

                int seqNo = 0;

                string valErrorCodes = string.Empty;

                 var  admTransactionConfiguration = await  _repoadmTransactionConfiguration.GetAsync(c=> c.ServiceId == p.ServiceId);
                 foreach(var b in p.ListBatchItemsTemp)
                 {
                    var getAccType = await _repoadmAccountType.GetAsync(c=> c.AccountTypeCode.Trim().ToLower() == b.AcctType);
                    if(getAccType == null)
                    {
                        valErrorCodes = _Formatter.AddComma("1001");
                    }

                    if(!string.IsNullOrWhiteSpace(b.AcctType))
                    {
                        if(b.AcctType.Trim().ToLower() != "gl")
                        if(!string.IsNullOrWhiteSpace(b.AcctNo))
                        {
                            if(getAccType.AcctLenght < b.AcctNo.Length && b.AcctNo.Length > 0)
                            {
                                valErrorCodes = _Formatter.AddComma("1002");
                            }
                            if(b.AcctNo.Length > getAccType.AcctLenght && b.AcctNo.Length > 0)
                            {
                                valErrorCodes = _Formatter.AddComma("1003");
                            }
                        }
                    }

                    var amtVal = b.Amount != null ? _Formatter.ValDecimal(b.Amount.ToString()) : 0;
                    if(amtVal  == 0 )
                    {
                        valErrorCodes = _Formatter.AddComma("1004");
                    }                
                    if(b.CbsTC != null && b.AcctType != null && b.AcctType.ToString().ToLower() != "gl" 
                                && b.DrCr != null && b.DrCr.ToString().ToLower() == "dr" && admTransactionConfiguration.CustomerAcctDrTC.Trim() != b.CbsTC.Trim())
                    {
                        valErrorCodes = _Formatter.AddComma("1006");
                    }

                     if(b.CbsTC != null && b.AcctType != null && b.AcctType.ToString().ToLower() != "gl" 
                                && b.DrCr != null && b.DrCr.ToString().ToLower() == "cr" 
                                && admTransactionConfiguration.CustomerAcctDrTC.Trim() != b.CbsTC.Trim())
                    {
                        valErrorCodes = _Formatter.AddComma("1006");
                    }

                    if(string.IsNullOrWhiteSpace(b.CbsTC))
                    {
                          
                           if(!string.IsNullOrWhiteSpace(b.DrCr))
                           if(b.DrCr.Trim().ToLower() == "dr")
                           {
                               if(b.AcctNo.Length == 10)
                               {
                                    b.CbsTC = admTransactionConfiguration.CustomerAcctDrTC;
                               }
                               else
                               {
                                   b.CbsTC = admTransactionConfiguration.GLAcctDrTC;
                               }
                           } 
                           else if(b.DrCr.Trim().ToLower() == "cr")
                           {
                               if(b.AcctNo.Length == 10)
                               {
                                    b.CbsTC = admTransactionConfiguration.CustomerAcctCrTC;
                               }
                               else
                               {
                                   b.CbsTC = admTransactionConfiguration.GLAcctCrTC;
                               }
                           }  
                    }

                    if(!string.IsNullOrWhiteSpace(b.AcctType))
                    if(b.AcctType.Trim().ToLower() != "gl")
                    {
                        if(b.AcctNo.Trim().Length < getAccType.AcctLenght)
                        {

                        }
                    }

                    if(b.ChargeAmount != null)
                    {
                        var chgAmtVal = _Formatter.ValDecimal(b.Amount.ToString());
                        if(chgAmtVal  == 0 )
                        {
                            valErrorCodes = _Formatter.AddComma("1007");
                        }
                    }

                    if(b.ValueDate != null)
                    {
                        var date = _Formatter.ValidateDate(b.ValueDate.ToString());
                        if(date.ToString()  == "01/01/001")
                        {
                            valErrorCodes = _Formatter.AddComma("1008");
                        }
                    }

                    
                    valErrorCodes = _Formatter.RemoveLast(valErrorCodes);

                    seqNo += 1;

                    b.BatchSeqNo = seqNo;
                    b.ServiceStatus = "Loaded";
                    b.DateCreated= DateTime.Now;
                    b.UserId = p.LoginUserId;
                    b.BatchNo = batc.BatchNo;
                    b.ServiceId = p.ServiceId;
                    b.ValErrorIds = valErrorCodes;
                    
                    await  _repoBatchItemsTemp.AddAsync(b);
                    rev = await _ApplicationDbContext.SaveChanges(p.LoginUserId);
               }
               if(batchInsert> 0 && rev > 0)
               {

                    ApiResponse.ResponseCode =   0;
                    ApiResponse.ResponseMessage=  "Processed Successfully";
                   var res = new 
                   {
                       batch = batc,
                       ApiResponse= ApiResponse
                   };
                    return Ok(res);
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
        public async Task<IActionResult> Update(BatchControlDTO p)
        {
            try
            {
                BatchControlTemp  batch = new BatchControlTemp();
               
                batch = p.BatchControlTemp;

                _repoBatchControlTemp.Update(batch);
               
                int rev = await _ApplicationDbContext.SaveChanges(p.LoginUserId);

               if(rev > 0)
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


        [HttpPost("AddItem")]
        public async Task<IActionResult> AddItem(BatchControlDTO p)
        {
            try
            {
                BatchItems  batchItem = new BatchItems();
               
                batchItem = p.BatchItems;

                 string UnauthorizedStatus =   _configuration["Statuses:UnauthorizedStatus"];
                 batchItem.DateCreated= DateTime.Now;
                 batchItem.Status= UnauthorizedStatus;
                 batchItem.UserId= p.LoginUserId;
           
               await  _repoBatchItems.AddAsync(batchItem);
               int rev = await _ApplicationDbContext.SaveChanges(p.LoginUserId);

               if(rev > 0)
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
        public async Task<IActionResult> GetById(BatchControlDTO p)
        {
            try
            {
               var  instrumentDetails = await _repoBatchControlTemp.GetAsync(c=> c.BatchNo == p.BatchControlTemp.BatchNo);
                    instrumentDetails.RecordCount = instrumentDetails.RecordCount == null ? 0 :  instrumentDetails.RecordCount;
                    instrumentDetails.PostedTransCount = instrumentDetails.PostedTransCount == null ? 0 :  instrumentDetails.PostedTransCount;
                    instrumentDetails.TotalDr = instrumentDetails.TotalDr == null ? Convert.ToDecimal("0.00") :  instrumentDetails.TotalDr;
                    instrumentDetails.TotalCr = instrumentDetails.TotalCr == null ? Convert.ToDecimal("0.00") :  instrumentDetails.TotalCr;
                    instrumentDetails.TDifference = instrumentDetails.TDifference == null ? Convert.ToDecimal("0.00") :  instrumentDetails.TDifference;
                 if(instrumentDetails != null)
                 {

                    var allUsers = await _UsersImplementation.GetAllUser(instrumentDetails.LoadedBy, instrumentDetails.RejectedBy, 0);

                    var batchItems = await _repoBatchItemsTemp.GetManyAsync(c=> c.BatchNo == instrumentDetails.BatchNo );

                    var res = new {
                                    instrumentDetails = instrumentDetails,
                                    allUsers = allUsers,
                                    batchItems = batchItems
                                };

                    return Ok(res);
                 
                  }
                }
                catch(Exception ex){
                    var exM = ex;
                    return BadRequest();
                }
               
                return BadRequest();
        }

        [HttpPost("GetByIdItem")]
        public async Task<IActionResult> GetByIdItem(BatchControlDTO p)
        {
            try
            {
               var  instrumentDetails = await _repoBatchItemsTemp.GetAsync(c=> c.ItbId == p.BatchItemsTemp.ItbId);
                    
                 if(instrumentDetails != null)
                 {

                    var allUsers = await _UsersImplementation.GetAllUser(instrumentDetails.UserId, 0, 0);
                    var serviceChargeslist = new List<oprServiceCharges>();
                    serviceChargeslist.Add(new oprServiceCharges
                       {
                            ServiceId = p.ServiceId,
                            ServiceItbId = instrumentDetails.ItbId,
                            ChgAcctNo = instrumentDetails.AcctNo, 
                            ChgAcctType = instrumentDetails.AcctType ,
                            ChgAcctName = null,//instrumentDetails.AcctName,
                            ChgAvailBal = null,  //instrumentDetails.AcctBalance,
                            ChgAcctCcy = null,  //instrumentDetails.CcyCode,
                            ChgAcctStatus  = null,  //instrumentDetails.AcctStatus,
                            ChargeCode  = null,  //instrumentDetails.ChargeCode,
                            ChargeRate  = null,  //instrumentDetails.TaxRate,
                            OrigChgAmount  = null,  // instrumentDetails.ChargeAmount,
                            OrigChgCCy  = null,  //instrumentDetails.CcyCode,
                            ExchangeRate  = null,// instrumentDetails.ItbId,
                            EquivChgAmount  = null,// instrumentDetails.ItbId,
                            EquivChgCcy  = instrumentDetails.CcyCode,
                            ChgNarration  = instrumentDetails.ChgNarration,
                            TaxAcctNo  = null,  //instrumentDetails.TaxAcctNo,
                            TaxAcctType  = null,  //instrumentDetails.TaxAcctType,
                            TaxRate = null,  //instrumentDetails.TaxRate,
                            TaxAmount  = null,  //instrumentDetails.TaxAmount,
                            TaxNarration  = null,  //instrumentDetails.TaxNarration,
                            IncBranch  = null,  //instrumentDetails.IncBranch,
                            IncAcctNo  = null,  //instrumentDetails.IncAcctNo,
                            IncAcctType  = null,  //instrumentDetails.IncAcctType,
                            IncAcctName  = null,  //instrumentDetails.IncAcctName,
                            IncAcctBalance  = null,  //instrumentDetails.IncAcctBalance,
                            IncAcctStatus  = null,  //instrumentDetails.IncAcctStatus,
                            IncAcctNarr  = null,  // instrumentDetails.IncAcctNarr,
                            // SeqNo  = null,
                            // Status  = null,
                            // DateCreated  = null,

                       }); 
                   
                   if(instrumentDetails.ChargeCode == null)
                   {
                       serviceChargeslist = null;
                   }
                   
                    var res = new {
                        instrumentDetails = instrumentDetails,
                        allUsers = allUsers,
                        serviceChargeslist = serviceChargeslist,
                      
                    };

                    return Ok(res);
                 
                  }
                }
                catch(Exception ex){
                    var exM = ex;
                    return BadRequest();
                }
               
                return BadRequest();
        }

         [HttpPost("CalCulateCharge")]
        public async Task<IActionResult> CalCulateCharge(BatchControlDTO p)
        {
            try
            {
                    AccountValParam AccountValParam = new AccountValParam();

                    AccountValParam.AcctNo = p.AcctNo;
                    AccountValParam.AcctType = p.AcctType == null ? null : p.AcctType.Trim();
                    
                    AccountValParam.CrncyCode = p.CcyCode;
                    AccountValParam.Username = p.LoginUserName;
                    
                    var InstrumentAcctDetails = await _AccountValidationImplementation.ValidateAccountCall(AccountValParam);
                    
                    
                    InstrumentAcctDetails.sRsmId = InstrumentAcctDetails.sCustomerId != null ? Convert.ToInt32(InstrumentAcctDetails.sCustomerId) : 0;

                    List<RevCalChargeModel> chgList = new List<RevCalChargeModel>();
                    if(p.ChargeThisAcct != false){
                        foreach(var i in p.ListoprServiceCharge)
                        {
                            AccountValParam AccountVal = new AccountValParam();

                            AccountVal.AcctType = i.ChgAcctType == null ? InstrumentAcctDetails.sAccountType : i.ChgAcctType;
                            AccountVal.AcctNo = i.ChgAcctNo == null ? p.AcctNo : i.ChgAcctNo;
                            AccountVal.CrncyCode = InstrumentAcctDetails.sCrncyIso.Trim();
                            AccountVal.Username = p.LoginUserName;

                            var ChgDetails = await _AccountValidationImplementation.ValidateAccountCall(AccountVal);

                            OperationViewModel OperationViewModel = new OperationViewModel();
                            
                            decimal amt =  p.TransAmout != null ? _Formatter.ValDecimal(p.TransAmout) : 0;
                            OperationViewModel.serviceId = p.ServiceId;
                            OperationViewModel.TransAmount = amt.ToString() ;
                            OperationViewModel.InstrumentAcctNo = i.ChgAcctNo == null ? p.AcctNo : i.ChgAcctNo;
                            OperationViewModel.InstrumentCcy = InstrumentAcctDetails.sCrncyIso;
                            OperationViewModel.ChargeCode = i.ChargeCode;

                            int? TempId = i.TemplateId != null ?  _Formatter.ValIntergers(i.TemplateId.ToString()) : 0;

                            OperationViewModel.TempTypeId = TempId == 0 ? null : TempId;

                            var calCharge = await _ComputeChargesImplementation.CalChargeModel(OperationViewModel, 
                                                                                                ChgDetails.nBranch.ToString(), ChgDetails.sAccountType, ChgDetails.sCrncyIso);
                            calCharge.chgAcctNo	= AccountVal.AcctNo;
                            calCharge.chgAcctType = AccountVal.AcctType;
                            calCharge.chgAcctName = ChgDetails.sName;
                            calCharge.chgAvailBal = ChgDetails.nBalance;
                            calCharge.chgAcctCcy = ChgDetails.sCrncyIso;
                            calCharge.chgAcctStatus	= ChgDetails.sStatus;
                            calCharge.TemplateId = TempId;

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

                            calCharge.chargeRate = calCharge.nChargeRate  != null ? calCharge.nChargeRate.ToString() : "0";
                            calCharge.origChgAmount = calCharge.nOrigChargeAmount != null ? calCharge.nOrigChargeAmount.ToString() : "0";
                            calCharge.exchangeRate = calCharge.nExchRate != null ? calCharge.nExchRate.ToString() : "0";
                            calCharge.equivChgAmount = calCharge.nActualChgAmt  != null ? calCharge.nActualChgAmt.ToString() : "0";
                            calCharge.taxAmount  = calCharge.nTaxAmt  != null ? calCharge.nTaxAmt.ToString() : "0";
                          
                            
                            chgList.Add(calCharge);

                    }
                      
                    }
                     

                     if(p.ChargeThisAcct == false){
                         chgList = null;
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

        [HttpPost("AddBatchItem")]
        public async Task<IActionResult> AddBatchItem(BatchControlDTO p)
        {
            try
            {
                int rev = -1;
                 BatchItemsTemp batchItemChg = new BatchItemsTemp();
               
                batchItemChg = p.BatchItemsTemp;

                string UnauthorizedStatus = "Loaded";
                batchItemChg.BatchNo = (long)p.BatchNo;
                batchItemChg.ServiceStatus = UnauthorizedStatus;
                batchItemChg.DateCreated= DateTime.Now;
                batchItemChg.TransactionDate = Convert.ToDateTime(p.TransactionDate);
                batchItemChg.ValueDate = Convert.ToDateTime(p.ValueDate);
                batchItemChg.BatchSeqNo = 1;
                batchItemChg.UserId = p.LoginUserId;

                 await _repoBatchItemsTemp.AddAsync(batchItemChg);
                 rev = await _ApplicationDbContext.SaveChanges(p.LoginUserId);

           
                   if(rev > 0)
                   {
                       ApiResponse.ResponseMessage = "Your Request was Successful!";
                      // var getAll = await _repoBatchItems.GetManyAsync(c=> c.BatchNo == p.BatchNo);

                       var res = new {
                          // getAll = getAll,
                           ApiResponse = ApiResponse
                       };
                        return Ok(ApiResponse);
                   }
                 
                        
                    
                ApiResponse.ResponseMessage = "Error Occured!";
                    return BadRequest(ApiResponse);

            }
            catch (Exception ex)
            {
                 ApiResponse.ResponseMessage =  ex == null ? ex.InnerException.Message : ex.Message;
                
                 ApiResponse.ResponseCode = -99;
                 return BadRequest(ApiResponse); 
            }
        }
        
        [HttpPost("UpdateBatchItem")]
        public async Task<IActionResult> UpdateBatchItem(BatchControlDTO p)
        {
            try
            {
                int rev = -1;
                 BatchItemsTemp batchItemChg = new BatchItemsTemp();
               
                batchItemChg = p.BatchItemsTemp;

               
                batchItemChg.UserId = p.LoginUserId;
            
                  _repoBatchItemsTemp.Update(batchItemChg);
                    rev = await _ApplicationDbContext.SaveChanges(p.LoginUserId);
               
                   if(rev > 0)
                   {
                       ApiResponse.ResponseMessage = "Your Request was Successful!";
                      // var getAll = await _repoBatchItems.GetManyAsync(c=> c.BatchNo == p.BatchNo);

                       var res = new {
                          // getAll = getAll,
                           ApiResponse = ApiResponse
                       };
                        return Ok(ApiResponse);
                   }
                 
                        
                    
                ApiResponse.ResponseMessage = "Error Occured!";
                    return BadRequest(ApiResponse);

            }
            catch (Exception ex)
            {
                 ApiResponse.ResponseMessage =  ex == null ? ex.InnerException.Message : ex.Message;
                
                 ApiResponse.ResponseCode = -99;
                 return BadRequest(ApiResponse); 
            }
        }
        

        [HttpPost("RemoveBatch")]
        public async Task<IActionResult> RemoveBatch(BatchControlDTO p)
        {
              ApiResponse apiResponse = new ApiResponse();
           
            try
            {
                StringBuilder sb = new StringBuilder("<ul>");
        
                foreach(var b in p.ListBatchControl)
                {
                    int rev = -1;
                       var get = await _repoBatchControl.GetAsync(c => c.BatchNo == b.BatchNo);
                       get.Status = "Closed";
                       
                       _repoBatchControl.Update(get);

                        rev = await _ApplicationDbContext.SaveChanges(p.LoginUserId);
                        if(rev > 0)
                        {
                            sb.Append($"<li> Success: Batch No: { b.BatchNo }  Removed Successfully!</li>");
                
                        }
                        else
                        {
                            sb.Append($"<li> Failed: Batch No: { b.BatchNo }  Did'nt Removed</li>");

                        }
                }
            sb.Append("</ul>");
            apiResponse.ResponseMessage = sb.ToString(); 

             return Ok(apiResponse);
            }
            catch(Exception ex)
            {
                var exM = ex;
            }

            apiResponse.ResponseMessage  = "Error Occured!";

         return BadRequest(apiResponse);

        
        }

        [HttpPost("ValidateUpload")]
        public async Task<IActionResult> ValidateUpload(BatchControlDTO p)
        {
            try
            {
                
                 string valErrorCodes = string.Empty;
                 var  admTransactionConfiguration = await  _repoadmTransactionConfiguration.GetAsync(c=> c.ServiceId == p.ServiceId);
                 foreach(var b in p.UploadBulkFilesValidatorList)
                 {
                    var getAccType = await _repoadmAccountType.GetAsync(c=> c.AccountTypeCode.Trim().ToLower() == b.AcctType);
                    if(getAccType == null)
                    {
                        valErrorCodes = _Formatter.AddComma("1001");

                        b.DontSave = true;
                    }
                    else
                    {
                        
                        if(!string.IsNullOrWhiteSpace(b.AcctType))
                        {
                            if(b.AcctType.Trim().ToLower() != "gl")
                            if(!string.IsNullOrWhiteSpace(b.AcctNo))
                            {
                                if(getAccType.AcctLenght < b.AcctNo.Length && b.AcctNo.Length > 0)
                                {
                                    valErrorCodes += _Formatter.AddComma("1002");
                                }
                                if(b.AcctNo.Length > getAccType.AcctLenght && b.AcctNo.Length > 0)
                                {
                                    valErrorCodes += _Formatter.AddComma("1003");
                                    
                                }
                            }
                        }

                          if(b.Amount != null)
                        {
                            var amt = _Formatter.ValDecimal(b.Amount.ToString());
                            if(amt  == 0 )
                            {
                                b.DontSave = true;
                                valErrorCodes += _Formatter.AddComma("1004");
                            }
                        }

                    if(b.DrCr != null && b.DrCr.ToString().ToLower() != "dr" 
                    && b.DrCr.ToString().ToLower() != "cr")
                    {
                         b.DontSave = true;
                         valErrorCodes += _Formatter.AddComma("1005");
                    }
                         if(b.CbsTC != null && b.AcctType != null && b.AcctType.ToString().ToLower() != "gl" 
                                && b.DrCr != null && b.DrCr.ToString().ToLower() == "dr" && admTransactionConfiguration.CustomerAcctDrTC.Trim() != b.CbsTC.Trim())
                    {
                        valErrorCodes += _Formatter.AddComma("1006");
                    }

                         if(b.CbsTC != null && b.AcctType != null && b.AcctType.ToString().ToLower() != "gl" 
                                && b.DrCr != null && b.DrCr.ToString().ToLower() == "cr" 
                                && admTransactionConfiguration.CustomerAcctCrTC.Trim() != b.CbsTC.Trim())
                    {
                        valErrorCodes = _Formatter.AddComma("1006");
                    }

                          if(string.IsNullOrWhiteSpace(b.CbsTC))
                         {
                          
                           if(!string.IsNullOrWhiteSpace(b.DrCr))
                           if(b.DrCr.Trim().ToLower() == "dr")
                           {
                               if(b.AcctNo.Length == 10)
                               {
                                    b.CbsTC = admTransactionConfiguration.CustomerAcctDrTC;
                               }
                               else
                               {
                                   b.CbsTC = admTransactionConfiguration.GLAcctDrTC;
                               }
                           } 
                           else if(b.DrCr.Trim().ToLower() == "cr")
                           {
                               if(b.AcctNo.Length == 10)
                               {
                                    b.CbsTC = admTransactionConfiguration.CustomerAcctCrTC;
                               }
                               else
                               {
                                   b.CbsTC = admTransactionConfiguration.GLAcctCrTC;
                               }
                           }  
                         }

                         if(!string.IsNullOrWhiteSpace(b.AcctType))
                         if(b.AcctType.Trim().ToLower() != "gl")
                        {
                            if(b.AcctNo.Trim().Length < getAccType.AcctLenght)
                            {
                            }
                        }
                         if(b.ChargeAmount != null)
                        {
                            var chgAmtVal = _Formatter.ValDecimal(b.ChargeAmount.ToString());
                            if(chgAmtVal  == 0 )
                            {
                                b.DontSave = true;
                                valErrorCodes += _Formatter.AddComma("1007");
                            }
                        }
                        if(b.ValueDate != null)
                        {
                            var date = _Formatter.ValidateDate(b.ValueDate.ToString());
                            if(date.ToString()  == "1/1/0001 12:00:00 AM")
                            {
                                b.DontSave = true;
                                valErrorCodes += _Formatter.AddComma("1008");
                            }
                        }

                     

                    }

                      valErrorCodes = _Formatter.RemoveLast(valErrorCodes);
                      if(!string.IsNullOrWhiteSpace(valErrorCodes))
                      {
                          var rtn = new DapperDATAImplementation<admErrorMsg>();

                        string script = $"Select * from admErrorMsg where ErrorId in ({valErrorCodes})";
                        
                        var _response = await rtn.LoadListNoParam(script, db);
                        int number = 0;
                        foreach(var m in _response)
                        {
                             number += 1;
                             
                             b.ValErrorMessage +=  $"{ number }. { m.ErrorText }  "; 
                        }

                        valErrorCodes = null;

                      }
                       
               } 
        
              return Ok(p.UploadBulkFilesValidatorList);
        
          }
          catch (Exception ex)
          {
                ApiResponse.ResponseMessage =  ex == null ? ex.InnerException.Message : ex.Message;
            
                ApiResponse.ResponseCode = -99;
                return BadRequest(ApiResponse); 
          }

           
        }
        
        [HttpPost("ProcessFileUpload")]
        public async Task<IActionResult> ProcessFileUpload(BatchControlDTO p)
        {
            int rev = -1,   getNoOfSuccess = 0,  batchSucess = 0;
            try
            {
               
                foreach(var i in p.ListBatchControl)
                {
                 var rtn = new DapperDATAImplementation<BatchItemsTemp>();
                 string script = $"select * from BatchItemsTemp where batchNo in({i.BatchNo})";
                 var transactions = await rtn.LoadListNoParam(script, db);

                         BatchControl batchControl = new BatchControl();
                    batchControl.BatchNo = null;
                    batchControl.Description			= i.Description ;
                    batchControl.ServiceId				= i.ServiceId ;
                    batchControl.CcyCode				= i.CcyCode ;
                    batchControl.PostedTransCount		= i.PostedTransCount ;
                    batchControl.RecordCount			= i.RecordCount ;
                    batchControl.TotalDrCount			= i.TotalDrCount ;
                    batchControl.TotalCrCount			= i.TotalCrCount ;
                    batchControl.TotalDr				= i.TotalDr ;
                    batchControl.TotalCr				= i.TotalCr ;
                    batchControl.TDifference			= i.TDifference ;
                    batchControl.LoadedBy				= i.LoadedBy ;
                    batchControl.Dept					= i.Dept ;
                    batchControl.OriginBranchNo			= i.OriginBranchNo ;
                    batchControl.IsBalanced				= i.IsBalanced;
                    //batchControl.VerifiedBy				= i. ;
                    //batchControl.ApprovedBy				= i. ;
                    batchControl.PostedDrCount			= i.PostedDrCount ;
                    batchControl.PostedCrCount			= i.PostedCrCount ;
                    batchControl.IsManual				= "N" ;
                    batchControl.Status					=  "Loaded";
                    batchControl.Filename				= i.Filename ;
                    batchControl.DateCreated			= DateTime.Now;;
                    //batchControl.DateVerified			= i. ;
                    //batchControl.DateApproved			= i. ;
                    batchControl.ProcessingDept			= i.ProcessingDept ;
                   // batchControl.Rejected				= i. ;
                   // batchControl.RejectedBy				= i. ;
                   // batchControl.RejectionReason		= i. ;
                   // batchControl.RejectionDate			= i. ;
                   // batchControl.ClosedBy				= i. ;
                   // batchControl.DateClosed				= i. ;
                   // batchControl.PostingDate			= i. ;
                  //  batchControl.DefaultNar				= i. ;
                 

                    await _repoBatchControl.AddAsync(batchControl);

                    int bctInsert = await _ApplicationDbContext.SaveChanges(p.LoginUserId);

                 

                  foreach (var b in transactions)
                  {  
                     
                        BatchItems  item = new BatchItems();

                        AccountValParam AccountValParam = new AccountValParam();

                        AccountValParam.AcctNo = b.AcctNo;
                        AccountValParam.AcctType = b.AcctType == null ? null : b.AcctType.Trim();
                        
                        AccountValParam.CrncyCode = b.CcyCode;
                        AccountValParam.Username = p.LoginUserName;
                        
                        var InstrumentAcctDetails = await _AccountValidationImplementation.ValidateAccountCall(AccountValParam);
                    
                        item.ServiceId				= b.ServiceId;
                        item.BatchNo				= (long)batchControl.BatchNo;
                        item.BranchNo				= InstrumentAcctDetails.nBranch;
                        item.AcctNo					= b.AcctNo;
                        item.AcctType				= b.AcctType ;
                        item.AcctName				= InstrumentAcctDetails.sName ;
                        item.AcctBalance			= InstrumentAcctDetails.nBalanceDec;
                        item.AcctStatus				= InstrumentAcctDetails.sStatus;
                        item.CbsTC					= b.CbsTC ;
                        item.ChequeNo				= null;// InstrumentAcctDetails.nLastChqNo;
                        item.CcyCode				= b.CcyCode ;
                        item.Amount					= b.Amount ;
                        item.DrCr					= b.DrCr ;
                        item.Narration				= b.Narration;
                        item.ChargeCode				= b.ChargeCode ;
                        item.ChargeAmount			= b.ChargeAmount ;
                        item.ChgNarration			= b.ChgNarration ;
                        item.TaxAcctNo				= null; //b.TaxAcctNo ;
                        item.TaxAcctType			= null;//b.TaxAcctType ;
                        item.TaxRate				= null ;//b.TaxRate ;
                        item.TaxAmount				= null; //b.TaxAmount ;
                        item.TaxNarration			= null ;//b.TaxNarration ;
                        item.IncBranch				= null;//b.IncBranch ;
                        item.IncAcctNo				= null;//b.IncAcctNo ;
                        item.IncAcctType			= null; //b.IncAcctType ;
                        item.IncAcctName			= null; //b.IncAcctName ;
                        item.IncAcctBalance			= null;//b.IncAcctBalance ;
                        item.IncAcctStatus			= null; //b.IncAcctStatus ;
                        item.IncAcctNarr			= null;//b.IncAcctNarr ;
                        item.ClassCode				= InstrumentAcctDetails.sProductCode ;
                        item.OpeningDate			= _Formatter.ValidateDateReturnNull(InstrumentAcctDetails.sAcctOpenDate);
                        item.IndusSector			= InstrumentAcctDetails.sSector;
                        item.CustType				= InstrumentAcctDetails.sCustomerType; //
                        item.CustNo					= _Formatter.ValIntergers(InstrumentAcctDetails.sCustomerId);
                        item.RsmId					= _Formatter.ValIntergers(InstrumentAcctDetails.sCustomerId);
                        item.CashBalance			= _Formatter.ValDecimal(InstrumentAcctDetails.nCashBalance);
                        
                        var cashAmt = _Formatter.ValDecimal(InstrumentAcctDetails.nBalance) - _Formatter.ValDecimal(InstrumentAcctDetails.nCashBalance);
                        
                        item.CashAmt				= cashAmt;

                        item.City					= InstrumentAcctDetails.sCity ;
                        item.ValUserId				= p.LoginUserId;
                        item.ValErrorCode			= InstrumentAcctDetails.nErrorCode;
                        item.ValErrorMsg			= InstrumentAcctDetails.sErrorText ;
                        item.TransactionDate		= b.TransactionDate;
                        item.ValueDate				= b.ValueDate ;
                        item.DateCreated			= DateTime.Now ;
                        item.ServiceStatus			= b.ServiceStatus;
                        item.Status					= null; 
                        item.DeptId					= b.DeptId ;
                        item.ProcessingDept			= b.ProcessingDept ;
                        item.BatchSeqNo				= b.BatchSeqNo ;
                        item.UserId					= b.UserId ;
                        item.SupervisorId			= null; //b.SupervisorId ;
                        item.Direction				= null; //b. ;
                        item.OriginatingBranchId	= b.OriginatingBranchId ;
                        item.DismissedBy			= null;//b. ;
                        item.DismissedDate			= null;//b. ;
                        item.Rejected				= null;//b. ;
                        item.RejectionIds			= null;//b. ;
                        item.RejectionDate			= null;//b. ;
                        item.ReferenceNo			= b.ReferenceNo;

                        await _repoBatchItems.AddAsync(item);
                        rev = await _ApplicationDbContext.SaveChanges(p.LoginUserId);

                        _repoBatchItemsTemp.Delete(c=> c.ItbId == b.ItbId );

                        int del = await _ApplicationDbContext.SaveChanges(p.LoginUserId);
                        if(del > 0)
                        getNoOfSuccess ++;
                  }

            
                    if(bctInsert > 0)
                    {
                        _repoBatchControlTemp.Delete(c=> c.BatchNo == i.BatchNo );
                        int delBath = await _ApplicationDbContext.SaveChanges(p.LoginUserId);

                        if(delBath > 0 && getNoOfSuccess == transactions.Count())
                        {
                            batchSucess++;

                        }
                    }
                 
                  getNoOfSuccess = 0;
              }

               if(batchSucess == p.ListBatchControl.Count() && rev > 0)
               {
                    ApiResponse.ResponseCode =   0;
                    ApiResponse.ResponseMessage=  "Processed Successfully!";
                        
                    return Ok(ApiResponse);
               }
               else
               {
                    ApiResponse.ResponseCode =   -1;
                    ApiResponse.ResponseMessage=  "Record not Processed Successfully!";
                    return BadRequest(ApiResponse);
               }
            }
            catch(Exception ex)
            {
                var exM = ex;
                 ApiResponse.ResponseMessage=  "Error Occured!!";
                return BadRequest(ApiResponse);
            }

        }

        
    }
}