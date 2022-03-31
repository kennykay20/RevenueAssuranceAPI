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
using RevAssuranceApi.AppSettings;
using RevAssuranceApi.TokenGen;
using RevAssuranceApi.RevenueAssurance.DATA.Models;
using RevAssuranceApi.RevenueAssurance.Repository.DapperDAL;

namespace RevAssuranceApi.Controllers
{
    
    [Route("api/v1/[controller]")]
    [ApiController]
    [EnableCors("CorsPolicy")]
    public class ChequeProductController : ControllerBase
    {
        
         IConfiguration _configuration;
        ApiResponse ApiResponse = new ApiResponse();
        TokenGenerator TokenGenerator;  
        AppSettingsPath AppSettingsPath ;
         IDbConnection db = null;
         ApplicationDbContext   _ApplicationDbContext;
        public ChequeProductController( 
                                        IConfiguration configuration,ApplicationDbContext   ApplicationDbContext)
        {
             
           _configuration = configuration;
           AppSettingsPath = new AppSettingsPath(_configuration);
           TokenGenerator = new TokenGenerator(_configuration);
           db = new SqlConnection(AppSettingsPath.GetDefaultCon());
           _ApplicationDbContext =  ApplicationDbContext;
        }

          [HttpPost("GetAll")]
        public async Task<IActionResult> GetAll(AAuth AnyAuth)
        {
            try
            {
                var rtn = new DapperDATAImplementation<admChequeProduct>();

                string script = "Select * from admChequeProduct";
                
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

         [HttpPost("Add")]
        public async Task<IActionResult> Add(admChequeProduct p)
        {
            try
            {
                p.Unit= "Booklets(s)";
                p.ProductId = int.Parse(p.ChqProductCode);
                p.DateCreated = DateTime.Now;
                p.Status =  _configuration["Statuses:ActiveStatus"];
               _ApplicationDbContext.admChequeProduct.Add(p);
              int _response =  await _ApplicationDbContext.SaveChanges(p.UserId);
                if(_response > 0)
                {
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
                 ApiResponse.ResponseMessage =  ex.InnerException.Message ?? ex.Message;
                 ApiResponse.ResponseCode = -99;
                 return BadRequest(ApiResponse); 
            }
        }

        [HttpPost("Update")]
        public async Task<IActionResult> Update(admChequeProduct p)
        {
            try
            {
                var get =  _ApplicationDbContext.admChequeProduct.FirstOrDefault(c=>c.ItbId  == p.ItbId);
                if(get != null)
                 {
                     if(get.ChqProductCode == p.ChqProductCode && get.ChqProductDescr == p.ChqProductDescr
                     && get.VendorId == p.VendorId && get.Status == p.Status && get.NoOfChqPerUnit == p.NoOfChqPerUnit
                     && get.ProductId == p.ProductId && get.Unit == p.Unit)
                     {
                         ApiResponse.sErrorText=  "No changes was made";
                         ApiResponse.ResponseCode = -99;
                         return Ok(ApiResponse);
                     }
                     get.ChqProductCode = p.ChqProductCode;
                     get.ChqProductDescr = p.ChqProductDescr;
                     get.VendorId = p.VendorId;
                     get.Status = p.Status;
                     get.NoOfChqPerUnit = p.NoOfChqPerUnit;
                     get.ProductId = p.ProductId;
                     get.Unit = p.Unit;
                    

                        _ApplicationDbContext.admChequeProduct.Update(get);
              int _response =  await _ApplicationDbContext.SaveChanges(p.UserId);
                if(_response > 0)
                {
                   ApiResponse.sErrorText=  _configuration["Message:UpdateSuc"];
                   ApiResponse.ResponseCode = 0;
                   return Ok(ApiResponse);
                }
                else
                {
                    return BadRequest(_response); 
                }
        
                  }
                ApiResponse.sErrorText=  "Error occured";
               return BadRequest(ApiResponse);  
            
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