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
using RevAssuranceWebAPi.AnythingGood.DATA.Models;
using Microsoft.EntityFrameworkCore;
using RevAssuranceApi.RevenueAssurance.DATA.Models;
using RevAssuranceApi.RevenueAssurance.Repository.DapperDAL;
using RevAssuranceApi.Response;
using RevAssuranceApi.RevenueAssurance.Repository.Interface;
using RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO;
using RevAssuranceApi.OperationImplemention;

namespace RevAssuranceApi.Controllers
{

    [Route("api/v1/[controller]")]
    [ApiController]
    [EnableCors("CorsPolicy")]
    //[Authorize]
    public class DepartmentController : ControllerBase
    {
        IConfiguration _configuration;
        ApiResponse ApiResponse = new ApiResponse();
        TokenGenerator TokenGenerator;
        AppSettingsPath AppSettingsPath;
        IDbConnection db = null;
        ApplicationDbContext _ApplicationDbContext;
        IRepository<admDepartment> _repoadmDepartment;
        IRepository<admBankBranch> _repoadmBankBranch;
        IRepository<admRole> _repoadmRole;
        IRepository<admUserProfile> _repoadmUserProfile;
        RoleAssignImplementation _RoleAssignImplementation;
        public DepartmentController(IConfiguration configuration,
                                    ApplicationDbContext ApplicationDbContext,
                                    IRepository<admDepartment> repoadmDepartment,
                                    IRepository<admUserProfile> repoadmUserProfile,
                                    RoleAssignImplementation RoleAssignImplementation,
                                    IRepository<admBankBranch> repoadmBankBranch,
                                     IRepository<admRole> repoadmRole)
        {
            _configuration = configuration;
            AppSettingsPath = new AppSettingsPath(_configuration);
            TokenGenerator = new TokenGenerator(_configuration);
            db = new SqlConnection(AppSettingsPath.GetDefaultCon());
            _ApplicationDbContext = ApplicationDbContext;
            _repoadmDepartment = repoadmDepartment;
            _RoleAssignImplementation = RoleAssignImplementation;
            _repoadmBankBranch = repoadmBankBranch;
            _repoadmRole = repoadmRole;
            _repoadmUserProfile = repoadmUserProfile;
        }


        [HttpPost("GetAll")]
        public async Task<IActionResult> GetAll(ParamLoadPage AnyAuth)
        {
            try
            {
                var roleAssign = await _RoleAssignImplementation.GetRoleAssign(AnyAuth.MenuId, AnyAuth.RoleId);

                var rtn = new DapperDATAImplementation<admDepartment>();

                string script = "Select * from admDepartment";

                var _response = await rtn.LoadListNoParam(script, db);

                if (_response != null)
                {
                    var res = new
                    {
                        _response = _response,
                        roleAssign = roleAssign
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
                ApiResponse.ResponseMessage = ex.InnerException.Message ?? ex.Message;

                ApiResponse.ResponseCode = -99;
                return BadRequest(ApiResponse);
            }


        }

        [HttpPost("Add")]
        public async Task<IActionResult> Add(admDepartmentDTO p)
        {
            try
            {
                admDepartment adm = new admDepartment();

                var getDept = await _repoadmDepartment.GetAsync(c => c.Deptname == p.admDepartment.Deptname);
                if (getDept != null)
                {
                    ApiResponse.ResponseMessage = "Department Already Exist!";
                    return BadRequest(ApiResponse);
                }

                var getDeptCode = await _repoadmDepartment.GetAsync(c => c.DeptCode == p.admDepartment.DeptCode);
                if (getDeptCode != null)
                {
                    ApiResponse.ResponseMessage = "Department Code Already Exist!";
                    return BadRequest(ApiResponse);
                }

                // adm.DeptCode = p.admDepartment.DeptCode;
                adm.Deptname = p.admDepartment.Deptname;
                adm.HODName = p.admDepartment.HODName;
                adm.HODEmail = p.admDepartment.HODEmail;
                adm.HODAddress = p.admDepartment.HODAddress;
                adm.UserId = (int)p.LoginUserId;
                adm.DateCreated = DateTime.Now;
                adm.DeptCode = p.admDepartment.DeptCode;
                adm.Status = _configuration["Statuses:ActiveStatus"];
                await _repoadmDepartment.AddAsync(adm);
                int _response = await _ApplicationDbContext.SaveChanges(p.LoginUserId);
                if (_response > 0)
                {
                    ApiResponse.ResponseMessage = _configuration["Message:AddedSuc"];
                    return Ok(ApiResponse);
                }
                else
                {
                    return BadRequest(_response);
                }
            }
            catch (Exception ex)
            {
                ApiResponse.ResponseMessage = ex.InnerException.Message ?? ex.Message;

                ApiResponse.ResponseCode = -99;
                return BadRequest(ApiResponse);
            }
        }

        [HttpPost("Update")]
        public async Task<IActionResult> Update(admDepartmentDTO p)
        {
            try
            {
                var get = await _repoadmDepartment.GetAsync(c => c.DeptId == p.admDepartment.DeptId);
                if (get != null)
                {

                    if (get.Deptname == p.admDepartment.Deptname && get.HODAddress == p.admDepartment.HODAddress
                     && get.HODEmail == p.admDepartment.HODEmail && get.HODName == p.admDepartment.HODName &&
                     get.DeptCode == p.admDepartment.DeptCode && get.Status == p.admDepartment.Status)
                    {
                        ApiResponse.ResponseMessage = "No changes was made";
                        ApiResponse.ResponseCode = -99;
                        return Ok(ApiResponse);
                    }
                    get.Deptname = p.admDepartment.Deptname;
                    get.HODAddress = p.admDepartment.HODAddress;
                    get.HODEmail = p.admDepartment.HODEmail;
                    get.HODName = p.admDepartment.HODName;
                    get.DeptCode = p.admDepartment.DeptCode;
                    get.Status = p.admDepartment.Status;
                }
                _ApplicationDbContext.admDepartment.Update(get);
                int _response = await _ApplicationDbContext.SaveChanges(p.LoginUserId);
                if (_response > 0)
                {
                    ApiResponse.ResponseMessage = _configuration["Message:UpdateSuc"];
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
                ApiResponse.ResponseMessage = ex == null ? ex.InnerException.Message : ex.Message;

                ApiResponse.ResponseCode = -99;
                return BadRequest(ApiResponse);
            }
        }

        [HttpPost("GetAllDepAndbranch")]
        public async Task<IActionResult> GetAllDepAndbranch(ParamLoadPage AnyAuth)
        {
            try
            {
                var roleAssign = await _RoleAssignImplementation.GetRoleAssign(AnyAuth.MenuId, AnyAuth.RoleId);


                var _response = await _repoadmDepartment.GetManyAsync(c => c.DeptId > 0);

                var _response1 = await _repoadmBankBranch.GetManyAsync(c => c.Itbid > 0);

                var _response2 = await _repoadmRole.GetManyAsync(c => c.RoleId > 0);

                if (_response != null)
                {
                    var res = new
                    {
                        _response = _response,
                        roleAssign = roleAssign,
                        _response1 = _response1,
                        _response2 = _response2
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
                ApiResponse.ResponseMessage = ex.InnerException.Message ?? ex.Message;

                ApiResponse.ResponseCode = -99;
                return BadRequest(ApiResponse);
            }


        }


        [HttpPost("GetBy")]
        public async Task<IActionResult> GetBy(ParamLoadPage p)
        {
            try
            {
                var getCretedBy = await _repoadmUserProfile.GetAsync(c => c.UserId == p.UserId);

                return Ok(getCretedBy);
            }
            catch (Exception ex)
            {
                var exM = ex == null ? ex.InnerException.Message : ex.Message;

            }

            return BadRequest();
        }

    }
}