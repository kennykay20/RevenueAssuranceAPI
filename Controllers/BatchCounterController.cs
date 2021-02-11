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

using Dapper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RevAssuranceWebAPi.AnythingGood.DATA.Models;
using Microsoft.EntityFrameworkCore;
using RevAssuranceApi.RevenueAssurance.DATA.Models;
using RevAssuranceApi.Response;
using RevAssuranceApi.TokenGen;
using RevAssuranceApi.AppSettings;
using RevAssuranceApi.RevenueAssurance.Repository.DapperDAL;

namespace RevAssuranceApi.Controllers
{
     [Route("api/v1/[controller]")]
    [ApiController]
    [EnableCors("CorsPolicy")]
    public class BatchCounterController : ControllerBase
    {
       
         IConfiguration _configuration;
        ApiResponse ApiResponse = new ApiResponse();
        TokenGenerator TokenGenerator;  
        AppSettingsPath AppSettingsPath ;
         IDbConnection db = null;
         ApplicationDbContext   _ApplicationDbContext;
        public BatchCounterController(IConfiguration configuration,ApplicationDbContext   ApplicationDbContext)
        {
           
           _configuration = configuration;
           AppSettingsPath = new AppSettingsPath(_configuration);
           TokenGenerator = new TokenGenerator(_configuration);
           db = new SqlConnection(AppSettingsPath.GetDefaultCon());
           _ApplicationDbContext =  ApplicationDbContext;
        }

        // [HttpPost("GetAll")]
        // public async Task<IActionResult> GetAll(AAuth AnyAuth)
        // {
        //     try
        //     {
        //         var rtn = new DapperDATAImplementation<BatchCounter>();

        //         string script = "Select * from BatchCounter";
                
        //         var _response = await rtn.LoadListNoParam(script, db);
            
        //         if(_response != null)
        //         {
        //            return Ok(_response);
        //         }
        //         else
        //         {
        //             return BadRequest(_response); 
        //         }
                  
            
        //     }
        //     catch (Exception ex)
        //     {
        //          ApiResponse.ResponseMessage =  ex.InnerException.Message ?? ex.Message;
                
        //          ApiResponse.ResponseCode = -99;
        //          return BadRequest(ApiResponse); 
        //     }
        // }
    
    
    
    }
}