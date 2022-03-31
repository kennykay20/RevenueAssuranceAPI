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
using RevAssuranceWebAPi.AnythingGood.DATA.Models;
using RevAssuranceApi.RevenueAssurance.Repository.Interface;

namespace RevAssuranceApi.OperationImplemention
{
    
    public class ColCollateralTypeImplementation
    {
        IRepository<admCollateralType> _repoadmCollateralType;

        public ColCollateralTypeImplementation( 
                                          IConfiguration configuration,
                                          IRepository<admCollateralType> repoadmCollateralType
                                          )
        {
            _repoadmCollateralType = repoadmCollateralType;
        }
        public async Task<List<admCollateralType>> GetAll()
        {

            var get = await _repoadmCollateralType.GetManyAsync(c=> c.CollTypeId > 0 && c.Status == "Active");

            return get.ToList();
        }

    
    }
}