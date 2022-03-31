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
using RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO;
using RevAssuranceApi.RevenueAssurance.Repository.Interface;
using RevAssuranceApi.OperationImplemention;

namespace RevAssuranceApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [EnableCors("CorsPolicy")]
    //[Authorize]
    public class DashboardController : ControllerBase
    {
         IConfiguration _configuration;
        ApiResponse ApiResponse = new ApiResponse();
        TokenGenerator TokenGenerator;  
        AppSettingsPath AppSettingsPath ;
         IDbConnection db = null;
         ApplicationDbContext   _ApplicationDbContext;
         IRepository<admRoleAssignment> _repoadmRoleAssignment;
         IRepository<admDashBoardAssignment> _repoadmDashBoardAssignment;
          Formatter _Formatter = new Formatter();
        public DashboardController(
                                        IConfiguration configuration,ApplicationDbContext   ApplicationDbContext,
                                        IRepository<admRoleAssignment> repoadmRoleAssignment, IRepository<admDashBoardAssignment> repoadmDashBoardAssignment) 
        {
           _configuration = configuration;
           AppSettingsPath = new AppSettingsPath(_configuration);
           TokenGenerator = new TokenGenerator(_configuration);
           db = new SqlConnection(AppSettingsPath.GetDefaultCon());
           _ApplicationDbContext =  ApplicationDbContext;
           _repoadmRoleAssignment = repoadmRoleAssignment;
           _repoadmDashBoardAssignment = repoadmDashBoardAssignment;
        }
  
  
        [HttpPost("GetAllDashboard")]
        public async Task<IActionResult> GetAllDashboard(RoleAssignDTO AnyAuth)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();

                param.Add("@nRoleId", AnyAuth.RoleId);

                var rtn = new DapperDATAImplementation<RoleAssignDTO>();
                
                var _response = await rtn.LoadData("Isp_AssignedDashboardMenu", param, db);
                var _response2 =  _response.GroupBy(c=> c.MenuId).Select(x => x.FirstOrDefault());

               var tt =  _response2.GroupBy(x => x.MenuId)
                .Where(g => g.Count() == 1)
                .Select(g => g.First());
              
                if(_response2 != null)
                {
                   var res = new {
                       _response = tt,
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
   

        [HttpPost("GetDashboard")]
        public async Task<IActionResult> GetDashboard(RoleAssignDTO AnyAuth)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();

                param.Add("@nRoleId", AnyAuth.RoleId);

                var rtn = new DapperDATAImplementation<DashboardCountDTO>();
                
                var _response = await rtn.LoadData("Isp_GetDashboard", param, db);
                var _response2 =  _response.GroupBy(c=> c.MenuId).Select(x => x.FirstOrDefault());

               var tt =  _response2.GroupBy(x => x.MenuId)
                .Where(g => g.Count() == 1)
                .Select(g => g.First());

                var resultCount =  new List<DashboardCountDTO>();

                foreach(var b in tt) 
                {
                    
                     var count = new DapperDATAImplementation<DashboardCountDTO>();
                     var countResult = new DashboardCountDTO();
                     countResult = b;

                     if(b.MenuId == 65) 
                     {
                        DynamicParameters param2 = new DynamicParameters();
                        param2.Add("@pdtCurrentDate", "2020-01-01");
                        param2.Add("@psBranchNo", 1);
                        param2.Add("@pnDeptId", 1);
                        param2.Add("@pnGlobalView", "Y");

                        var rtn22 = new DapperDATAImplementation<GetApproveServiceDTO>();

                        var _response22 = await rtn22.LoadData("isp_GetAppoveService", param2, db);

                                 
                        countResult.DashBoardValues = _response22.Count();
                        countResult.RouterLink = b.RouterLink;
                        countResult.Icon = b.Icon;
                        countResult.IconColor = b.IconColor;
                        countResult.DashboardMenuName = b.DashboardMenuName;

                        resultCount.Add(countResult);
                        continue;
                     }

                    if(!string.IsNullOrWhiteSpace(b.Query))
                    {
                   
                       countResult = await count.LoadSingle(b.Query, db);

                        countResult.RouterLink = b.RouterLink;
                        countResult.Icon = b.Icon;
                        countResult.IconColor = b.IconColor;
                        countResult.DashboardMenuName = b.DashboardMenuName;

                        resultCount.Add(countResult);
                    }
                    else
                    {
                        resultCount.Add(b);
                    }
                }
              
                if(_response2 != null)
                {
                   var res = new {
                       _response = resultCount,
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
   



        [HttpPost("AssignDashboard")]
        public async Task<IActionResult> AssignDashboard(GenRoleAssignDTO p)
        {
            try
            {
                var getAllExist = await _repoadmDashBoardAssignment.GetManyAsync(c=> c.RoleId == p.RoleId);
                 foreach(var b in getAllExist) {

                      _repoadmDashBoardAssignment.Delete(c=> c.Itbid == b.Itbid);
                     
                 }

                  int save = await _ApplicationDbContext.SaveChanges(p.UserId);

                int? LoginUserId = 0;
                var _admadmDashBoardAssignment  = new List<admDashBoardAssignment>();
                foreach(var b in p.ListRoleAssignDTO){

                    _admadmDashBoardAssignment.Add(new admDashBoardAssignment
                    {
                            
                            RoleId = p.RoleId,
                            MenuId = b.MenuId,
                            CanView = b.CanView,
                            CanAdd = b.CanAdd,
                            CanEdit = b.CanEdit,
                            CanAuth = b.CanAuth,
                            CanDelete = b.CanDelete,
                            IsGlobalSupervisor = b.IsGlobalSupervisor,
                    });
                    
                    LoginUserId  = b.UserId;
                    
                }
                
              await _repoadmDashBoardAssignment.InsertRangeAsync(_admadmDashBoardAssignment);
              int _response =  await _ApplicationDbContext.SaveChanges(p.UserId);
                if(_response > 0)
                {
                    ApiResponse.ResponseCode = 0;
                    ApiResponse.ResponseMessage = "Dashboard Assigned Successfully!";
                  
                    
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
        


        [HttpPost("Assign")]
        public async Task<IActionResult> Assign(GenRoleAssignDTO p)
        {
            try
            {
                var getAllExist = await _repoadmRoleAssignment.GetManyAsync(c=> c.RoleId == p.RoleId);
                 foreach(var b in getAllExist) {

                      _repoadmRoleAssignment.Delete(c=> c.Itbid == b.Itbid);
                      int save = await _ApplicationDbContext.SaveChanges(p.UserId);
                 }

                int? LoginUserId = 0;
                var _admRoleAssignment = new List<admRoleAssignment>();
                foreach(var b in p.ListRoleAssignDTO){

                    _admRoleAssignment.Add(new admRoleAssignment
                    {
                            
                            RoleId = p.RoleId,
                            MenuId = b.MenuId,
                            CanView = b.CanView,
                            CanAdd = b.CanAdd,
                            CanEdit = b.CanEdit,
                            CanAuth = b.CanAuth,
                            CanDelete = b.CanDelete,
                            IsGlobalSupervisor = b.IsGlobalSupervisor,
                    });
                    
                    LoginUserId  = b.UserId;
                    
                }
                
              await _repoadmRoleAssignment.InsertRangeAsync(_admRoleAssignment);
              int _response =  await _ApplicationDbContext.SaveChanges(p.UserId);
                if(_response > 0)
                {
                    ApiResponse.ResponseCode = 0;
                    ApiResponse.ResponseMessage = "Menu Assigned Successfully!";
                  
                    
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
        
        [HttpPost("DeleteAllAssign")]
        public async Task<IActionResult> DeleteAllAssign(GenRoleAssignDTO p)
        {
            try
            {
                 int save = 0;
                var getAllExist = await _repoadmRoleAssignment.GetManyAsync(c=> c.RoleId == p.RoleId);
                 foreach(var b in getAllExist) {

                      _repoadmRoleAssignment.Delete(c=> c.Itbid == b.Itbid);
                       save = await _ApplicationDbContext.SaveChanges(p.UserId);
                 }
                if(save > 0)
                {
                    ApiResponse.ResponseCode = 0;
                    ApiResponse.ResponseMessage = "All Menu(s) Removed Successfully!";
                  
                    
                    return Ok(ApiResponse);
                }
                else
                {
                    ApiResponse.ResponseMessage = "Menu(s) Could'nt Removed";
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


        [HttpPost("DeleteAllAssignDashboard")]
        public async Task<IActionResult> DeleteAllAssignDashboard(GenRoleAssignDTO p)
        {
            try
            {
                 int save = 0;
                var getAllExist = await _repoadmDashBoardAssignment.GetManyAsync(c=> c.RoleId == p.RoleId);
                 foreach(var b in getAllExist) {

                      _repoadmDashBoardAssignment.Delete(c=> c.Itbid == b.Itbid);
                       save = await _ApplicationDbContext.SaveChanges(p.UserId);
                 }
                if(save > 0)
                {
                    ApiResponse.ResponseCode = 0;
                    ApiResponse.ResponseMessage = "All Dashbaord Removed Successfully!";
                  
                    
                    return Ok(ApiResponse);
                }
                else
                {
                    ApiResponse.ResponseMessage = "Dashbaord(s) Could'nt Removed";
                    return BadRequest(ApiResponse); 
                }
                  
            
            }
            catch (Exception ex)
            {
                 ApiResponse.ResponseMessage =  ex == null ? ex.InnerException.Message : ex.Message;
                 ApiResponse.ResponseMessage = "Error Occured while Removing Dashbaord, Contact System Admin";
                 ApiResponse.ResponseCode = -99;
                 return BadRequest(ApiResponse); 
            }
        }


        [HttpPost("GetRoles")]
        public async Task<IActionResult> GetRoles(AAuth AnyAuth)
        {
            try
            {
                var rtn = new DapperDATAImplementation<admRole>();

                string script = "Select * from admRole";
                
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

    
    
    }
}