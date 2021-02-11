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
using RevAssuranceApi.RevenueAssurance.Repository.Interface;
using RevAssuranceApi.TokenGen;
using RevAssuranceWebAPi.AnythingGood.DATA.Models;


namespace RevAssuranceApi.Controllers
{   
    [Route("api/v1/[controller]")]
    [ApiController]
    [EnableCors("CorsPolicy")]
    public class InstrumentTemplateController : ControllerBase
    {
         
         IConfiguration _configuration;
        ApiResponse ApiResponse = new ApiResponse();
        TokenGenerator TokenGenerator;  
         AppSettingsPath AppSettingsPath ;
         IDbConnection db = null;
         ApplicationDbContext   _ApplicationDbContext;
          IRepository<oprInstrmentTemp> _repooprInstrmentTem;

         public InstrumentTemplateController(IConfiguration configuration,
                                            ApplicationDbContext   ApplicationDbContext,
                                            IRepository<oprInstrmentTemp> repooprInstrmentTem)
         {
             
           _configuration = configuration;
           AppSettingsPath = new AppSettingsPath(_configuration);
           TokenGenerator = new TokenGenerator(_configuration);
           db = new SqlConnection(AppSettingsPath.GetDefaultCon());
           _ApplicationDbContext =  ApplicationDbContext;
           _repooprInstrmentTem = repooprInstrmentTem;
         }

         
     
        [HttpPost("GetTemCont")]
        public async Task<IActionResult> GetTemCont(ParamInstrumentTmp p)
        {
            try
            {
               
                var  Content = await _repooprInstrmentTem.GetAsync(c=> c.ItbId == p.TemplateItbId);
                if(Content != null)
                {
                  
                   return Ok(Content);
                }
                else
                {
                     ApiResponse.ResponseMessage =  "No Template Content!";
                    return BadRequest(ApiResponse); 
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