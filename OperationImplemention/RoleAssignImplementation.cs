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
using RevAssuranceApi.RevenueAssurance.Repository.Interface;

namespace RevAssuranceApi.OperationImplemention
{
    
    public class RoleAssignImplementation
    {
        IRepository<admRoleAssignment> _repoadmRoleAssignment;

        public RoleAssignImplementation( 
                                          IConfiguration configuration,
                                          IRepository<admRoleAssignment> repoadmRoleAssignment
                                          )
        {
            _repoadmRoleAssignment = repoadmRoleAssignment;
        }
        public async Task<admRoleAssignment> GetRoleAssign(int? MenuId, int? RoleId)
        {
            var get = await _repoadmRoleAssignment.GetAsync(c=> c.RoleId == RoleId && c.MenuId == MenuId);

            return get;
        }

    
    }
}