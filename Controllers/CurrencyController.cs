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
using RevAssuranceApi.Response;
using RevAssuranceApi.RevenueAssurance.DATA.Models;
using RevAssuranceApi.AppSettings;
using RevAssuranceApi.TokenGen;
using RevAssuranceApi.RevenueAssurance.Repository.DapperDAL;
using RevAssuranceApi.RevenueAssurance.Repository.Interface;
using RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO;

namespace RevAssuranceApi.Controllers
{

    [Route("api/v1/[controller]")]
    [ApiController]
    [EnableCors("CorsPolicy")]
   public class CurrencyController : ControllerBase
    {

         IConfiguration _configuration;
        ApiResponse ApiResponse = new ApiResponse();
        TokenGenerator TokenGenerator;  
        AppSettingsPath AppSettingsPath ;
         IDbConnection db = null;
         ApplicationDbContext   _ApplicationDbContext;
         IRepository<admCurrency> _repoadmCurrency;
         IRepository<admUserProfile> _repoadmUserProfile;
         IRepository<admGlobalCurrency> _repoadmGlobalCurrency;
        public CurrencyController( 
                                        IConfiguration configuration,
                                        ApplicationDbContext   ApplicationDbContext,
                                         IRepository<admCurrency> repoadmCurrency,
                                         IRepository<admUserProfile> repoadmUserProfile,
                                         IRepository<admGlobalCurrency> repoadmGlobalCurrency) 
        {
           
            _configuration = configuration;
            AppSettingsPath = new AppSettingsPath(_configuration);
            TokenGenerator = new TokenGenerator(_configuration);
            db = new SqlConnection(AppSettingsPath.GetDefaultCon());
            _ApplicationDbContext =  ApplicationDbContext;
            _repoadmUserProfile = repoadmUserProfile;
            _repoadmCurrency = repoadmCurrency;
            _repoadmGlobalCurrency = repoadmGlobalCurrency;
        }
   

        [HttpPost("GetAll")]
        public async Task<IActionResult> GetAll( AAuth AnyAuth)
        {
            try
            {
                var rtn = new DapperDATAImplementation<admCurrency>();

                string script = "Select * from admCurrencies";
                
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
                 ApiResponse.ResponseMessage =  ex.InnerException.Message ?? ex.Message;
                
                 ApiResponse.ResponseCode = -99;
                 return BadRequest(ApiResponse); 
            }

           
        }

        [HttpPost("Update")]
        public async Task<IActionResult> Update(admCurrency p)
        {
            try
            {
                var get =  _ApplicationDbContext.admCurrencies.FirstOrDefault(c=>c.CurrencyNo  == p.CurrencyNo);
                if(get != null)
                 {
                     if(get.CurrencyNo == p.CurrencyNo && get.Description == p.Description
                        && get.IsoCode == p.IsoCode && get.IsLocalCurrency == p.IsLocalCurrency
                        && get.Status == p.Status && get.NumberOfDecimal == p.NumberOfDecimal &&
                     get.Weight == p.Weight)
                     {
                         ApiResponse.sErrorText=  "No changes was made";
                         ApiResponse.ResponseCode = -99;
                        return Ok(ApiResponse);
                     }
                     get.CurrencyNo = p.CurrencyNo;
                     get.Description = p.Description;
                     get.IsoCode = p.IsoCode;
                     get.IsLocalCurrency = p.IsLocalCurrency;
                     get.Status = p.Status;
                     get.NumberOfDecimal = p.NumberOfDecimal;
                     get.Weight = p.Weight;
                  }
               _ApplicationDbContext.admCurrencies.Update(get);
              int _response =  await _ApplicationDbContext.SaveChanges(p.UserId);
                if(_response > 0)
                {
                    if(get.IsLocalCurrency == true){

                        var getAll = await _repoadmCurrency.GetManyAsync(c => c.CurrencyNo != get.CurrencyNo);

                        foreach(var b in getAll) {

                                b.IsLocalCurrency = false;
                                _ApplicationDbContext.admCurrencies.Update(b);
                                int _responseUpdateOther =  await _ApplicationDbContext.SaveChanges(p.UserId);
                        }

                    }
                   ApiResponse.sErrorText=  _configuration["Message:UpdateSuc"];
                   ApiResponse.ResponseCode = 0;
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
        public async Task<IActionResult> Add( admCurrency p)
        {
            try
            {
                var isLocal = await _repoadmCurrency.GetAsync(a=>a.IsLocalCurrency == true);
                if(isLocal != null && p.IsLocalCurrency == true)
                {
                    return BadRequest(new ApiResponse{
                        ResponseMessage =  "System already has a local currrency",
                        ResponseCode = -99
                    }); 
                }

                p.DateCreated = DateTime.Now;
                p.Status =  _configuration["Statuses:ActiveStatus"];
               _ApplicationDbContext.admCurrencies.Add(p);
              int _response =  await _ApplicationDbContext.SaveChanges(p.UserId);
                if(_response > 0)
                {
                    ApiResponse.sErrorText=  _configuration["Message:AddedSuc"];
                    return Ok(ApiResponse);
                }
                else
                {
                     return BadRequest(new ApiResponse{
                        ResponseMessage =  "Could Not Save",
                        ResponseCode = -99
                    }); 
                }
            }
            catch (Exception ex)
            {
                 ApiResponse.ResponseMessage =  ex.InnerException.Message ?? ex.Message;
                 ApiResponse.ResponseCode = -99;
                 return BadRequest(ApiResponse); 
            }
        }
    
         [HttpPost("GetBy")]
        public async Task<IActionResult> GetBy(CurrencyDTO p)
        {
            try
            {
                var getCretedBy = await _repoadmUserProfile.GetAsync(c => c.UserId == p.UserId);

                return Ok(getCretedBy);
            }
            catch(Exception ex){
                var exM = ex == null ? ex.InnerException.Message : ex.Message;

            }
            
            return BadRequest();
        }


        [HttpGet("GetAllGlobal")]
        public async Task<IActionResult> GetAllGlobal()
        {
            try
            {
                var rtn = new DapperDATAImplementation<admGlobalCurrency>();

                string script = "Select * from admGlobalCurrency where Status = 'Active'";
                
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
                 ApiResponse.ResponseMessage =  ex.InnerException.Message ?? ex.Message;
                
                 ApiResponse.ResponseCode = -99;
                 return BadRequest(ApiResponse); 
            }

           
        }


        [HttpPost("GetAllGlobalByValue")]
        public async Task<IActionResult> GetAllGlobalByValue(admGlobalCurrencyDTO val)
        {
            try
            {
                //var rtn = new DapperDATAImplementation<admGlobalCurrency>();

                
                var get =  await _repoadmGlobalCurrency.GetAsync(c=>c.CountryCode2  == val.vals);
                
                var _response = get;
            
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
                 ApiResponse.ResponseMessage =  ex.InnerException.Message ?? ex.Message;
                
                 ApiResponse.ResponseCode = -99;
                 return BadRequest(ApiResponse); 
            }

           
        }

        [HttpGet("GetAllListedCurrency")]

        public async Task<IActionResult> GetAllListedCurrency()
        {
            try
            {
                var rtn = new DapperDATAImplementation<admGlobalCurrency>();

                string script = @"select 
                                CountryCode2, 
                                CcyCode, 
                                CountryName, 
                                CurrencyName, 
                                CcyNo from admGlobalCurrency 
                                where
                                (CountryCode2 not in 
                                (select CountryCode2 from admCurrencies))
                                order by 1";
                
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
                 ApiResponse.ResponseMessage =  ex.InnerException.Message ?? ex.Message;
                
                 ApiResponse.ResponseCode = -99;
                 return BadRequest(ApiResponse); 
            }

           
        }

    
    }

}