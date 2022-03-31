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
using RevAssuranceApi.AppSettings;
using RevAssuranceApi.OperationImplemention;
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
    public class BroadCastMessageController : ControllerBase
    {
        IConfiguration _configuration;
        ApiResponse ApiResponse = new ApiResponse();
        TokenGenerator TokenGenerator;
        AppSettingsPath AppSettingsPath;
        IDbConnection db = null;
        ApplicationDbContext _ApplicationDbContext;
        RoleAssignImplementation _RoleAssignImplementation;
        IRepository<admBroadCast> _repoadmBroadCast;
        IRepository<admUserProfile> _repoadmUserProfile;
        Formatter _Formatter;
        public BroadCastMessageController(IConfiguration configuration,
                        ApplicationDbContext ApplicationDbContext,
                        RoleAssignImplementation RoleAssignImplementation,
                        IRepository<admBroadCast> repoadmBroadCast,
                        IRepository<admUserProfile> repoadmUserProfile,
                        Formatter Formatter)
        {

            _configuration = configuration;
            AppSettingsPath = new AppSettingsPath(_configuration);
            TokenGenerator = new TokenGenerator(_configuration);
            db = new SqlConnection(AppSettingsPath.GetDefaultCon());
            _ApplicationDbContext = ApplicationDbContext;
            _RoleAssignImplementation = RoleAssignImplementation;
            _repoadmBroadCast = repoadmBroadCast;
            _repoadmUserProfile = repoadmUserProfile; ;
            _Formatter = Formatter;
        }



        [HttpPost("GetAll")]
        public async Task<IActionResult> GetAll(ParamLoadPage AnyAuth)
        {
            try
            {
                var roleAssign = await _RoleAssignImplementation.GetRoleAssign(AnyAuth.MenuId, AnyAuth.RoleId);

                var rtn = new DapperDATAImplementation<admBroadCastDTO>();

                string script = @"Select 
                                  (Select DeptName  
                                   from admDepartment where DeptId = a.DeptId) DeptName,
                                   * 
                            from admBroadCast a";

                var _response = await rtn.LoadListNoParam(script, db);

                if (_response != null)
                {
                    var res = new
                    {
                        roleAssign = roleAssign,
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
                ApiResponse.ResponseMessage = ex.InnerException.Message ?? ex.Message;

                ApiResponse.ResponseCode = -99;
                return BadRequest(ApiResponse);
            }
        }


        [HttpPost("GetDueBroadCast")]
        public async Task<IActionResult> GetDueBroadCast(ParamLoadPage AnyAuth)
        {
            try
            {
                var roleAssign = await _RoleAssignImplementation.GetRoleAssign(AnyAuth.MenuId, AnyAuth.RoleId);

                var rtn = new DapperDATAImplementation<admBroadCastDTO>();
                var dt = DateTime.Now;
                string startDate = _Formatter.ValidateDateReturnString(DateTime.Now.ToString());
                string script1 = @"  Select top 1 StartDate,NotifyTimeInterval,
                            EndDate,                
						     (select  StartDate - cast(NotifyTimeInterval as datetime)) as Diff , 
                            * from admBroadCast a 
                            where
                            deptid = {deptid}
                            or TargetAudience = 'ALL'
                            and StartDate >= StartDate - cast(NotifyTimeInterval as datetime) 
                            and EndDate !> getDate()  and status = 'Active'".Replace("{deptid}", AnyAuth.pnDeptId);

                // string script = @"Select * from admBroadCast a 
                //                                 where 
                //                                 StartDate = CAST(StartDate AS DATETIME)
                //                                 and EndDate <  CAST(getDate() AS DATETIME)
                //                                 and Status = 'Active'";

                string script = @"Select * from admBroadCast a 
                                    where CONVERT(varchar, getDate(), 103) between CONVERT(varchar, StartDate, 103) 
                                    and CONVERT(varchar, EndDate, 103)
                                    and status = 'Active'";


                var _response = await rtn.LoadListNoParam(script, db);
                List<admBroadCastDTO> messageDisplay = new List<admBroadCastDTO>();


                if (_response != null)
                {
                    string selectOnlyNonRead = "";
                    foreach (var data in _response)
                    {
                        selectOnlyNonRead = $" Select * from admBroadCastDetails where BroadCastId = {data.ItbId} and UserId = {AnyAuth.UserId} and Status = 'Read'";

                        var rtnadmBroadCast = new DapperDATAImplementation<admBroadCastDetail>();

                        var broadCastToUpdate = await rtnadmBroadCast.LoadListNoParam(selectOnlyNonRead, db);

                        if (broadCastToUpdate.Count == 0)
                        {
                            messageDisplay.Add(data);
                        }

                    }

                    //string UpdateThePastOnes = $" Select * from admBroadCastDetails where BroadCastId = {_response}";



                    if (messageDisplay.Count > 0)
                    {
                        var res = new
                        {
                            roleAssign = roleAssign,
                            _response = messageDisplay,
                            respCode = 1
                        };
                        return Ok(res);
                    }

                    var ress = new
                    {
                        roleAssign = roleAssign,
                        respCode = 2
                    };
                    return Ok(ress);
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

        [HttpPost("ReadDueBroadCast")]
        public async Task<IActionResult> ReadDueBroadCast(ParamLoadPage AnyAuth)
        {
            try
            {
                var roleAssign = await _RoleAssignImplementation.GetRoleAssign(AnyAuth.MenuId, AnyAuth.RoleId);

                var rtn = new DapperDATAImplementation<admBroadCastDTO>();
                var dt = DateTime.Now;
                string startDate = _Formatter.ValidateDateReturnString(DateTime.Now.ToString());



                List<admBroadCastDTO> messageDisplay = new List<admBroadCastDTO>();


                var selectOnlyNonRead = $" Select * from admBroadCast where Itbid = {AnyAuth.BroadCastId} and Status = 'Active'";

                var rtnadmBroadCast = new DapperDATAImplementation<admBroadCast>();

                var broadCastToUpdate = await rtnadmBroadCast.LoadListNoParam(selectOnlyNonRead, db);

                if (broadCastToUpdate.Count >= 0)
                {
                    admBroadCastDetail data = new admBroadCastDetail();
                    data.BroadCastId = Convert.ToInt32(AnyAuth.BroadCastId);
                    data.DateCreated = DateTime.UtcNow;
                    data.UserId = Convert.ToInt32(AnyAuth.UserId);
                    data.Status = "Read";

                    _ApplicationDbContext.admBroadCastDetails.Add(data);
                    int _response = await _ApplicationDbContext.SaveChanges(AnyAuth.UserId);
                    if (_response > 0)
                    {
                        var resp = new
                        {
                            roleAssign = roleAssign,
                            respCode = 1
                        };
                        return Ok(resp);
                    }
                }
                //string UpdateThePastOnes = $" Select * from admBroadCastDetails where BroadCastId = {_response}";

                var res = new
                {
                    roleAssign = roleAssign,
                    respCode = 2
                };
                return BadRequest(res);


            }
            catch (Exception ex)
            {
                ApiResponse.ResponseMessage = ex.InnerException.Message ?? ex.Message;

                ApiResponse.ResponseCode = -99;
                return BadRequest(ApiResponse);
            }
        }


        [HttpPost("Add")]
        public async Task<IActionResult> Add(admBroadCast p)
        {
            try
            {
                p.DateCreated = DateTime.Now;
                p.Status = "Active";

                if (p.EndDate < p.StartDate)
                {
                    ApiResponse.ResponseMessage = "Start Date Cannot be greater than End Date";
                    return BadRequest(ApiResponse);
                }
                if (p.EndDate == p.StartDate)
                {
                    ApiResponse.ResponseMessage = "Start Date and End Date Cannot be the same";
                    return BadRequest(ApiResponse);
                }
                await _repoadmBroadCast.AddAsync(p);
                int save = await _ApplicationDbContext.SaveChanges(p.UserId);
                if (save > 0)
                {
                    ApiResponse.ResponseMessage = "Broadcast Message Set Up Successfully!";
                    return Ok(ApiResponse);
                }
                else
                {
                    ApiResponse.ResponseMessage = "Error Occured while Setting Up Broadcast, Kindly contact system admin";
                    return BadRequest(ApiResponse);
                }


            }
            catch (Exception ex)
            {
                ApiResponse.ResponseMessage = ex == null ? ex.InnerException.Message : ex.Message;

                ApiResponse.ResponseCode = -99;
                return BadRequest(ApiResponse);
            }
        }


        [HttpPost("Update")]
        public async Task<IActionResult> Update(admBroadCast p)
        {
            try
            {
                if (p.EndDate < p.StartDate)
                {
                    ApiResponse.ResponseMessage = "Start Date Cannot be greater than End Date";
                    return BadRequest(ApiResponse);
                }

                if (p.EndDate == p.StartDate)
                {
                    ApiResponse.ResponseMessage = "Start Date and End Date Cannot be the same";
                    return BadRequest(ApiResponse);
                }

                var get = await _repoadmBroadCast.GetAsync(x => x.ItbId == p.ItbId);
                if (get != null)
                {
                    if (
                        get.Message == p.Message && get.StartDate == p.StartDate && get.EndDate == p.EndDate
                        && get.DeptId == p.DeptId && get.TargetAudience == p.TargetAudience && get.NotifyTimeInterval == p.NotifyTimeInterval
                        && get.Subject == p.Subject && get.BroadcastType == p.BroadcastType
                    )
                    {
                        ApiResponse.ResponseMessage = "No changes was made";
                        ApiResponse.ResponseCode = -99;
                        return Ok(ApiResponse);
                    }
                }

                get.Message = p.Message;
                get.StartDate = p.StartDate;
                get.EndDate = p.EndDate;
                get.DeptId = p.DeptId;
                get.TargetAudience = p.TargetAudience;
                get.Subject = p.Subject;
                get.BroadcastType = p.BroadcastType;
                get.NotifyTimeInterval = p.NotifyTimeInterval;
                get.Status = p.Status;
                get.DeptId = p.DeptId;
                get.DeptId = p.UserId;

                _ApplicationDbContext.admBroadCast.Update(get);
                int _response = await _ApplicationDbContext.SaveChanges(p.UserId);
                if (_response > 0)
                {
                    ApiResponse.ResponseMessage = "Broadcast Message Updated Successfully!";
                    ApiResponse.ResponseCode = 1;
                    return Ok(ApiResponse);
                }

                ApiResponse.ResponseMessage = "Error Occured";
                return BadRequest(ApiResponse);

            }
            catch (Exception ex)
            {
                ApiResponse.ResponseMessage = ex == null ? ex.InnerException.Message : ex.Message;

                ApiResponse.ResponseCode = -99;
                return BadRequest(ApiResponse);
            }
        }


        [HttpPost("GetById")]
        public async Task<IActionResult> GetById(admBroadCast p)
        {
            try
            {
                var get = await _repoadmBroadCast.GetAsync(c => c.ItbId == p.ItbId);
                var getUser = await _repoadmUserProfile.GetAsync(c => c.UserId == get.UserId);
                if (get != null)
                {
                    string startDate = get.StartDate != null ? string.Format("{0:dd-MMM-yyyy h:mm:tt}", get.StartDate) : string.Empty;
                    string endDate = get.EndDate != null ? string.Format("{0:dd-MMM-yyyy h:mm:tt}", get.EndDate) : string.Empty;

                    var res = new
                    {
                        get = get,
                        getUser = getUser,
                        startDate = startDate,
                        endDate = endDate

                    };

                    return Ok(res);
                }
                else
                {
                    return BadRequest(ApiResponse);
                }


            }
            catch (Exception ex)
            {
                ApiResponse.ResponseMessage = ex == null ? ex.InnerException.Message : ex.Message;

                ApiResponse.ResponseCode = -99;
                return BadRequest(ApiResponse);
            }
        }


    }
}