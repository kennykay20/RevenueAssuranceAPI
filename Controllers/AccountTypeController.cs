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
using RevAssuranceApi.AppSettings;

using RevAssuranceApi.TokenGen;
using Dapper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RevAssuranceApi.Response;
using RevAssuranceApi.RevenueAssurance.DATA.Models;
using RevAssuranceApi.RevenueAssurance.Repository.DapperDAL;
using RevAssuranceWebAPi.AnythingGood.DATA.Models;
using RevAssuranceApi.RevenueAssurance.Repository.Interface;

namespace RevAssuranceApi.Controllers
{

    [Route("api/v1/[controller]")]
    [ApiController]
    [EnableCors("CorsPolicy")]
    public class AccountTypeController : ControllerBase
    {
       
         IConfiguration _configuration;
        ApiResponse ApiResponse = new ApiResponse();
        TokenGenerator TokenGenerator;  
        AppSettingsPath AppSettingsPath ;
         IDbConnection db = null;
         IRepository<admAccountType> _repoAccountType;
         ApplicationDbContext   _ApplicationDbContext;
        public AccountTypeController(
                IConfiguration configuration,
                ApplicationDbContext   ApplicationDbContext,
                IRepository<admAccountType> repoAccountType) 
        {
          
           _configuration = configuration;
           AppSettingsPath = new AppSettingsPath(_configuration);
           TokenGenerator = new TokenGenerator(_configuration);
           db = new SqlConnection(AppSettingsPath.GetDefaultCon());
           _ApplicationDbContext =  ApplicationDbContext;
           _repoAccountType = repoAccountType;
        }
   
    

        [HttpPost("GetAll")]
        public async Task<IActionResult> GetAll([FromBody] AAuth AnyAuth)
        {
            try
            {
                var rtn = new DapperDATAImplementation<admAccountType>();

                string script = "Select * from admAccountType";
                
                var _response = await rtn.LoadListNoParam(script, db);
            
                if(_response != null)
                {
                   return Ok(_response);
                }
                else
                {
                    return BadRequest(_response); 
                }
                  
            
            }
            catch (Exception ex)
            {
                 ApiResponse.ResponseMessage =  ex == null ? ex.InnerException.Message : ex.Message;
                
                 ApiResponse.ResponseCode = -99;
                 return BadRequest(ApiResponse); 
            }

           
        }

        [HttpGet("GetAccountFormat/{accountType}")]
        public async Task<IActionResult> GetAccountFormat(string accountType)
        {
            if(accountType == null){
                return Ok(new {
                    ResponseCode = -99,
                    ResponseMessage = "Account Type cannot be null"
                });
            }
            
            try
            {
                
                var rtn = new DapperDATAImplementation<admAccountType>();

                //string script = "Select AccountFormat from admAccountType where AccountTypeCode = " + $"{accountType}";
                
                //var _response = await rtn.LoadParam(script, db);

                var _response = await _repoAccountType.GetAsync(x => x.AccountTypeCode == accountType);
                
                ApiResponse.AccountFormat = _response.AccountFormat;
                ApiResponse.AccountType = _response.AccountTypeCode;
                ApiResponse.AccountDescription = _response.Description;
            
                if(_response != null)
                {
                   return Ok(ApiResponse);
                }
                else
                {
                    return Ok(_response); 
                }
                  
            }
            catch (Exception ex)
            {
                 ApiResponse.ResponseMessage =  ex == null ? ex.InnerException.Message : ex.Message;
                
                 ApiResponse.ResponseCode = -99;
                 return BadRequest(ApiResponse); 
            }
            
        }
        [HttpPost("Update")]
        public async Task<IActionResult> Update([FromBody] admAccountType p)
        {
            try
            {

                admAccountType update = new  admAccountType();
                update.ItbId = p.ItbId;
                update.AccountTypeCode = p.AccountTypeCode;
                update.Description = p.Description;
                update.AccountFormat = p.AccountFormat;
                update.Status = p.Status;
                update.DateCreated = p.DateCreated;
        

               _ApplicationDbContext.admAccountType.Update(update);
                int _response =  await _ApplicationDbContext.SaveChanges((int)p.UserId);
                if(_response > 0)
                {
                   ApiResponse.ResponseCode = 0;
                   ApiResponse.ResponseMessage =  _configuration["Message:UpdateSuc"];
                    ApiResponse.sErrorText=  _configuration["Message:UpdateSuc"];
                   return Ok(ApiResponse);
                   
                }
                else
                {
                    return BadRequest(_response); 
                }
                  
            
            }
            catch (Exception ex)
            {
                 ApiResponse.ResponseMessage =  ex == null ? ex.InnerException.Message : ex.Message;
                
                 ApiResponse.ResponseCode = -99;
                 return BadRequest(ApiResponse); 
            }

           
        }
    
        [HttpPost("Add")]
        public async Task<IActionResult> Add([FromBody] admAccountType p)
        {
            try
            {
                p.DateCreated = DateTime.Now;
                p.Status =  _configuration["Statuses:ActiveStatus"];
               _ApplicationDbContext.admAccountType.Add(p);
              int _response =  await _ApplicationDbContext.SaveChanges((int)p.UserId);
                if(_response > 0)
                {
                    ApiResponse.ResponseCode = 0;
                    ApiResponse.ResponseMessage = _configuration["Message:AddedSuc"];
                    ApiResponse.sErrorText=  _configuration["Message:AddedSuc"];
                    
                    return Ok(ApiResponse);
                }
                else
                {
                    return BadRequest(_response); 
                }
                  
            
            }
            catch (Exception ex)
            {
                 ApiResponse.ResponseMessage =  ex == null ? ex.InnerException.Message : ex.Message;
                
                 ApiResponse.ResponseCode = -99;
                 return BadRequest(ApiResponse); 
            }

           
        }
    
    
    
    }



}