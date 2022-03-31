
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
 
    public class AcctStatementImplementation
    {
         IRepository<admCharge> _repoadmCharge;
         ApplicationDbContext _ApplicationDbContext;
         Formatter _Formatter = new Formatter();
         IRepository<admBankBranch> _repoadmBankBranch;
         IRepository<admBankServiceSetup> _repoadmBankServiceSetup;
        FunctionApiSetUpImplementation _FunctionApiSetUpImplementation;
        public AcctStatementImplementation( 
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

        public async Task<List<MiniStatementResp>> MiniStatement(AcctStateCoreBankParam AcctStateCoreBankParam)
        {
            var values = new List<MiniStatementResp>();
            try
            {
               
                var get = await _FunctionApiSetUpImplementation.GetConnectDetails("ValidateAccount");
                
                var _responseBankServ = await _repoadmBankServiceSetup.GetAsync(c => c.Itbid == get.ConnectionId);

                var url = _responseBankServ.WebServiceUrl.Trim() + "/api/Services/MiniStatement";
                
                AcctStateCoreBankParam.ConnectionStringId = get.ConnectionId;

                var json = JsonConvert.SerializeObject(AcctStateCoreBankParam);

                var data = new StringContent(json, Encoding.UTF8, "application/json");

                var client = new HttpClient();

                var response = await client.PostAsync(url, data);

                string result = response.Content.ReadAsStringAsync().Result;
                values = JsonConvert.DeserializeObject<List<MiniStatementResp>>(result);
                if (values.Count() > 0 )
                {
                     return values;
                }
            }
            catch (Exception ex)
            {
                var exM = ex;
                return null;
            }
             return null;
        }
  }
}