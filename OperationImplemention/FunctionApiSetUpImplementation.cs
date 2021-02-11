
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

    public class FunctionApiSetUpImplementation
    {
        IRepository<admAPIConfig> _repoadmAPIConfig;
        ApplicationDbContext _ApplicationDbContext;
        Formatter _Formatter = new Formatter();

        IRepository<admBankBranch> _repoadmBankBranch;
        IRepository<admBankServiceSetup> _repoadmBankServiceSetup;

        public FunctionApiSetUpImplementation(
                                          IConfiguration configuration,
                                          IRepository<admAPIConfig> repoadmAPIConfig,
                                           ApplicationDbContext ApplicationDbContext,
                                           IRepository<admBankBranch> repoadmBankBranch,
                                           IRepository<admBankServiceSetup> repoadmBankServiceSetup
                                          )
        {
            _repoadmAPIConfig = repoadmAPIConfig;
            _ApplicationDbContext = ApplicationDbContext;
            _repoadmBankBranch = repoadmBankBranch;
            _repoadmBankServiceSetup = repoadmBankServiceSetup;
        }


       public async Task<admAPIConfig> GetConnectDetails(string FunctionName)
       {
         
            try
            {
                var getRec = await _repoadmAPIConfig.GetAsync(c=> c.FunctionName == FunctionName);
        
              return getRec;
           }
                
            catch (Exception ex)
            {
                admAPIConfig admAPIConfig = new admAPIConfig();
                admAPIConfig.ConnectionId = 1; // this it to set as default
                 var exM = ex;
                 return admAPIConfig; 
            }

            
        }

    }
}
