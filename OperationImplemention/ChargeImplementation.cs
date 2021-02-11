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

namespace RevAssuranceApi.OperationImplemention
{
    
    public class ChargeImplementation
    {
        IRepository<admCharge> _repoadmCharge;

        public ChargeImplementation( 
                                          IConfiguration configuration,
                                          IRepository<admCharge> repoadmCharge
                                          )
        {
            _repoadmCharge = repoadmCharge;
        }
        public async Task<List<admCharge>> GetChargeDetails(int? SerbiceId)
        {
            var get = await _repoadmCharge.GetManyAsync(c=> c.ServiceId == SerbiceId && c.Ammendment == "N" && c.RePrint == "N");

            return get.ToList();
        }
    }
}