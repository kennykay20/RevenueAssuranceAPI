using System.ComponentModel;
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
using System.Web;
using anythingGoodApi.AnythingGood.DATA.Models;
using RevAssuranceApi.AppSettings;
using RevAssuranceApi.TokenGen;
using Dapper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RevAssuranceApi.Helper;
using RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO;
using RevAssuranceWebAPi.AnythingGood.DATA.Models;
using RevAssuranceApi.RevenueAssurance.Repository.Interface;
using RevAssuranceApi.Response;
using RevAssuranceApi.RevenueAssurance.DATA.Models;

namespace RevAssuranceApi.OperationImplemention
{
    
    public class ApproveImplementation
    {
        IRepository<admCharge> _repoadmCharge;
        IRepository<oprServiceCharges> _repooprServiceCharges;
        IRepository<OprInstrument> _repoOprInstrument;
        IRepository<OprToken> _repoOprToken;
        IRepository<OprChqBookRequest> _repoOprChqBookRequest;
        ServiceChargeImplementation _ServiceChargeImplementation;
        AccountValidationImplementation _AccountValidationImplementation;
        IRepository<admBankBranch> _repoadmBankBranch;
        IRepository<admUserProfile> _repoadmUserProfile;
        IRepository<FXRate> _repoFXRate;
        IRepository<admCurrency> _repoadmCurrency;
        Formatter _Formatter = new Formatter();
        IRepository<admService> _repoadmService;
        IRepository<CbsTransaction> _repoCbsTransaction;
        ApplicationDbContext  _ApplicationdbContext;
        ComputeChargesImplementation _ComputeChargesImplementation;
        IConfiguration _configuration;
        IRepository<admClientProfile> _repoadmClientProfile;
        public ApproveImplementation( 
                                          IConfiguration configuration,
                                          IRepository<admCharge> repoadmCharge,
                                          ServiceChargeImplementation ServiceChargeImplementation,
                                          AccountValidationImplementation AccountValidationImplementation,
                                          IRepository<oprServiceCharges> repooprServiceCharges,
                                          IRepository<admBankBranch> repoadmBankBranch,
                                          IRepository<OprInstrument> repoOprInstrument,
                                          IRepository<admUserProfile> repoadmUserProfile,
                                          IRepository<FXRate> repoFXRate,
                                          IRepository<admCurrency> repoadmCurrency,
                                          IRepository<admService> repoadmService,
                                          IRepository<CbsTransaction> repoCbsTransaction,
                                          ApplicationDbContext  ApplicationdbContext,
                                          ComputeChargesImplementation ComputeChargesImplementation,
                                          IRepository<admClientProfile> repoadmClientProfile,
                                          IRepository<OprToken> repoOprToken,
                                          IRepository<OprChqBookRequest> repoOprChqBookRequest
                                          
                                          )
        {
            _repoadmCharge = repoadmCharge;
            _repoOprInstrument = repoOprInstrument;
            _ServiceChargeImplementation = ServiceChargeImplementation;
            _AccountValidationImplementation = AccountValidationImplementation;
            _repooprServiceCharges = repooprServiceCharges;
            _repoadmBankBranch = repoadmBankBranch;
            _repoadmUserProfile = repoadmUserProfile;
            _repoFXRate = repoFXRate;
            _repoadmCurrency = repoadmCurrency;
            _repoadmService = repoadmService;
            _repoCbsTransaction = repoCbsTransaction;
            _ComputeChargesImplementation = ComputeChargesImplementation;
            _configuration = configuration;
            _repoadmClientProfile = repoadmClientProfile;
            _repoOprToken = repoOprToken;
            _ApplicationdbContext = ApplicationdbContext;
            _repoOprChqBookRequest = repoOprChqBookRequest;
        }
        public async Task<List<admCharge>> GetChargeDetails(int? SerbiceId)
        {

            var get = await _repoadmCharge.GetManyAsync(c=> c.ServiceId == SerbiceId);

            return get.ToList();
        }

        private async Task<ApiResponse> PostWithYCHg(int ServiceId, int ServiceItbId,string _userName, int UserId)
        {
             var cbs = new CbsTransaction();
             var ApiResponse = new ApiResponse();
            try 
            {
             string NtValStatus =  _configuration["Statuses:NotValidatedStatus"]; 
             string res = _configuration["Statuses:UnPostedStatus"]; 

             var clientProfile = await _repoadmClientProfile.GetAsync(null);

             var getCharge = await _ServiceChargeImplementation.GetServiceChargesByServIdAndItbId(ServiceId, ServiceItbId);

             string localIso = string.Empty;
             var getIsLocal = await _repoadmCurrency.GetAsync(p => p.IsLocalCurrency == true);
             string OrigBranch = string.Empty; 

              if (getIsLocal != null)
                {
                    localIso = getIsLocal.IsoCode;
                }
            int save = -1;

            
            
            foreach(var b in getCharge)
            {
                

                int? ProcessingDeptId = 0;
                DateTime? TransactionDate = null;
                DateTime? ValueDate = null;
                Int64 ItbId = 0;
                string AcctBranchNo = string.Empty;
                string ReferenceNo = string.Empty;

                AccountValParam DrAccountValParam = new AccountValParam();
                
                DrAccountValParam.AcctType = b.ChgAcctType;
                DrAccountValParam.AcctNo = b.ChgAcctNo;
                DrAccountValParam.CrncyCode = b.ChgAcctCcy;
                DrAccountValParam.Username = _userName;
                    
                var ValDrAcct = await _AccountValidationImplementation.ValidateAccountCall(DrAccountValParam);

                AccountValParam CrAccountValParam = new AccountValParam();
                
                CrAccountValParam.AcctType = b.IncAcctType;
                CrAccountValParam.AcctNo = b.IncAcctNo;
                CrAccountValParam.CrncyCode = b.ChgAcctCcy;
                CrAccountValParam.Username = _userName;

                var ValCrAcct = await _AccountValidationImplementation.ValidateAccountCall(DrAccountValParam);

                var admCharge = await _repoadmCharge.GetAsync(c=> c.ServiceId == b.ServiceId);
                var oprServiceModel = await _repoadmService.GetAsync(c=> c.ServiceId == b.ServiceId); // OprServiceModel.getOprByServidId((int)oprAPG.ServiceId);
                var getBrachCodeDr = new admBankBranch();
                var getBrachCodeCr = new admBankBranch();

                int CrBranch = Convert.ToInt32(ValCrAcct.nBranch);
                if(b.ServiceId == 19) {

                    var getService = await _repoOprInstrument.GetAsync(c=> c.ItbId == b.ServiceItbId);
                    
                    getBrachCodeDr = await  _repoadmBankBranch.GetAsync(c=> c.BranchNo == getService.BranchNo);
                    getBrachCodeCr = await  _repoadmBankBranch.GetAsync(c=> c.BranchNo == CrBranch);

                    OrigBranch = getService.BranchNo != null ? getService.BranchNo.ToString() : null;
                    ProcessingDeptId = (int)getService.ProcessingDeptId;
                    TransactionDate = getService.TransactionDate;
                    ValueDate = getService.ValueDate;
                    ItbId = (int)getService.ItbId;
                    AcctBranchNo = getService.BranchNo.ToString();
                    ReferenceNo = getService.ReferenceNo;
                }

           var getUser = await _repoadmUserProfile.GetAsync(c=> c.LoginId == "System");

           decimal? @vnLclEquivExchRate = 0;
           if (b.ChgAcctCcy  != localIso)
           {
              
               var getEqu = await  _repoFXRate.GetAsync(p => (p.MajorCcy == b.ChgAcctCcy || p.MinorCcy == b.ChgAcctCcy) && (p.MajorCcy == localIso || p.MinorCcy == localIso));

               @vnLclEquivExchRate = getEqu != null ? getEqu.MidRate : 0;
           }
           else
           {
               @vnLclEquivExchRate = 1;
           }

           cbs.LclEquivExchRate = @vnLclEquivExchRate;
           if (_Formatter.ValDecimal(b.EquivChgAmount.ToString()) != 0 && _Formatter.ValDecimal(@vnLclEquivExchRate.ToString()) != 0)
           {
               var cal = b.EquivChgAmount * @vnLclEquivExchRate;
               cbs.LcyEquivAmt = Math.Round((Decimal)cal, 2);
           }
           cbs.DrAcctValUserId = getUser.UserId;
           cbs.DrAcctValErrorCode = ValDrAcct.nErrorCode;
           cbs.DrAcctValErrorMsg = ValCrAcct.sErrorText;
           cbs.CbsChannelId = clientProfile.ChannelId;
           cbs.ProcessingDept = ProcessingDeptId;
           cbs.CrAcctValUserId = getUser.UserId;
           cbs.CrAcctValErrorCode = ValCrAcct.nErrorCode;
           cbs.CrAcctValErrorMsg = ValCrAcct.sErrorText;

           cbs.ServiceId = b.ServiceId;
           cbs.DrAcctBranchCode = ValDrAcct.nBranch;// getBrachCodeDr != null ? getBrachCodeDr.BranchCode : null;
           cbs.DrAcctNo = b.ChgAcctCcy;
           cbs.DrAcctType = b.ChgAcctType;
           cbs.DrAcctName = b.ChgAcctName;
           cbs.DrAcctBalance = b.ChgAvailBal;
           cbs.DrAcctStatus = b.ChgAcctStatus;
           cbs.DrAcctTC = b.ChgAcctType != "GL" ? oprServiceModel.CustAcctDrTC : oprServiceModel.GLAcctDrTC;
           var getTranseRef = await _ComputeChargesImplementation.GenTranRef(b.ServiceId);
           cbs.TransReference = getTranseRef.nReference;
           cbs.DrAcctNarration = b.ChgNarration;
           cbs.DrAcctAddress = ValDrAcct.sAddress;// NULL;--To be updated after Dr Acct Sucessful Validation
           cbs.DrAcctClassCode = ValDrAcct.sProductCode;// NULL;--To be updated after Dr Acct Sucessful Validation

           cbs.DrAcctChargeCode = null;
           cbs.DrAcctChargeAmt = null;
           cbs.DrAcctTaxAmt = null; //; --view
           cbs.DrAcctChargeRate = b.ChargeRate;
           cbs.DrAcctOpeningDate = _Formatter.ValidateDate(ValDrAcct.sAcctOpenDate);// NULL;--To be updated after Dr Acct Sucessful Validation
           cbs.DrAcctIndusSector = ValDrAcct.sSector;// NULL;--To be updated after Dr Acct Sucessful Validation
           cbs.DrAcctCustType = ValDrAcct.sAccountType;// NULL;--To be updated after Dr Acct Sucessful Validation
           cbs.DrAcctCustNo = _Formatter.ValIntergers(ValDrAcct.sCustomerId);// NULL;--To be updated after Dr Acct Sucessful Validation
           cbs.DrAcctCashBalance = _Formatter.ValDecimal(ValDrAcct.nCashBalance);// NULL;--To be updated after Dr Acct Sucessful Validation
                                                                                 //cbs.DrAcctCashAmt = NULL;--To be updated after Dr Acct Sucessful Validation
           cbs.DrAcctCity = ValDrAcct.sCity;// NULL;--To be updated after Dr Acct Sucessful Validation
           cbs.DrAcctIncBranch = ValDrAcct.nBranch;// IncomeBranch;
           cbs.CcyCode = b.ChgAcctCcy;// ChargeAcctCcy;
           cbs.TaxRate = b.TaxRate;// TaxRate; --view
           cbs.Amount = b.EquivChgAmount;// EquivChargeAmount;

          //Credit Session

           cbs.CrAcctBranchCode = ValCrAcct.nBranch;// getBrachCodeCr != null ? getBrachCodeCr.BranchCode : null; //NULL;--To be updated after Cr Acct Sucessful Validation
           cbs.CrAcctNo = b.IncAcctNo;// ChargeIncAcctNo;--View
           cbs.CrAcctType = b.IncAcctType; // ChargeIncAcctType;--View
           cbs.CrAcctName = b.IncAcctName;//.IncomeAcctName2;// ChargeIncAcctName;--View
           cbs.CrAcctBalance = _Formatter.ValDecimal(ValCrAcct.nBalance);// Null;--To be updated after Cr Acct Sucessful Validation
           cbs.CrAcctStatus = ValCrAcct.sStatus;// Null;--To be updated after Cr Acct Sucessful Validation
           cbs.CrAcctTC = b.IncAcctType != "GL" ? oprServiceModel.CustAcctCrTC : oprServiceModel.GLAcctCrTC;

           cbs.CrAcctNarration = b.ChgNarration;// IncomeAcctNarr;
           cbs.CrAcctAddress = ValCrAcct.sAddress;// NULL;--To be updated after Cr Acct Sucessful Validation
           cbs.CrAcctProdCode = ValCrAcct.sProductCode;// NULL; --To be updated after Cr Acct Sucessful Validation

           cbs.CrAcctChargeCode = null;// NULL; --To be passed if Credit is the charge Account
           cbs.CrAcctChargeAmt = null;// NULL;--To be passed if Credit is the charge Account
           cbs.CrAcctTaxAmt = null;// NULL; --To be passed if Credit is the charge Account
           cbs.CrAcctChargeRate = null;// NULL; --To be passed if Credit is the charge Account
           cbs.CrAcctChargeNarr = null;// NULL; --To be passed if Credit is the charge Account
           cbs.CrAcctOpeningDate = _Formatter.ValidateDate(ValCrAcct.sAcctOpenDate); ;// NULL; --To be updated after Cr Acct Sucessful Validation
           cbs.CrAcctIndusSector = ValCrAcct.sSector;// NULL;--To be updated after Cr Acct Sucessful Validation
                                                      //cbs.CrAcctCbsTranId = NULL;--To be updated after Cr Acct Sucessful Posting
           cbs.CrAcctCustType = ValCrAcct.sAccountType;// NULL;--To be updated after Cr Acct Sucessful Validation
           cbs.CrAcctCustNo = _Formatter.ValIntergers(ValCrAcct.sCustomerId);// NULL;--To be updated after Cr Acct Sucessful Validation
           cbs.CrAcctCashBalance = _Formatter.ValDecimal(ValCrAcct.nCashBalance);// NULL;--To be updated after Cr Acct Sucessful Validation
                                                                                 //cbs.CrAcctCashAmt = NULL;--To be updated after Cr Acct Sucessful Validation
           cbs.CrAcctCity = ValCrAcct.sCity;// NULL;--To be updated after Cr Acct Sucessful Validation
           cbs.CrAcctIncBranch = ValCrAcct.nBranch;
           cbs.TransactionDate = TransactionDate;// TransactionDate;
           cbs.ValueDate = ValueDate;//  ValueDate;
           if (ValDrAcct.nErrorCode != 0 || ValCrAcct.nErrorCode != 0)
           {
               cbs.Status = NtValStatus;

               if (ValDrAcct.nErrorCode != 0)
               {
                   cbs.DrAcctStatus = ValDrAcct.sErrorText;

               }
               if (ValCrAcct.nErrorCode != 0)
               {
                   cbs.CrAcctStatus = ValCrAcct.sErrorText;
               }
           }
           else
           {
               cbs.Status = res;
           }

         

           cbs.TransTracer = ReferenceNo;
           //cbs.DeptId = oprAPG.ProcessingDeptId;// Update after Post;
           cbs.BatchId = ItbId;
           cbs.BatchSeqNo = 1; //		--Assign 1 to the first entry and increament depending on the number of entries
           cbs.Direction = 1;
           cbs.PrimaryId = ItbId;
           cbs.Reversal = 0;
           cbs.OriginatingBranchId = OrigBranch != string.Empty ? OrigBranch.ToString() : null;
           cbs.ParentTransactionId = null; //NULL;
           cbs.ChannelId = oprServiceModel.Channel;// (select channel from oprService where ServiceId = a.ServiceId)
           cbs.DateCreated = DateTime.Now;

           await _repoCbsTransaction.AddAsync(cbs);
           save = await  _ApplicationdbContext.SaveChanges(UserId); //hard c
           

         }

         if(save > 0){
             ApiResponse.ResponseCode = 0;
             ApiResponse.ResponseMessage = "Approved Successfully!";
         }
            return ApiResponse;

            }
            catch(Exception ex)
            {
                var exM = ex;
            }

            return ApiResponse;
        }
        private async Task<ApiResponse> PostWithNCHg(int ServiceId, int ServiceItbId, string _userName, int UserId)
        {
            
            var ApiResponse = new ApiResponse();
           try
             {
            var clientProfile = await _repoadmClientProfile.GetAsync(null);
            var getCharge = await _ServiceChargeImplementation.GetServiceChargesByServIdAndItbId(ServiceId, ServiceItbId);

            string localIso = string.Empty;
            var getIsLocal = await _repoadmCurrency.GetAsync(p => p.IsLocalCurrency == true);
            string OrigBranch = string.Empty; 
            string res = _configuration["Statuses:UnPostedStatus"]; 
            string NtValStatus =  _configuration["Statuses:NotValidatedStatus"]; 

            if (getIsLocal != null)
            {
                localIso = getIsLocal.IsoCode;
            }
            int save = -1;

            foreach(var b in getCharge)
            {
                var cbs = new CbsTransaction();
                int? ProcessingDeptId = 0;
                DateTime? TransactionDate = null;
                DateTime? ValueDate = null;
                Int64 ItbId = 0;
                string AcctBranchNo = string.Empty;
                string ReferenceNo = string.Empty;
               

                AccountValParam DrAccountValParam = new AccountValParam();
                
                DrAccountValParam.AcctType = b.ChgAcctType;
                DrAccountValParam.AcctNo = b.ChgAcctNo;
                DrAccountValParam.CrncyCode = b.ChgAcctCcy;
                DrAccountValParam.Username = _userName;
                    
                var ValDrAcct = await _AccountValidationImplementation.ValidateAccountCall(DrAccountValParam);

                AccountValParam CrAccountValParam = new AccountValParam();
                
                CrAccountValParam.AcctType = b.IncAcctType;
                CrAccountValParam.AcctNo = b.IncAcctNo;
                CrAccountValParam.CrncyCode = b.ChgAcctCcy;
                CrAccountValParam.Username = _userName;

                var ValCrAcct = await _AccountValidationImplementation.ValidateAccountCall(DrAccountValParam);

                var admCharge = await _repoadmCharge.GetAsync(c=> c.ServiceId == b.ServiceId);
                var oprServiceModel = await _repoadmService.GetAsync(c=> c.ServiceId == b.ServiceId); // OprServiceModel.getOprByServidId((int)oprAPG.ServiceId);
                var getBrachCodeDr = new admBankBranch();
                var getBrachCodeCr = new admBankBranch();

                int CrBranch = Convert.ToInt32(ValCrAcct.nBranch);

                //APG Start Here
                if(b.ServiceId == 19) {

                    var getService = await _repoOprInstrument.GetAsync(c=> c.ItbId == b.ServiceItbId);
                    
                    getBrachCodeDr = await  _repoadmBankBranch.GetAsync(c=> c.BranchNo == getService.BranchNo);
                    getBrachCodeCr = await  _repoadmBankBranch.GetAsync(c=> c.BranchNo == CrBranch);

                    OrigBranch = getService.BranchNo != null ? getService.BranchNo.ToString() : null;
                    ProcessingDeptId = (int)getService.ProcessingDeptId;
                    TransactionDate = getService.TransactionDate;
                    ValueDate = getService.ValueDate;
                    ItbId = (int)getService.ItbId;
                    AcctBranchNo = getService.BranchNo.ToString();
                    ReferenceNo = getService.ReferenceNo;
                }

                //Apg End Here
               //Token
                if(b.ServiceId == 1) {

                    var getService = await _repoOprToken.GetAsync(c=> c.ItbId == b.ServiceItbId);
                    
                    getBrachCodeDr = await  _repoadmBankBranch.GetAsync(c=> c.BranchNo == getService.BranchNo);
                    getBrachCodeCr = await  _repoadmBankBranch.GetAsync(c=> c.BranchNo == CrBranch);

                    OrigBranch = getService.BranchNo != null ? getService.BranchNo.ToString() : null;
                    ProcessingDeptId = (int)getService.ProcessingDeptId;
                    TransactionDate = getService.TransactionDate;
                    ValueDate = getService.ValueDate;
                    ItbId = getService.ItbId;
                    AcctBranchNo = getService.BranchNo.ToString();
                    ReferenceNo = getService.ReferenceNo;
                }
               //Check book Req
                if(b.ServiceId == 2) {

                    var getService = await _repoOprChqBookRequest.GetAsync(c=> c.ItbId == b.ServiceItbId);
                    
                    getBrachCodeDr = await  _repoadmBankBranch.GetAsync(c=> c.BranchNo == getService.BranchNo);
                    getBrachCodeCr = await  _repoadmBankBranch.GetAsync(c=> c.BranchNo == CrBranch);

                    OrigBranch = getService.BranchNo != null ? getService.BranchNo.ToString() : null;
                    ProcessingDeptId = (int)getService.ProcessingDeptId;
                    TransactionDate = getService.TransactionDate;
                    ValueDate = getService.ValueDate;
                    ItbId = getService.ItbId;
                    AcctBranchNo = getService.BranchNo.ToString();
                    ReferenceNo = getService.ReferenceNo;
                }

           // CrAcctTC

           decimal? @vnLclEquivExchRate = 0;
           if (b.ChgAcctCcy != localIso)
           {
               
               var getEqu = await  _repoFXRate.GetAsync(p => (p.MajorCcy == b.ChgAcctCcy || p.MinorCcy == b.ChgAcctCcy) && (p.MajorCcy == localIso || p.MinorCcy == localIso));

               @vnLclEquivExchRate = getEqu != null ? getEqu.MidRate : 0;
           }
           else
           {
               @vnLclEquivExchRate = 1;
           }

           cbs.LclEquivExchRate = @vnLclEquivExchRate;
           if (_Formatter.ValDecimal(b.EquivChgAmount.ToString()) != 0 && _Formatter.ValDecimal(@vnLclEquivExchRate.ToString()) != 0)
           {
               var cal = b.EquivChgAmount  * @vnLclEquivExchRate;

               cbs.LcyEquivAmt = Math.Round((Decimal)cal, 2);
           }
           cbs.ServiceId = b.ServiceId;
           cbs.DrAcctBranchCode = ValDrAcct.nBranch;// getBrachCodeDr != null ? getBrachCodeDr.BranchCode : null;
           cbs.DrAcctNo = b.ChgAcctNo;
           cbs.DrAcctType = b.ChgAcctType;
           cbs.DrAcctName = b.ChgAcctName;
           cbs.DrAcctBalance = b.ChgAvailBal;
           cbs.DrAcctStatus = b.ChgAcctStatus;
           cbs.DrAcctTC = b.ChgAcctType != "GL" ? oprServiceModel.CustAcctDrTC : oprServiceModel.GLAcctDrTC;
           cbs.DrAcctNarration = b.ChgNarration;
           cbs.DrAcctChargeNarr = b.ChgNarration;
           var getTranseRef = await _ComputeChargesImplementation.GenTranRef(b.ServiceId);
           cbs.TransReference =  getTranseRef.nReference;

           cbs.ProcessingDept = ProcessingDeptId;

           var getUser = await _repoadmUserProfile.GetAsync(c=> c.LoginId == "System");


           cbs.DrAcctValUserId = getUser != null ? getUser.UserId : 0;
           cbs.DrAcctValErrorCode = ValDrAcct.nErrorCode;
           cbs.DrAcctValErrorMsg = ValCrAcct.sErrorText;


           cbs.CrAcctValUserId = getUser != null ? getUser.UserId : 0;
           cbs.CrAcctValErrorCode = ValCrAcct.nErrorCode;
           cbs.CrAcctValErrorMsg = ValCrAcct.sErrorText;
           //Cr side

           cbs.DrAcctAddress = ValDrAcct.sAddress;
           cbs.DrAcctClassCode = ValDrAcct.sProductCode;
           //cbs.DrAcctChargeCode = oprAPG.ChargeCode2;
           //cbs.DrAcctChargeAmt = oprAPG.EquivChargeAmount2;
           //cbs.DrAcctTaxAmt = oprAPG.TaxAmount2;
           //cbs.DrAcctChargeRate = oprAPG.ChargeRate2;

           cbs.CbsChannelId = clientProfile.ChannelId;;
           //Val 
           cbs.DrAcctOpeningDate = _Formatter.ValidateDate(ValDrAcct.sAcctOpenDate);
           cbs.DrAcctIndusSector = ValDrAcct.sSector;
           cbs.DrAcctCustType = ValDrAcct.sAccountType;
           cbs.DrAcctCustNo = _Formatter.ValIntergers(ValDrAcct.sCustomerId);
           cbs.DrAcctCashBalance = _Formatter.ValDecimal(ValDrAcct.nCashBalance);
           // cbs.DrAcctCashAmt = ValDrAcct.c// Proc Not returning this
           cbs.DrAcctCity = ValDrAcct.sCity;
           cbs.DrAcctIncBranch = ValDrAcct.nBranch;
           cbs.CcyCode = b.ChgAcctCcy;
           cbs.TaxRate = b.TaxRate;
           cbs.Amount = 0;
           cbs.CrAcctBranchCode = ValCrAcct.nBranch;// getBrachCodeCr != null ? getBrachCodeCr.BranchCode : null;
           cbs.CrAcctNo = b.IncAcctNo;
           cbs.CrAcctType = b.IncAcctType;
           cbs.CrAcctName = b.IncAcctName; ;
           cbs.CrAcctBalance = _Formatter.ValDecimal(ValCrAcct.nBalance);
           cbs.CrAcctStatus = ValCrAcct.sStatus;
           cbs.CrAcctTC = b.IncAcctType != "GL" ? oprServiceModel.CustAcctCrTC : oprServiceModel.GLAcctCrTC;
           cbs.CrAcctNarration = b.ChgNarration;
           cbs.CrAcctAddress = ValCrAcct.sAddress;
           cbs.CrAcctProdCode = ValCrAcct.sProductCode;
           cbs.CrAcctChargeCode = null;// NULL; --To be passed if Credit is the charge Account
           cbs.CrAcctChargeAmt = null; //NULL;--To be passed if Credit is the charge Account
           cbs.CrAcctTaxAmt = null;// NULL; --To be passed if Credit is the charge Account
           cbs.CrAcctChargeRate = null;// NULL; --To be passed if Credit is the charge Account
           cbs.CrAcctChargeNarr = null;// NULL; --To be passed if Credit is the charge Account
           cbs.CrAcctOpeningDate = _Formatter.ValidateDate(ValCrAcct.sAcctOpenDate);// NULL; --To be updated after Cr Acct Sucessful Validation
           cbs.CrAcctIndusSector = ValCrAcct.sSector;// NULL;--To be updated after Cr Acct Sucessful Validation
           cbs.CrAcctCustType = ValCrAcct.sAccountType;
           cbs.CrAcctCustNo = _Formatter.ValIntergers(ValCrAcct.sCustomerId);// NULL,--To be updated after Cr Acct Sucessful Validation
           cbs.CrAcctCashBalance = _Formatter.ValDecimal(ValCrAcct.nCashBalance);// NULL,--To be updated after Cr Acct Sucessful Validation
                                                                                 // cbs.CrAcctCashAmt = ValCrAcct.nCashBalance;// NULL,--To be updated after Cr Acct Sucessful Validation
           cbs.CrAcctCity = ValCrAcct.sCity;// NULL,--To be updated after Cr Acct Sucessful Validation
           cbs.CrAcctIncBranch = ValCrAcct.nBranch;
           cbs.TransactionDate = TransactionDate;
           cbs.ValueDate = ValueDate;
           // cbs.Status = 'Not Validated',

           if (ValDrAcct.nErrorCode != 0 || ValCrAcct.nErrorCode != 0)
           {
               cbs.Status = NtValStatus;

               if (ValDrAcct.nErrorCode != 0)
               {
                   cbs.DrAcctStatus = ValDrAcct.sErrorText;

               }
               if (ValCrAcct.nErrorCode != 0)
               {
                   cbs.CrAcctStatus = ValCrAcct.sErrorText;
               }
           }
           else
           {
               cbs.Status = res; 
           }

           cbs.TransTracer = ReferenceNo;
           cbs.DeptId = null;  // would be updated after Post
           cbs.BatchId = ItbId;
           cbs.BatchSeqNo = 1;// oprAPG.ServiceId --Assign 1 to the first entry and increament depending on the number of entries
           cbs.Direction = 1;
           cbs.PrimaryId = ItbId;
           cbs.Reversal = 0;
           cbs.OriginatingBranchId = AcctBranchNo;
           cbs.ParentTransactionId = null;
           cbs.ChannelId = oprServiceModel.Channel;// (select channel from oprService where ServiceId = a.ServiceId)
          
           cbs.ProcessingDept = ProcessingDeptId;
           cbs.ServiceId = b.ServiceId;
           cbs.DrAcctNo = b.ChgAcctNo;
           cbs.DrAcctType = b.ChgAcctType;
           cbs.DrAcctName = b.ChgAcctName;
           cbs.DrAcctBalance = b.ChgAvailBal;
           cbs.DrAcctStatus = b.ChgAcctStatus;
           cbs.Direction = 1;
           cbs.Reversal = 0;
           cbs.ParentTransactionId = null;
           cbs.DateCreated = DateTime.Now;
          
              await _repoCbsTransaction.AddAsync(cbs);
           save = await  _ApplicationdbContext.SaveChanges(UserId); //hard c
         }

         if(save > 0){
             ApiResponse.ResponseCode = 0;
             ApiResponse.ResponseMessage = "Approved Successfully!";
         }
        return ApiResponse;


        
        }
            catch(Exception ex)
            {
                var exM = ex;
            }

          return ApiResponse;;


        }
        
        private async Task<ApiResponse> TaxPostWithY(int ServiceId, int ServiceItbId, string _userName, int UserId)
        {
           var cbs = new CbsTransaction();
           var ApiResponse = new ApiResponse();
           string NtValStatus =  _configuration["Statuses:NotValidatedStatus"]; 
             string res = _configuration["Statuses:UnPostedStatus"]; 

             var clientProfile = await _repoadmClientProfile.GetAsync(null);

             var getCharge = await _ServiceChargeImplementation.GetServiceChargesByServIdAndItbId(ServiceId, ServiceItbId);

             string localIso = string.Empty;
             var getIsLocal = await _repoadmCurrency.GetAsync(p => p.IsLocalCurrency == true);
             string OrigBranch = string.Empty; 

              if (getIsLocal != null)
                {
                    localIso = getIsLocal.IsoCode;
                }
            int save = -1;

            foreach(var b in getCharge)
            {
                 int? ProcessingDeptId = 0;
                DateTime? TransactionDate = null;
                DateTime? ValueDate = null;
                Int64 ItbId = 0;
                string AcctBranchNo = string.Empty;
               string ReferenceNo = string.Empty;

                AccountValParam DrAccountValParam = new AccountValParam();
                
                DrAccountValParam.AcctType = b.ChgAcctType;
                DrAccountValParam.AcctNo = b.ChgAcctNo;
                DrAccountValParam.CrncyCode = b.ChgAcctCcy;
                DrAccountValParam.Username = _userName;
                    
                var ValDrAcct = await _AccountValidationImplementation.ValidateAccountCall(DrAccountValParam);

                AccountValParam CrAccountValParam = new AccountValParam();
                
                CrAccountValParam.AcctType = b.IncAcctType;
                CrAccountValParam.AcctNo = b.IncAcctNo;
                CrAccountValParam.CrncyCode = b.ChgAcctCcy;
                CrAccountValParam.Username = _userName;

                var ValCrAcct = await _AccountValidationImplementation.ValidateAccountCall(DrAccountValParam);

                var getBrachCodeDr = new admBankBranch();
                var getBrachCodeCr = new admBankBranch();
                  int CrBranch = Convert.ToInt32(ValCrAcct.nBranch);
                // apg
                if(b.ServiceId == 19) {

                    var getService = await _repoOprInstrument.GetAsync(c=> c.ItbId == b.ServiceItbId);
                    
                    getBrachCodeDr = await  _repoadmBankBranch.GetAsync(c=> c.BranchNo == getService.BranchNo);
                    getBrachCodeCr = await  _repoadmBankBranch.GetAsync(c=> c.BranchNo == CrBranch);

                    OrigBranch = getService.BranchNo != null ? getService.BranchNo.ToString() : null;
                    ProcessingDeptId = (int)getService.ProcessingDeptId;
                    TransactionDate = getService.TransactionDate;
                    ValueDate = getService.ValueDate;
                    ItbId = (int)getService.ItbId;
                    AcctBranchNo = getService.BranchNo.ToString();
                    ReferenceNo = getService.ReferenceNo;
                }
                 //Token request
                  if(b.ServiceId == 1) {

                    var getService = await _repoOprToken.GetAsync(c=> c.ItbId == b.ServiceItbId);
                    
                    getBrachCodeDr = await  _repoadmBankBranch.GetAsync(c=> c.BranchNo == getService.BranchNo);
                    getBrachCodeCr = await  _repoadmBankBranch.GetAsync(c=> c.BranchNo == CrBranch);

                    OrigBranch = getService.BranchNo != null ? getService.BranchNo.ToString() : null;
                    ProcessingDeptId = (int)getService.ProcessingDeptId;
                    TransactionDate = getService.TransactionDate;
                    ValueDate = getService.ValueDate;
                    ItbId = (int)getService.ItbId;
                    AcctBranchNo = getService.BranchNo.ToString();
                    ReferenceNo = getService.ReferenceNo;
                }
               
               
                //check Book request
                  if(b.ServiceId == 2) {

                    var getService = await _repoOprChqBookRequest.GetAsync(c=> c.ItbId == b.ServiceItbId);
                    
                    getBrachCodeDr = await  _repoadmBankBranch.GetAsync(c=> c.BranchNo == getService.BranchNo);
                    getBrachCodeCr = await  _repoadmBankBranch.GetAsync(c=> c.BranchNo == CrBranch);

                    OrigBranch = getService.BranchNo != null ? getService.BranchNo.ToString() : null;
                    ProcessingDeptId = (int)getService.ProcessingDeptId;
                    TransactionDate = getService.TransactionDate;
                    ValueDate = getService.ValueDate;
                    ItbId = (int)getService.ItbId;
                    AcctBranchNo = getService.BranchNo.ToString();
                    ReferenceNo = getService.ReferenceNo;
                }

                var getUser = await _repoadmUserProfile.GetAsync(c=> c.LoginId == "System");
                decimal? @vnLclEquivExchRate = 0;
                if (b.ChgAcctCcy != localIso)
                {
                     var getEqu = await  _repoFXRate.GetAsync(p => (p.MajorCcy == b.ChgAcctCcy || p.MinorCcy == b.ChgAcctCcy) && (p.MajorCcy == localIso || p.MinorCcy == localIso));

                        @vnLclEquivExchRate = getEqu != null ? getEqu.MidRate : 0;
                }
                else
                {
                    @vnLclEquivExchRate = 1;
                }

                cbs.LclEquivExchRate = @vnLclEquivExchRate;


           if (_Formatter.ValDecimal(b.EquivChgAmount.ToString()) != 0 && _Formatter.ValDecimal(@vnLclEquivExchRate.ToString()) != 0)
           {
               var cal = b.TaxAmount * @vnLclEquivExchRate;

               cbs.LcyEquivAmt = Math.Round((Decimal)cal, 2);
           }

            var oprServiceModel = await _repoadmService.GetAsync(c=> c.ServiceId == b.ServiceId);

            cbs.ProcessingDept = ProcessingDeptId;
            cbs.DrAcctValUserId = getUser != null ? getUser.UserId : 0;
            cbs.DrAcctValErrorCode = ValDrAcct.nErrorCode;
            cbs.DrAcctValErrorMsg = ValCrAcct.sErrorText;
            var getTranseRef = await _ComputeChargesImplementation.GenTranRef(b.ServiceId);
            cbs.TransReference = getTranseRef.nReference;

           cbs.CrAcctValUserId = getUser != null ? getUser.UserId : 0;
           cbs.CrAcctValErrorCode = ValCrAcct.nErrorCode;
           cbs.CrAcctValErrorMsg = ValCrAcct.sErrorText;
           cbs.CbsChannelId = clientProfile.ChannelId;
           cbs.ServiceId = b.ServiceId;
           cbs.DrAcctBranchCode = ValDrAcct.nBranch;// getBrachCodeDr != null ? getBrachCodeDr.BranchCode : null;
          // cbs.DrAcctNo = oprAPG.ChargeAcctNo;
           cbs.DrAcctType = ValDrAcct.sAccountType;// ChargeAcctType;
           cbs.DrAcctName = ValDrAcct.sName;// ChargeAcctName;
           cbs.DrAcctBalance = ValDrAcct.nBalanceDec;// ChargeAvailBal;
           cbs.DrAcctStatus = ValDrAcct.sStatus;// ChargeAcctStatus;
           //cbs.DrAcctTC = oprAPG.ChargeAcctType != "GL" ? oprServiceModel.CustAcctDrTC : oprServiceModel.GLAcctDrTC;
           cbs.DrAcctNarration = b.TaxNarration;
           cbs.DrAcctAddress = ValDrAcct.sAddress;// NULL;--To be updated after Dr Acct Sucessful Validation
           cbs.DrAcctClassCode = ValDrAcct.sProductCode;// NULL;--To be updated after Dr Acct Sucessful Validation
                                                        // cbs.DrAcctChargeCode = oprAPG NULL;--
                                                        //  cbs.DrAcctChargeAmt = NULL;
           cbs.DrAcctTaxAmt = b.TaxAmount;//. NULL; --view
           cbs.DrAcctChargeRate = b.ChargeRate;// ChargeRate;
                                                    // cbs.DrAcctChargeNarr = NULL;
           cbs.DrAcctOpeningDate = _Formatter.ValidateDate(ValDrAcct.sAcctOpenDate);// NULL;--To be updated after Dr Acct Sucessful Validation
           cbs.DrAcctIndusSector = ValDrAcct.sSector;// NULL;--To be updated after Dr Acct Sucessful Validation
           // cbs.DrAcctCbsTranId = NULL;--To be updated after Dr Acct Sucessful Posting
           cbs.DrAcctCustType = ValDrAcct.sAccountType;// NULL;--To be updated after Dr Acct Sucessful Validation
           cbs.DrAcctCustNo = _Formatter.ValIntergers(ValDrAcct.sCustomerId);// NULL;--To be updated after Dr Acct Sucessful Validation
           cbs.DrAcctCashBalance = _Formatter.ValDecimal(ValDrAcct.nCashBalance);// NULL;--To be updated after Dr Acct Sucessful Validation
           // cbs.DrAcctCashAmt = NULL;--To be updated after Dr Acct Sucessful Validation
           cbs.DrAcctCity = ValDrAcct.sCity;// NULL;--To be updated after Dr Acct Sucessful Validation
           cbs.DrAcctIncBranch = b.IncBranch  != null ?_Formatter.ToString() : null;
           cbs.CcyCode = b.ChgAcctCcy;
           cbs.TaxRate = b.TaxRate; //--view
           cbs.Amount = b.TaxAmount;
           cbs.CrAcctBranchCode = ValCrAcct.nBranch;// = NULL;--To be updated after Cr Acct Sucessful Validation
           cbs.CrAcctNo = b.TaxAcctNo;
           cbs.CrAcctType = b.TaxAcctType;// TaxAcctType;
           cbs.CrAcctName = ValCrAcct.sName;// NULL;--To be updated after Cr Acct Sucessful Validation
           cbs.CrAcctBalance = ValCrAcct.nBalanceDec; // Null;--To be updated after Cr Acct Sucessful Validation
           cbs.CrAcctStatus = ValCrAcct.sStatus;// Null;--To be updated after Cr Acct Sucessful Validation
           cbs.CrAcctTC = b.IncAcctType  != "GL" ? oprServiceModel.CustAcctCrTC : oprServiceModel.GLAcctCrTC;
           cbs.CrAcctNarration = b.TaxNarration + " " + ReferenceNo;
           cbs.CrAcctAddress = ValCrAcct.sAddress;// NULL;--To be updated after Cr Acct Sucessful Validation
           cbs.CrAcctProdCode = ValCrAcct.sProductCode;// NULL; --To be updated after Cr Acct Sucessful Validation
           //cbs.CrAcctChargeCode =  NULL; --To be passed if Credit is the charge Account
           //cbs.CrAcctChargeAmt = NULL;--To be passed if Credit is the charge Account
           //cbs.CrAcctTaxAmt = NULL; --To be passed if Credit is the charge Account
           //cbs.CrAcctChargeRate = NULL; --To be passed if Credit is the charge Account
           //cbs.CrAcctChargeNarr = NULL; --To be passed if Credit is the charge Account
           cbs.CrAcctOpeningDate = _Formatter.ValidateDate(ValCrAcct.sAcctOpenDate);// NULL; --To be updated after Cr Acct Sucessful Validation
           cbs.CrAcctIndusSector = ValCrAcct.sSector;// NULL;--To be updated after Cr Acct Sucessful Validation
           //cbs.CrAcctCbsTranId = NULL;--To be updated after Cr Acct Sucessful Posting
           cbs.CrAcctCustType = ValCrAcct.sAccountType;// NULL;--To be updated after Cr Acct Sucessful Validation
           cbs.CrAcctCustNo = _Formatter.ValIntergers(ValCrAcct.sCustomerId);// NULL;--To be updated after Cr Acct Sucessful Validation
           cbs.CrAcctCashBalance = _Formatter.ValDecimal(ValCrAcct.nCashBalance);// NULL;--To be updated after Cr Acct Sucessful Validation
           //cbs.CrAcctCashAmt = NULL;--To be updated after Cr Acct Sucessful Validation
           cbs.CrAcctCity = ValCrAcct.sCity;//  NULL;--To be updated after Cr Acct Sucessful Validation
           cbs.CrAcctIncBranch = ValCrAcct.nBranch;
           cbs.TransactionDate = TransactionDate;
           cbs.ValueDate = ValueDate;
           if (ValDrAcct.nErrorCode != 0 || ValCrAcct.nErrorCode != 0)
           {
               cbs.Status = NtValStatus;

               if (ValDrAcct.nErrorCode != 0)
               {
                   cbs.DrAcctStatus = ValDrAcct.sErrorText;
               }
               if (ValCrAcct.nErrorCode != 0)
               {
                   cbs.CrAcctStatus = ValCrAcct.sErrorText;
               }
           }
           else
           {
               cbs.Status = res;
           }
           cbs.TransTracer = ReferenceNo;
           //cbs.DeptId = oprAPG.ProcessingDeptId;
           cbs.DeptId = ProcessingDeptId;
           cbs.DeptId = ProcessingDeptId;
           cbs.BatchId = ItbId;
           cbs.BatchSeqNo = 3;			//--Assign 1 to the first entry and increament depending on the number of entries
           cbs.Direction = 1;
           cbs.PrimaryId = ItbId;
           cbs.Reversal = 0;
           cbs.OriginatingBranchId = AcctBranchNo;
           cbs.DateCreated = DateTime.Now;// cbs.ParentTransactionId = NULL;
           cbs.ChannelId = oprServiceModel.Channel;
           await _repoCbsTransaction.AddAsync(cbs);
           save = await  _ApplicationdbContext.SaveChanges(UserId);
        }
         if(save > 0){
             ApiResponse.ResponseCode = 0;
             ApiResponse.ResponseMessage = "Approved Successfully!";
         }
            return ApiResponse;
        }
         private async Task<ApiResponse> ContingentIsY(int ServiceId, int ServiceItbId, string _userName, int UserId)
        {
             var cbs = new CbsTransaction();
            var ApiResponse = new ApiResponse();
           string NtValStatus =  _configuration["Statuses:NotValidatedStatus"]; 
             string res = _configuration["Statuses:UnPostedStatus"]; 

             var clientProfile = await _repoadmClientProfile.GetAsync(null);

             var getCharge = await _ServiceChargeImplementation.GetServiceChargesByServIdAndItbId(ServiceId, ServiceItbId);

             string localIso = string.Empty;
             var getIsLocal = await _repoadmCurrency.GetAsync(p => p.IsLocalCurrency == true);
             string OrigBranch = string.Empty; 

              if (getIsLocal != null)
                {
                    localIso = getIsLocal.IsoCode;
                }

            int save = -1;
            foreach(var b in getCharge)
            {
                 int? ProcessingDeptId = 0;
                DateTime? TransactionDate = null;
                DateTime? ValueDate = null;
                Int64 ItbId = 0;
                string AcctBranchNo = string.Empty;
               string ReferenceNo = string.Empty;

                AccountValParam DrAccountValParam = new AccountValParam();
                
                DrAccountValParam.AcctType = b.ChgAcctType;
                DrAccountValParam.AcctNo = b.ChgAcctNo;
                DrAccountValParam.CrncyCode = b.ChgAcctCcy;
                DrAccountValParam.Username = _userName;
                    
                var ValDrAcct = await _AccountValidationImplementation.ValidateAccountCall(DrAccountValParam);

                AccountValParam CrAccountValParam = new AccountValParam();
                
                CrAccountValParam.AcctType = b.IncAcctType;
                CrAccountValParam.AcctNo = b.IncAcctNo;
                CrAccountValParam.CrncyCode = b.ChgAcctCcy;
                CrAccountValParam.Username = _userName;

                var ValCrAcct = await _AccountValidationImplementation.ValidateAccountCall(DrAccountValParam);

                var getBrachCodeDr = new admBankBranch();
                var getBrachCodeCr = new admBankBranch();
                var getService = new OprInstrument();
                  int CrBranch = Convert.ToInt32(ValCrAcct.nBranch);
           
                 var getUser = await _repoadmUserProfile.GetAsync(c=> c.LoginId == "System");

                   if(b.ServiceId == 19) {

                     getService = await _repoOprInstrument.GetAsync(c=> c.ItbId == b.ServiceItbId);
                    
                    getBrachCodeDr = await  _repoadmBankBranch.GetAsync(c=> c.BranchNo == getService.BranchNo);
                    getBrachCodeCr = await  _repoadmBankBranch.GetAsync(c=> c.BranchNo == CrBranch);

                    OrigBranch = getService.BranchNo != null ? getService.BranchNo.ToString() : null;
                    ProcessingDeptId = (int)getService.ProcessingDeptId;
                    TransactionDate = getService.TransactionDate;
                    ValueDate = getService.ValueDate;
                    ItbId = (int)getService.ItbId;
                    AcctBranchNo = getService.BranchNo.ToString();
                    ReferenceNo = getService.ReferenceNo;
                }




            decimal? @vnLclEquivExchRate = 0;
            if (getService.ContDrAcctType != getService.InstrumentCcy)
            {
               // var getEqu = await FxRateModel.ConForLclEquivExchRate(oprAPG.ChargeAcctCcy, oprAPG.InstrumentCcy);

               // @vnLclEquivExchRate = getEqu != null ? getEqu.MidRate : 0;
            }
            else
            {
                @vnLclEquivExchRate = 1;
            }

            cbs.LclEquivExchRate = @vnLclEquivExchRate;


            //if (validation.ValDecimal(oprAPG.EquivChargeAmount.ToString()) != 0 && validation.ValDecimal(@vnLclEquivExchRate.ToString()) != 0)
            //{
            //    var cal = oprAPG.Amount * @vnLclEquivExchRate;

            //    cbs.LcyEquivAmt = Math.Round((Decimal)cal, 2);
            //}
            var oprServiceModel = await _repoadmService.GetAsync(c=> c.ServiceId == b.ServiceId);
            cbs.ServiceId = b.ServiceId;
            cbs.DrAcctBranchCode = ValDrAcct.nBranch;// getBrachCodeDr != null ? getBrachCodeDr.BranchCode : null;
                                                     //NULL;--To be updated after Dr Acct Sucessful Validation
            cbs.DrAcctNo = getService.ContDrAcctNo;
            cbs.DrAcctType = getService.ContDrAcctType;
            cbs.DrAcctName = ValDrAcct.sName;

            cbs.DrAcctValUserId = getUser != null ? getUser.UserId : 0;
            // cbs.CbsUserId = getUser != null ? getUser.UserId : 0;
            cbs.DrAcctValErrorCode = ValDrAcct.nErrorCode;
            cbs.DrAcctValErrorMsg = ValCrAcct.sErrorText;


            cbs.CrAcctValUserId = getUser != null ? getUser.UserId : 0;
            cbs.CrAcctValErrorCode = ValCrAcct.nErrorCode;
            cbs.CrAcctValErrorMsg = ValCrAcct.sErrorText;

            cbs.ProcessingDept = ProcessingDeptId;

            cbs.CbsChannelId = clientProfile.ChannelId;

            cbs.Amount = getService.Amount;

            cbs.DrAcctBalance = ValDrAcct.nBalanceDec;
            cbs.DrAcctStatus = ValDrAcct.sStatus;//.ContDrAcctStatus;
            cbs.DrAcctTC = getService.ContDrAcctType != "GL" ? oprServiceModel.CustAcctDrTC : oprServiceModel.GLAcctDrTC;
            cbs.DrAcctNarration = getService.ContDrAcctNarration;
            cbs.DrAcctAddress = ValDrAcct.sAddress;// NULL; --To be updated after Dr Acct Sucessful Validation
            cbs.DrAcctClassCode = ValDrAcct.sProductCode;// NULL; --To be updated after Dr Acct Sucessful Validation
            //cbs.DrAcctChargeCode = oprAPG.ChargeCode;//  NULL; --vew
            cbs.DrAcctChargeAmt = 0;
            cbs.DrAcctTaxAmt = 0;
            cbs.DrAcctChargeRate = null;
            cbs.DrAcctChargeNarr = null;
            cbs.DrAcctOpeningDate = _Formatter.ValidateDate(ValDrAcct.sAcctOpenDate);// NULL; --To be updated after Dr Acct Sucessful Validation
            cbs.DrAcctIndusSector = ValDrAcct.sSector;// NULL; --To be updated after Dr Acct Sucessful Validation
            //cbs.DrAcctCbsTranId = NULL; --To be updated after Dr Acct Sucessful Posting
            cbs.DrAcctCustType = ValDrAcct.sAccountType;// NULL; --To be updated after Dr Acct Sucessful Validation
            cbs.DrAcctCustNo = _Formatter.ValIntergers(ValDrAcct.sCustomerId);// NULL; --To be updated after Dr Acct Sucessful Validation
            cbs.DrAcctCashBalance = _Formatter.ValDecimal(ValDrAcct.nCashBalance); //NULL; --To be updated after Dr Acct Sucessful Validation
            //cbs.DrAcctCashAmt = NULL; --To be updated after Dr Acct Sucessful Validation
            cbs.DrAcctCity = ValDrAcct.sCity;// NULL; --To be updated after Dr Acct Sucessful Validation
            cbs.DrAcctIncBranch = ValDrAcct.nBranch;// NULL;
            cbs.CcyCode = getService.ContDrCcyCode;// ContDrCcyCode;
            //cbs.TaxRate = oprAPG.TaxRate;
            cbs.CrAcctBranchCode = ValCrAcct.nBranch;  // NULL;--To be updated after Cr Acct Sucessful Validation

            cbs.CrAcctNo = getService.ContCrAcctNo;
            cbs.CrAcctType = getService.ContCrAcctType; ;// --View
            cbs.CrAcctName = ValCrAcct.sName;//  ContCrAcctName;--View
            cbs.CrAcctBalance = ValCrAcct.nBalanceDec;//. ContCrAvailBal;--To be updated after Cr Acct Sucessful Validation
            cbs.CrAcctStatus = ValCrAcct.sStatus;// ContCrAcctStatus;--To be updated after Cr Acct Sucessful Validation
            cbs.CrAcctTC = getService.ContCrAcctType != "GL" ? oprServiceModel.CustAcctCrTC : oprServiceModel.GLAcctCrTC;
            cbs.CrAcctNarration = getService.ContCrAcctNarration;
            cbs.CrAcctAddress = ValCrAcct.sAddress;// NULL; --To be updated after Cr Acct Sucessful Validation
            cbs.CrAcctProdCode = ValCrAcct.sProductCode;// NULL; --To be updated after Cr Acct Sucessful Validation

            cbs.CrAcctChargeCode = null;// oprAPG NULL; --To be passed if Credit is the charge Account
            cbs.CrAcctChargeAmt = null;// NULL;--To be passed if Credit is the charge Account
            cbs.CrAcctTaxAmt = null;// NULL; --To be passed if Credit is the charge Account
            cbs.CrAcctChargeRate = null;// NULL; --To be passed if Credit is the charge Account
            cbs.CrAcctChargeNarr = null;// NULL; --To be passed if Credit is the charge Account
            cbs.CrAcctOpeningDate = _Formatter.ValidateDate(ValCrAcct.sAcctOpenDate);// NULL; --To be updated after Cr Acct Sucessful Validation
            cbs.CrAcctIndusSector = ValCrAcct.sSector;// NULL;--To be updated after Cr Acct Sucessful Validation
            // cbs.CrAcctCbsTranId = NULL;--To be updated after Cr Acct Sucessful Posting
            cbs.CrAcctCustType = ValCrAcct.sAccountType;// NULL;--To be updated after Cr Acct Sucessful Validation
            cbs.CrAcctCustNo = _Formatter.ValIntergers(ValCrAcct.sCustomerId);// NULL;--To be updated after Cr Acct Sucessful Validation
            cbs.CrAcctCashBalance = _Formatter.ValDecimal(ValCrAcct.nCashBalance);// NULL;--To be updated after Cr Acct Sucessful Validation
            //cbs.CrAcctCashAmt = NULL;--To be updated after Cr Acct Sucessful Validation
            cbs.CrAcctCity = ValCrAcct.sCity;// NULL;--To be updated after Cr Acct Sucessful Validation
            cbs.CrAcctIncBranch = ValCrAcct.nBranch;
            cbs.TransactionDate = TransactionDate;// TransactionDate;
            cbs.ValueDate = ValueDate;// ValueDate;
            if (ValDrAcct.nErrorCode != 0 || ValCrAcct.nErrorCode != 0)
            {
                cbs.Status = NtValStatus;

                if (ValDrAcct.nErrorCode != 0)
                {
                    cbs.DrAcctStatus = ValDrAcct.sErrorText;
                }
                if (ValCrAcct.nErrorCode != 0)
                {
                    cbs.CrAcctStatus = ValCrAcct.sErrorText;
                }
            }
            else
            {
                cbs.Status = res;
            }
            cbs.TransTracer = ReferenceNo;
            var getTranseRef = await _ComputeChargesImplementation.GenTranRef(b.ServiceId);
            cbs.TransReference = getTranseRef.nReference;
           // cbs.DeptId = oprAPG.ProcessingDeptId;// ProcessingDeptId;
            cbs.BatchId = ItbId;
            cbs.BatchSeqNo = 5;//		--Increment depending on the number of Entries to raise
            cbs.Direction = 1;
            cbs.PrimaryId = ItbId;
            cbs.Reversal = 0;
            cbs.OriginatingBranchId = AcctBranchNo;
            //cbs.ParentTransactionId = NULL;
            cbs.ChannelId = oprServiceModel.Channel;// (select channel from oprService where ServiceId = a.ServiceId);
            cbs.DateCreated = DateTime.Now;
            if(cbs.DrAcctType == "GL" && cbs.CrAcctType == "GL")
            {
                cbs.DrAcctChargeCode = null;
                cbs.CrAcctChargeCode = null;
            }
           
           await _repoCbsTransaction.AddAsync(cbs);
           save = await  _ApplicationdbContext.SaveChanges(UserId);
        }
         if(save > 0){
             ApiResponse.ResponseCode = 0;
             ApiResponse.ResponseMessage = "Approved Successfully!";
         }
            return ApiResponse;
   
    }

        public async Task<ApiResponse> GetAllChecked(List<oprServiceCharges> ListoprServiceCharges, string _userName, int UserId)
        {
            ApiResponse ApiResponse = new ApiResponse();
            string UnPostedStatus = _configuration["Statuses:UnPostedStatus"];
            string AuthorizedStatus =  _configuration["Statuses:AuthorizedStatus"]; 
            foreach (var b in ListoprServiceCharges)
            {
                if(b.ServiceId == 19)
                {
                    var admCharge = await _repoadmCharge.GetAsync(c=> c.ServiceId == b.ServiceId);
                    var getRec = await _repoOprInstrument.GetAsync(c=> c.ItbId == b.ServiceItbId);
                    if(b.EquivChgAmount > 0 && admCharge.PostWithTC == "N") 
                    {
                       ApiResponse = await PostWithNCHg((int)b.ServiceId, (int)b.ServiceItbId, _userName, UserId);
                    }  
                    if(b.EquivChgAmount > 0 && admCharge.PostWithTC == "Y") 
                    {
                      ApiResponse =  await PostWithYCHg((int)b.ServiceId, (int)b.ServiceItbId, _userName, UserId);
                    } 

                     if(ApiResponse.ResponseCode == 0)
                    {

                       getRec.Status =  UnPostedStatus;
                       getRec.InstrumentStatus =  AuthorizedStatus;
                       _repoOprInstrument.Update(getRec);
                        int save = await  _ApplicationdbContext.SaveChanges(UserId);
                    }  
                }

               //Token Start Here
                if(b.ServiceId == 1)
                {
                    var admCharge = await _repoadmCharge.GetAsync(c=> c.ServiceId == b.ServiceId);
                    var getRec = await _repoOprToken.GetAsync(c=> c.ItbId == b.ServiceItbId);
                    if(b.EquivChgAmount > 0 && admCharge.PostWithTC == "N") 
                    {
                       ApiResponse = await PostWithNCHg((int)b.ServiceId, (int)b.ServiceItbId, _userName, UserId);
                    }  
                    if(b.EquivChgAmount > 0 && admCharge.PostWithTC == "Y") 
                    {
                      ApiResponse =  await PostWithYCHg((int)b.ServiceId, (int)b.ServiceItbId, _userName, UserId);
                    }   

                    if(ApiResponse.ResponseCode == 0)
                    {
                       getRec.Status =  UnPostedStatus;
                       getRec.ServiceStatus =  AuthorizedStatus;
                       _repoOprToken.Update(getRec);
                        int save = await  _ApplicationdbContext.SaveChanges(UserId);
                    }
                }
               
                 //Check Book Request Start Here
                if(b.ServiceId == 2)
                {
                    var admCharge = await _repoadmCharge.GetAsync(c=> c.ServiceId == b.ServiceId);
                    var getRec = await _repoOprChqBookRequest.GetAsync(c=> c.ItbId == b.ServiceItbId);
                    if(b.EquivChgAmount > 0 && admCharge.PostWithTC == "N") 
                    {
                       ApiResponse = await PostWithNCHg((int)b.ServiceId, (int)b.ServiceItbId, _userName, UserId);
                    }  
                    if(b.EquivChgAmount > 0 && admCharge.PostWithTC == "Y") 
                    {
                      ApiResponse =  await PostWithYCHg((int)b.ServiceId, (int)b.ServiceItbId, _userName, UserId);
                    }   

                    if(ApiResponse.ResponseCode == 0)
                    {
                       getRec.Status =  UnPostedStatus;
                       getRec.ServiceStatus =  AuthorizedStatus;
                       _repoOprChqBookRequest.Update(getRec);
                        int save = await  _ApplicationdbContext.SaveChanges(UserId);
                    }
                }
               
                
            }

            return ApiResponse;
        }

        public async Task<ApiResponse> RejAllChecked(List<oprServiceCharges> ListoprServiceCharges, string _userName, int UserId,string RejIds)
        {
            ApiResponse ApiResponse = new ApiResponse();
            string RejectedStatus = _configuration["Statuses:RejectedStatus"];
            string msg = "Rejected Success";
            
            foreach (var b in ListoprServiceCharges)
            {

                var getoprChg = await  _repooprServiceCharges.GetAsync(c=> c.ItbId == b.ItbId);
                getoprChg.Status = RejectedStatus;
                _repooprServiceCharges.Update(getoprChg);
                int savegetoprChg = await  _ApplicationdbContext.SaveChanges(UserId);

                if(b.ServiceId == 19)
                {
                    var admCharge = await _repoadmCharge.GetAsync(c=> c.ServiceId == b.ServiceId);
                    var getRec = await _repoOprInstrument.GetAsync(c=> c.ItbId == b.ServiceItbId);
                       getRec.Status =  RejectedStatus;
                       getRec.RejectedIds = RejIds;
                       getRec.RejectedBy = UserId;
                       getRec.RejectedDate = DateTime.Now;

                       _repoOprInstrument.Update(getRec);
                        int save = await  _ApplicationdbContext.SaveChanges(UserId);
                         if(save > 0){
                        ApiResponse.ResponseCode = 0;
                        ApiResponse.ResponseMessage = msg;
                    }  
                }

               
               //Token Start Here
                if(b.ServiceId == 1)
                {
                   
                    var getRec = await _repoOprToken.GetAsync(c=> c.ItbId == b.ServiceItbId);
                    
                    
                    getRec.Status =  RejectedStatus;
                    getRec.Status =  RejectedStatus;
                    getRec.RejectionIds = RejIds;
                    getRec.RejectedBy = UserId;
                    getRec.RejectionDate = DateTime.Now;
                    _repoOprToken.Update(getRec);
                    int save = await  _ApplicationdbContext.SaveChanges(UserId);

                    if(save > 0){
                        ApiResponse.ResponseCode = 0;
                        ApiResponse.ResponseMessage = msg;
                    }
                    
                }
               
                
            }

            return ApiResponse;
        }
       
      
    }
}