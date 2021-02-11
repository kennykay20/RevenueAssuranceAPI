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
using RevAssuranceApi.Helper;
using RevAssuranceApi.RevenueAssurance.DATA.Models;
using RevAssuranceApi.TokenGen;
using RevAssuranceApi.Response;
using RevAssuranceApi.AppSettings;

namespace RevAssuranceApi.Controllers
{
    
    [Route("api/v1/[controller]")]
    [ApiController]
    [EnableCors("CorsPolicy")]
    public class QueryController : ControllerBase
    {
       
         IConfiguration _configuration;
        ApiResponse ApiResponse = new ApiResponse();
        TokenGenerator TokenGenerator;  
        AppSettingsPath AppSettingsPath ;
         IDbConnection db = null;
         ApplicationDbContext   _ApplicationDbContext;
        public QueryController(IConfiguration configuration,ApplicationDbContext   ApplicationDbContext)
        {
                    
           _configuration = configuration;
           AppSettingsPath = new AppSettingsPath(_configuration);
           TokenGenerator = new TokenGenerator(_configuration);
           db = new SqlConnection(AppSettingsPath.GetDefaultCon());
           _ApplicationDbContext =  ApplicationDbContext;
        }

        [HttpPost("Select")]
        public async Task<IActionResult> SelectAsync(QueryDto q)
        {
            if (q.query != null)
            {
                try
                {
                    var res = (await db.QueryAsync(sql: q.query,
                    commandType: CommandType.Text
                     )).ToList();
                    return Ok(res);
                }
                catch (Exception ex)
                {
                    ApiResponse.ResponseMessage = ex.InnerException.Message ?? ex.Message;
                    ApiResponse.ResponseCode = -99;
                    return BadRequest(ApiResponse);
                }
            }
            return BadRequest("Invalid Request");
        }

        [HttpPost("Execute")]
        public async Task<IActionResult> ExecuteScript(QueryDto q)
        {
            if (q.query != null)
            {
                try
                {
                var  ApiResponse = new ApiResponse(); 
                using (var con = new SqlConnection(AppSettingsPath.GetDefaultCon()))
                {

                    con.Open();

                    string command = q.query;
                    SqlCommand com2 = new SqlCommand(command, con);
                    int rtv = com2.ExecuteNonQuery();
                    con.Close();

                    if (rtv > 0)
                    {

                        ApiResponse.ResponseCode  = 0;
                        ApiResponse.ResponseMessage = "Script Executed Successfully";
                        return Ok(ApiResponse);

                    }
                    else
                    {

                      
                        ApiResponse.ResponseMessage = "Script Failed";
                       return BadRequest(ApiResponse);

                    }


                }
                }
                catch (Exception ex)
                {
                     ApiResponse.ResponseMessage = ex.InnerException.Message ?? ex.Message;
                    ApiResponse.ResponseCode = -99;
                    return BadRequest(ApiResponse);
                }
            }
                ApiResponse.ResponseMessage = "Enter Scripts to be Executed";
                       return BadRequest(ApiResponse);
          
        }
    }
}