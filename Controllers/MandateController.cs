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
using RevAssuranceApi.AppSettings;

using RevAssuranceApi.TokenGen;
using Dapper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RevAssuranceApi.Helper;
using RevAssuranceApi.Response;
using RevAssuranceApi.RevenueAssurance.DATA.Models;
using RevAssuranceApi.RevenueAssurance.Repository.DapperDAL;
using RevAssuranceWebAPi.AnythingGood.DATA.Models;
using RevAssuranceApi.RevenueAssurance.Repository.Interface;
using RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO;
using RevAssuranceApi.OperationImplemention;

namespace RevAssuranceApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [EnableCors("CorsPolicy")]
    //[Authorize]
    public class MandateController : ControllerBase
    {
         IConfiguration _configuration;
        ApiResponse ApiResponse = new ApiResponse();
        TokenGenerator TokenGenerator;  
        AppSettingsPath AppSettingsPath ;
         IDbConnection db = null;
         ApplicationDbContext   _ApplicationDbContext;
         Cryptors Cryptors = new Cryptors();
         IRepository<admRole> _repoRoles;
         IRepository<admUserProfile> _repoadmUserProfile;
         UsersImplementation _UsersImplementation;
         RoleAssignImplementation _RoleAssignImplementation;
         IRepository<admUserLimit> _repoadmUserLimit;
         IRepository<admCurrency> _repoadmCurrency; 
         MandateImplementation _MandateImplementation;
        public MandateController(IConfiguration configuration,
                                ApplicationDbContext   ApplicationDbContext,
                                IRepository<admRole> repoRoles,
                                IRepository<admUserProfile> repoadmUserProfile,
                                 UsersImplementation UsersImplementation,
                                 RoleAssignImplementation RoleAssignImplementation,
                                 IRepository<admUserLimit> repoadmUserLimit,
                                 IRepository<admCurrency> repoadmCurrency,
                                 MandateImplementation MandateImplementation) 
        {
       
           _configuration = configuration;
           AppSettingsPath = new AppSettingsPath(_configuration);
           TokenGenerator = new TokenGenerator(_configuration);
           db = new SqlConnection(AppSettingsPath.GetDefaultCon());
           _ApplicationDbContext =  ApplicationDbContext;
           _repoRoles  = repoRoles;
           _repoadmUserProfile = repoadmUserProfile;
           _UsersImplementation = UsersImplementation;
           _RoleAssignImplementation = RoleAssignImplementation;
           _repoadmUserLimit = repoadmUserLimit;
           _repoadmCurrency = repoadmCurrency;
           _MandateImplementation = MandateImplementation;
        }

        [HttpPost("GetMandate")]
        public async Task<IActionResult> GetMandate(MandateDTO values)
        {
            try
            {
                var ApiResponse = new ApiResponse();
                var getExist = await _MandateImplementation.ViewMandate(values);
                if(getExist != null)
                {
                  ApiResponse.ResponseCode = -99;
                  ApiResponse.ResponseMessage = "Users Already Exist!";
                  return Ok(getExist);
                }  

                
                  ApiResponse.ResponseMessage = "No Mandate Found!";
                  return BadRequest(getExist);       
            }
            catch (Exception ex)
            {
                 ApiResponse.ResponseMessage =  ex == null ? ex.InnerException.Message : ex.Message;
                
                 ApiResponse.ResponseCode = -99;
                 return BadRequest(ApiResponse); 
            }

             return BadRequest(ApiResponse); 
        }
    }
}