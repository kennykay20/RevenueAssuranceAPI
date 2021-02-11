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
using RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO;
using RevAssuranceApi.OperationImplemention;
using RevAssuranceApi.RevenueAssurance.Repository.Interface;

namespace RevAssuranceApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [EnableCors("CorsPolicy")]
    //[Authorize]
    public class BankServiceSetUpController : ControllerBase
    {
         IConfiguration _configuration;
        ApiResponse ApiResponse = new ApiResponse();
        TokenGenerator TokenGenerator;  
        AppSettingsPath AppSettingsPath ;
         IDbConnection db = null;
         ApplicationDbContext   _ApplicationDbContext;
         Cryptors Cryptors = new Cryptors();
         RoleAssignImplementation _RoleAssignImplementation;
         IRepository<admBankServiceSetup> _repoadmBankServiceSetup;
        public BankServiceSetUpController(IConfiguration configuration,
                                          ApplicationDbContext   ApplicationDbContext,
                                          RoleAssignImplementation RoleAssignImplementation,
                                          IRepository<admBankServiceSetup> repoadmBankServiceSetup
) 
        {
       
           _configuration = configuration;
           AppSettingsPath = new AppSettingsPath(_configuration);
           TokenGenerator = new TokenGenerator(_configuration);
           db = new SqlConnection(AppSettingsPath.GetDefaultCon());
           _ApplicationDbContext =  ApplicationDbContext;
           _RoleAssignImplementation = RoleAssignImplementation;
           _repoadmBankServiceSetup = repoadmBankServiceSetup;
        }
   

        [HttpPost("GetAll")]
        public async Task<IActionResult> GetAll(ParamLoadPage AnyAuth)
        {
            try
            {
                 var RoleAssign = await _RoleAssignImplementation.GetRoleAssign(AnyAuth.MenuId,AnyAuth.RoleId);
                
                var rtn = new DapperDATAImplementation<admBankServiceSetup>();

                string script = "Select * from admBankServiceSetup";
                
                var _response = await rtn.LoadListNoParam(script, db);
            
                if(_response != null)
                {
                    var res = new {
                        roleAssign = RoleAssign,
                        _response = _response
                    };
                   return Ok(res);
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
        public async Task<IActionResult> Add(admBankServiceSetupDTO p)
        {
            try
            {
                var adm = new admBankServiceSetup();
                
                string Pwd =  Cryptors.EncryptRevAss(p.admBankServiceSetup.Password);

                adm.WebServiceUrl = p.admBankServiceSetup.WebServiceUrl;
                adm.ConnectionName = p.admBankServiceSetup.ConnectionName;
                adm.DataBaseName = p.admBankServiceSetup.DataBaseName;
                adm.DatabasePort = p.admBankServiceSetup.DatabasePort;
                adm.Server = p.admBankServiceSetup.Server;
                adm.UserName = p.admBankServiceSetup.UserName;
                adm.Password = Pwd;
                adm.DateCreated = DateTime.Now;
                adm.UserId = p.LoginUserId ;
                adm.Status =  _configuration["Statuses:ActiveStatus"];
                await _repoadmBankServiceSetup.AddAsync(adm);
              int _response =  await _ApplicationDbContext.SaveChanges(p.LoginUserId);
                if(_response > 0)
                {

                  
                    ApiResponse.ResponseMessage =  _configuration["Message:AddedSuc"];
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
    
    
        [HttpPost("Update")]
        public async Task<IActionResult> Update(admBankServiceSetupDTO p)
        {
            try
            {
                var get = await  _repoadmBankServiceSetup.GetAsync(c=> c.Itbid == p.admBankServiceSetup.Itbid);
                if(get != null)
                {
                    get.WebServiceUrl = p.admBankServiceSetup.WebServiceUrl;
                    get.ConnectionName = p.admBankServiceSetup.ConnectionName;
                    get.DataBaseName = p.admBankServiceSetup.DataBaseName;
                    get.DatabasePort = p.admBankServiceSetup.DatabasePort;
                    get.Server = p.admBankServiceSetup.Server;
                    get.UserName = p.admBankServiceSetup.UserName;
                    if(get.Password ==  p.admBankServiceSetup.Password){
                        
                        get.Password = p.admBankServiceSetup.Password;
                    }
                    else
                    {
                        if(!string.IsNullOrWhiteSpace(p.admBankServiceSetup.Password))
                        {
                            string Encript = Cryptors.EncryptRevAss(p.admBankServiceSetup.Password);
                            get.Password = Encript;
                        }
                        else
                        {
                            get.Password = null;
                        }
                        
                    }
                   
                    get.Status = p.admBankServiceSetup.Status;
        
                }
                 _repoadmBankServiceSetup.Update(get);
                int _response =  await _ApplicationDbContext.SaveChanges(p.LoginUserId);
                if(_response > 0)
                {
                   ApiResponse.ResponseMessage=  _configuration["Message:UpdateSuc"];
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