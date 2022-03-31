
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

    public class CoreBankingImplementation
    {
        IRepository<admCharge> _repoadmCharge;
        ApplicationDbContext _ApplicationDbContext;
        Formatter _Formatter = new Formatter();

        IRepository<admBankBranch> _repoadmBankBranch;
        IRepository<admBankServiceSetup> _repoadmBankServiceSetup;
        FunctionApiSetUpImplementation _FunctionApiSetUpImplementation;
        public CoreBankingImplementation(
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
           

                var _responseBankServ = await _repoadmBankServiceSetup.GetAsync(c => c.Itbid == get.ConnectionId);
                var url = _responseBankServ.WebServiceUrl.Trim() + "/api/Services/ValidateAccount";

                AccountValParam.ConnectionStringId = get.ConnectionId;

                var json = JsonConvert.SerializeObject(AccountValParam);
                var data = new StringContent(json, Encoding.UTF8, "application/json");


                var client = new HttpClient();

                var response = await client.PostAsync(url, data);

                string result = response.Content.ReadAsStringAsync().Result;
                values = JsonConvert.DeserializeObject<AccountValidationDTO>(result);
                if (values != null)
                {
                    // oValidateAcctResponse.nCheqNo = "200";

                    if (values.sCrncyIso != null)
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
                            var brch = await _repoadmBankBranch.GetAsync(c => c.BranchNo == branchNo);

                            if (brch != null)
                            {
                                values.sBranchName = brch.BranchName;
                            }
                        }
                        catch (Exception exc)
                        {
                            var ex1 = exc == null ? exc.InnerException.Message : exc.Message;
                        }

                    }
                    if (!string.IsNullOrEmpty(values.sSector))
                    {
                        values.sSector = values.sSector.Trim();

                    }

                }

                return values;
            }

            catch (Exception ex)
            {
                var exM = ex;
                return null;
            }


        }

        public async Task<UserLimit> CoreBankingLimit(string username, string IsoCurrency)
        {
            UserLimit UserLimit = null;
            try
            {
                 var get = await _FunctionApiSetUpImplementation.GetConnectDetails("UserLimit");
     
                var bankServ = await _repoadmBankServiceSetup.GetAsync(c => c.Itbid == get.ConnectionId);

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(bankServ.WebServiceUrl);

                    UserLimit values = new UserLimit()
                    {
                        UserName = !string.IsNullOrEmpty(username) ? username : null,
                        Ccy = IsoCurrency,
                        ConnectionStringId = get.ConnectionId

                    };

                    var url = bankServ.WebServiceUrl.Trim() + "/api/Services/UserLimit";

                    Task<HttpResponseMessage> postTask = client.PostAsJsonAsync(url, values);

                    postTask.Wait();
                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var responseString = result.Content.ReadAsStringAsync();
                        responseString.Wait();
                        UserLimit = JsonConvert.DeserializeObject<UserLimit>(responseString.Result.ToString());
                    }
                }

            }
            catch (Exception ex)
            {
                var exM = ex;
                //  LogManager.SaveLog(ex.Message == null ? ex.InnerException.ToString() : ex.Message.ToString());
            }
            return UserLimit;

        }
        public async Task<List<ValidateChqRangeRsp>> InsertChqRange(ChequeBookReqDTO p, string userName)
        {
            ValidateChqRangeRsp oValidateChqRangeRsp = new ValidateChqRangeRsp();
            var list = new List<ValidateChqRangeRsp>();
            
            try
            {
                  var get = await _FunctionApiSetUpImplementation.GetConnectDetails("InsertChqRange");

                var bankServ = await _repoadmBankServiceSetup.GetAsync(c => c.Itbid == get.ConnectionId);

                foreach (var cbs in p.ListOprChqBookRequest)
                {
                    ChqBookParamDTO values = new ChqBookParamDTO()
                    {
                        accntNo = cbs.AcctNo,
                        acctType = cbs.AcctType,
                        FromChqNo= cbs.StartNo.ToString(),
                        ToChqNo = cbs.EndNo.ToString(),
                        Username = userName,
                        VendorId = cbs.VendorId.ToString(),
                        ChqProductId = cbs.ChqProductCode,
                        CountOrdered = cbs.Quantity.ToString(),
                        ChargeAssessed = "0",
                        rimno = cbs.RsmId.ToString(),
                        ConnectionStringId = get.ConnectionId
                    };

                    var json = JsonConvert.SerializeObject(values);
                    var postdata = new StringContent(json, Encoding.UTF8, "application/json");

                    var client = new HttpClient();
                    var url = bankServ.WebServiceUrl.Trim() + "/api/Services/InsertChqRange";

                    var response = await client.PostAsync(url, postdata);

                    string result = response.Content.ReadAsStringAsync().Result;
                    oValidateChqRangeRsp = JsonConvert.DeserializeObject<ValidateChqRangeRsp>(result);
                
                     list.Add(oValidateChqRangeRsp);
                }
            }
            catch (Exception ex)
            {
                var exM = ex; 
            }
            return list;
        }
    }
}