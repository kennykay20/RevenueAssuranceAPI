using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using RevAssuranceApi.RevenueAssurance.Repository.Interface;
using RevAssuranceWebAPi.AnythingGood.DATA.Models;
using RevAssuranceApi.Response;
using RevAssuranceApi.RevenueAssurance.DATA.Models;
using System;
using RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO;
using Dapper;
using RevAssuranceApi.RevenueAssurance.Repository.DapperDAL;
using Microsoft.Extensions.Configuration;
using System.Data;
using RevAssuranceApi.AppSettings;
using System.Data.SqlClient;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;

namespace RevAssuranceApi.OperationImplemention
{
    public class MandateImplementation
    {
        IRepository<CbsTransaction> _repoCbsTransaction;
        ApplicationDbContext _ApplicationDbContext;
        IRepository<OprInstrument> _repoOprInstrument;
        IRepository<admClientProfile> _repoadmClientProfile;
        IRepository<admBankServiceSetup> _repoadmBankServiceSetup;

        IConfiguration _configuration;
        IDbConnection db = null;
        AppSettingsPath AppSettingsPath;
        Formatter _Formatter = new Formatter();
        FunctionApiSetUpImplementation _FunctionApiSetUpImplementation;

        public MandateImplementation(
                IRepository<CbsTransaction> repoCbsTransaction,
                ApplicationDbContext ApplicationDbContext,
                IRepository<OprInstrument> repoOprInstrument,
                IConfiguration configuration,
                IRepository<admClientProfile> repoadmClientProfile,
                IRepository<admBankServiceSetup> repoadmBankServiceSetup,
                FunctionApiSetUpImplementation FunctionApiSetUpImplementation)
        {
            _configuration = configuration;
            _repoCbsTransaction = repoCbsTransaction;
            _ApplicationDbContext = ApplicationDbContext;
            _repoOprInstrument = repoOprInstrument;
            AppSettingsPath = new AppSettingsPath(_configuration);
            db = new SqlConnection(AppSettingsPath.GetDefaultCon());
            _repoadmClientProfile = repoadmClientProfile;
            _repoadmBankServiceSetup = repoadmBankServiceSetup;
            _FunctionApiSetUpImplementation = FunctionApiSetUpImplementation;
        }

        public async Task<List<MandateResponse>> ViewMandate(MandateDTO values)
        {
            var MandateResponse = new List<MandateResponse>();
            try
            {
                var clientProfile = await _repoadmClientProfile.GetAsync(null);

                var get = await _FunctionApiSetUpImplementation.GetConnectDetails("GetMandate");

                var bankServ = await _repoadmBankServiceSetup.GetAsync(c => c.Itbid == 1);
                values.ConnectionStringId = get.ConnectionId;
                var json = JsonConvert.SerializeObject(values);
                var postdata = new StringContent(json, Encoding.UTF8, "application/json");


                var client = new HttpClient();
                var url = bankServ.WebServiceUrl.Trim() + "/api/Services/GetMandate";


                var response = await client.PostAsync(url, postdata);

                string result = response.Content.ReadAsStringAsync().Result;
                MandateResponse = JsonConvert.DeserializeObject<List<MandateResponse>>(result);

            }
            catch (Exception ex)
            {
                var exM = ex;
                // LogManager.SaveLog(ex.Message == null ? ex.InnerException.ToString() : ex.Message.ToString());

            }

            return MandateResponse;

        }


    }
}