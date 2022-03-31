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
using System.IO;
using RevAssuranceApi.Helper;
using RevAssuranceApi.Response;
using RevAssuranceApi.TokenGen;
using RevAssuranceApi.RevenueAssurance.DATA.Models;
using RevAssuranceApi.AppSettings;
using RevAssuranceApi.OperationImplemention;
using RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO;

namespace RevAssuranceApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [EnableCors("CorsPolicy")]
    public class LogController : ControllerBase
    {
         
         IConfiguration _configuration;
        ApiResponse ApiResponse = new ApiResponse();
        TokenGenerator TokenGenerator;  
        AppSettingsPath AppSettingsPath ;
         IDbConnection db = null;
         ApplicationDbContext   _ApplicationDbContext;
        RoleAssignImplementation _RoleAssignImplementation;
         public LogController(IConfiguration configuration,
                              ApplicationDbContext   ApplicationDbContext,
                               RoleAssignImplementation RoleAssignImplementation)
         {
           
           _configuration = configuration;
           AppSettingsPath = new AppSettingsPath(_configuration);
           TokenGenerator = new TokenGenerator(_configuration);
           db = new SqlConnection(AppSettingsPath.GetDefaultCon());
           _ApplicationDbContext =  ApplicationDbContext;
           _RoleAssignImplementation = RoleAssignImplementation;
         }   

        [HttpPost("GetAll")]
        public async Task<IActionResult> GetAll(ParamLoadPage AnyAuth)
        {
            try
            {
                var roleAssign = await _RoleAssignImplementation.GetRoleAssign(AnyAuth.MenuId, AnyAuth.RoleId);
                
                var path = _configuration["LgPth:logfileDwnPth"];

                if(Directory.Exists(path))
                {
                    var log = new List<LogTemplate>();
                   // var datepath = $"{path} \\ {AnyAuth.Date}";

                    var files  = Directory.GetDirectories(path);
                    foreach(var item in files)
                    {
                        FileInfo fi = new FileInfo(item);

                        var getlast = item.Split('\\').LastOrDefault();
                       
                        
                        log.Add(new LogTemplate {
                            name = getlast,
                            path = item,
                            date = fi.CreationTime

                        });
                    }

                    var res = new 
                    {
                        roleAssign = roleAssign,
                        log = log
                    };

                   return Ok(res);
                }

                 return BadRequest(new ApiResponse{
                     ResponseCode = -99,
                     ResponseMessage = "Log directory does not exist"
                 });

            }

            catch (Exception ex)
            {
                 ApiResponse.ResponseMessage =  ex.InnerException.Message ?? ex.Message;
                
                 ApiResponse.ResponseCode = -99;
                 return BadRequest(ApiResponse); 
            }
        }
       
         [HttpPost("Download")]
        public async Task<IActionResult> Download(LogPathDTO LogPathDTO)
        {
            try
            {
                var files  = Directory.GetFiles(LogPathDTO.LogPath);
                string[] lines = System.IO.File.ReadAllLines(files[0]);
      
               if(lines != null)
               {
                    // List<string> termsList = new List<string>();
                   StringBuilder termsList = new StringBuilder();
                    foreach (string line in lines)
                    {
                        // termsList.Append("\n");
                       termsList.AppendLine(line);
                        // termsList.Add(line);
                    }
                     return Ok(termsList);
               }
                 return BadRequest(new ApiResponse{
                     ResponseCode = -99,
                     ResponseMessage = "Log directory does not exist"
                 });

            }

            catch (Exception ex)
            {
                 ApiResponse.ResponseMessage =  ex.InnerException.Message ?? ex.Message;
                
                 ApiResponse.ResponseCode = -99;
                 return BadRequest(ApiResponse); 
            }
        }
       


    }
}