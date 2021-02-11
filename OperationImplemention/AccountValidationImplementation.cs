
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
using anythingGoodApi.AnythingGood.DATA;
using RevAssuranceApi.OperationImplemention;
using RevAssuranceApi.WebServices;
using Newtonsoft.Json;
using System.Net.Http;
using RevAssuranceApi.RevenueAssurance.DATA.Models;
using RevAssuranceApi.RevenueAssurance.Repository.Interface;

namespace RevAssuranceApi.OperationImplemention
{

    public class AccountValidationImplementation
    {
        IRepository<admCharge> _repoadmCharge;
        ApplicationDbContext _ApplicationDbContext;
        Formatter _Formatter = new Formatter();

        IRepository<admBankBranch> _repoadmBankBranch;
        IRepository<admBankServiceSetup> _repoadmBankServiceSetup;

        FunctionApiSetUpImplementation _FunctionApiSetUpImplementation;

        public AccountValidationImplementation(
                                          IConfiguration configuration,
                                          IRepository<admCharge> repoadmCharge,
                                           ApplicationDbContext ApplicationDbContext,
                                           IRepository<admBankBranch> repoadmBankBranch,
                                           IRepository<admBankServiceSetup> repoadmBankServiceSetup,
                                           FunctionApiSetUpImplementation FunctionApiSetUpImplementation
                                          )
        {
            _repoadmCharge = repoadmCharge;
            _ApplicationDbContext = ApplicationDbContext;
            _repoadmBankBranch = repoadmBankBranch;
            _repoadmBankServiceSetup = repoadmBankServiceSetup;
            _FunctionApiSetUpImplementation = FunctionApiSetUpImplementation;
        }
       public async Task<AccountValidationDTO> ValidateAccountCall(AccountValParam AccountValParam)
       {
          AccountValidationDTO values = new AccountValidationDTO();
            try
            {

             var get = await _FunctionApiSetUpImplementation.GetConnectDetails("ValidateAccount");
                
                var _responseBankServ = await _repoadmBankServiceSetup.GetAsync(c=> c.Itbid == get.ConnectionId);
             var url = _responseBankServ.WebServiceUrl.Trim() + "/api/Services/ValidateAccount";

            AccountValParam.ConnectionStringId = get.ConnectionId;

             var json = JsonConvert.SerializeObject(AccountValParam);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

          
            var client = new HttpClient();

            var response = await client.PostAsync(url, data);

            string result = response.Content.ReadAsStringAsync().Result;
            values =  JsonConvert.DeserializeObject<AccountValidationDTO>(result);
                        if (values != null)
                        {
                            // oValidateAcctResponse.nCheqNo = "200";

                            if(values.sCrncyIso != null)
                            {
                                values.sCrncyIso = values.sCrncyIso.Trim();
                            }

                            if (values.sAccountType != null)
                            {
                                values.sAccountType = values.sAccountType.Trim();
                            }
                            

                            if (values.nBalance != null)
                            {
                                values.nBalanceDec = decimal.Parse(values.nBalance);
                                values.nBalance = _Formatter.FormattedAmount(decimal.Parse(values.nBalance));

                            }
                            if (!string.IsNullOrEmpty(values.nBranch))
                            {
                                int branchNo = Convert.ToInt32(values.nBranch);

                                try
                                {
                                     var brch = await _repoadmBankBranch.GetAsync(c=> c.BranchNo == branchNo);
                                  
                                    if (brch != null)
                                    {
                                        values.sBranchName = brch.BranchName;
                                        values.BranchName = values.sBranchName;  
                                    }
                                }
                                catch(Exception exc)
                                {
                                    var ex1 = exc == null ? exc.InnerException.Message : exc.Message;
                                }
                              
                            }
                            if (!string.IsNullOrEmpty(values.sSector))
                            {
                                values.sSector = values.sSector.Trim();
                              
                            }

                        }
              values.ProductCode  =  values.sProductCode;
              values.sRsmId = _Formatter.ValIntergers(values.sCustomerId) ; 
              return values;
           }
                
            catch (Exception ex)
            {
                 var exM = ex;
                 return null; 
            }

            
        }
       public async Task<AccountValidationDTO> ValidateAccountCallOverDraft(AccountValParam AccountValParam)
       {
          AccountValidationDTO values = new AccountValidationDTO();
            try
            {

             var get = await _FunctionApiSetUpImplementation.GetConnectDetails("ValidateOdAccount");
                
                var _responseBankServ = await _repoadmBankServiceSetup.GetAsync(c=> c.Itbid == get.ConnectionId);
             var url = _responseBankServ.WebServiceUrl.Trim() + "/api/Services/ValidateOdAccount";

            AccountValParam.ConnectionStringId = get.ConnectionId;

            var param = new {
                AcctType = AccountValParam.AcctType,
                AcctNo= AccountValParam.AcctNo,
                Username = AccountValParam.Username,
                ConnectionStringId = get.ConnectionId
            };

             var json = JsonConvert.SerializeObject(param);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

          
            var client = new HttpClient();

            var response = await client.PostAsync(url, data);

            string result = response.Content.ReadAsStringAsync().Result;
            values =  JsonConvert.DeserializeObject<AccountValidationDTO>(result);
            values.sRsmId  = string.IsNullOrWhiteSpace(values.sRsmIdString)  ? 0 : Convert.ToInt32(values.sRsmIdString);
                        if (values != null)
                        {
                            // oValidateAcctResponse.nCheqNo = "200";

                            if(values.sCrncyIso != null)
                            {
                                values.sCrncyIso = values.sCrncyIso.Trim();
                            }

                            if (values.sAccountType != null)
                            {
                                values.sAccountType = values.sAccountType.Trim();
                            }
                            

                            if (values.nBalance != null)
                            {
                                values.nBalanceDec = decimal.Parse(values.nBalance);
                                values.nBalance = _Formatter.FormattedAmount(decimal.Parse(values.nBalance));

                            }
                            if (!string.IsNullOrEmpty(values.nBranch))
                            {
                                int branchNo = Convert.ToInt32(values.nBranch);

                                try
                                {
                                     var brch = await _repoadmBankBranch.GetAsync(c=> c.BranchNo == branchNo);
                                  
                                    if (brch != null)
                                    {
                                        values.sBranchName = brch.BranchName;
                                        values.BranchName = values.sBranchName;  
                                    }
                                }
                                catch(Exception exc)
                                {
                                    var ex1 = exc == null ? exc.InnerException.Message : exc.Message;
                                }
                              
                            }
                            if (!string.IsNullOrEmpty(values.sSector))
                            {
                                values.sSector = values.sSector.Trim();
                              
                            }

                        }
              values.ProductCode  =  values.sProductCode;
            
              return values;
           }
                
            catch (Exception ex)
            {
                 var exM = ex;
                 return null; 
            }

            
        }

     public async Task<CollateralHoldRes> CollateralHold(CollateralCoreBankingDTOParam param)
       {
          CollateralHoldRes values = new CollateralHoldRes();
            try
            {

            var get = await _FunctionApiSetUpImplementation.GetConnectDetails("CollateralHoldAccount");
                
             var _responseBankServ = await _repoadmBankServiceSetup.GetAsync(c=> c.Itbid == get.ConnectionId);
             var url = _responseBankServ.WebServiceUrl.Trim() + "/api/Services/CollateralHoldAccount";
             
             param.ConnectionStringId = get.ConnectionId;
             
             var json = JsonConvert.SerializeObject(param);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

          
            var client = new HttpClient();

            var response = await client.PostAsync(url, data);

            string result = response.Content.ReadAsStringAsync().Result;
            values =  JsonConvert.DeserializeObject<CollateralHoldRes>(result);                  
            return values;
           }
                
            catch (Exception ex)
            {
                 var exM = ex;
                 return null; 
            }

            
        }
    
    

    
    }
}
