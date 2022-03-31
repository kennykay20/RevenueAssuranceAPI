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
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RevAssuranceApi.AppSettings;
using RevAssuranceApi.Response;
using RevAssuranceApi.RevenueAssurance.DATA.Models;
using RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO;
using RevAssuranceApi.RevenueAssurance.Repository.DapperDAL;
using RevAssuranceApi.TokenGen;
using RevAssuranceWebAPi.AnythingGood.DATA.Models;

namespace RevAssuranceApi.Controllers
{   
    [Route("api/v1/[controller]")]
    [ApiController]
    [EnableCors("CorsPolicy")]
    public class TemplateController : ControllerBase
    {
         
         IConfiguration _configuration;
        ApiResponse ApiResponse = new ApiResponse();
        TokenGenerator TokenGenerator;  
         AppSettingsPath AppSettingsPath ;
         IDbConnection db = null;
         ApplicationDbContext   _ApplicationDbContext;

         public TemplateController(IConfiguration configuration,ApplicationDbContext   ApplicationDbContext)
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
                var rtn = new DapperDATAImplementation<admTemplateDTO>();

                string script = @"Select 
			*,
			(Select ServiceDescription from admService where ServiceId = a.ServiceId) ServiceDescription 
			
			from admTemplates a	";
                
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
        public async Task<IActionResult> Add([FromBody] admTemplate p)
        {
            try
            {
                p.DateCreated = DateTime.Now;
                p.Status =  _configuration["Statuses:ActiveStatus"];
               _ApplicationDbContext.admTemplates.Add(p);
              int _response =  await _ApplicationDbContext.SaveChanges(1);
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
        public async Task<IActionResult> Update(admTemplate p)
        {
            try
            {
                var get =  _ApplicationDbContext.admTemplates.FirstOrDefault(c=>c.ItbId  == p.ItbId);
                if(get != null)
                 {
                     if(get.TemplateName == p.TemplateName &&
                       get.TemplateCode == p.TemplateCode &&
                       get.TemplateContent == p.TemplateContent &&
                       get.RecordPerPage == p.RecordPerPage &&
                       get.ImageUrl == (p.ImageUrl != "" ? p.ImageUrl : null))
                       {
                            ApiResponse.sErrorText = "No changes was made";
                            ApiResponse.ResponseCode = -99;
                            return Ok(ApiResponse);
                       }

                       get.TemplateName = p.TemplateName;
                       get.TemplateCode = p.TemplateCode;
                       get.TemplateContent = p.TemplateContent;
                       get.RecordPerPage = p.RecordPerPage;
                       get.ImageUrl = p.ImageUrl;
                     

                        _ApplicationDbContext.admTemplates.Update(get);
                        int _response =  await _ApplicationDbContext.SaveChanges(p.UserId);
                        if(_response > 0)
                        {
                            ApiResponse.sErrorText=  _configuration["Message:UpdateSuc"];
                             ApiResponse.ResponseCode = 0;
                            return Ok(ApiResponse);
                        }
                 }
              
                ApiResponse.sErrorText = "Error occured";
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