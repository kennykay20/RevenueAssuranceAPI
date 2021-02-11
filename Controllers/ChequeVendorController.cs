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
using RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO;
using RevAssuranceApi.Response;
using RevAssuranceApi.TokenGen;
using RevAssuranceApi.AppSettings;
using RevAssuranceApi.RevenueAssurance.DATA.Models;
using RevAssuranceApi.RevenueAssurance.Repository.Interface;
using RevAssuranceApi.OperationImplemention;

namespace RevAssuranceApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [EnableCors("CorsPolicy")]
    public class ChequeVendorController : ControllerBase
    {
        IConfiguration _configuration;
        ApiResponse ApiResponse = new ApiResponse();
        TokenGenerator TokenGenerator;
        AppSettingsPath AppSettingsPath;
        IDbConnection db = null;
        ApplicationDbContext _ApplicationDbContext;

        IRepository<admChqVendor> _repoChqVendor;
        RoleAssignImplementation _RoleAssignImplementation;
        public ChequeVendorController(IConfiguration configuration, ApplicationDbContext ApplicationDbContext,
                                        IRepository<admChqVendor> repoChqVendor,
                                        RoleAssignImplementation RoleAssignImplementation)
        {
            _configuration = configuration;
            AppSettingsPath = new AppSettingsPath(_configuration);
            TokenGenerator = new TokenGenerator(_configuration);
            db = new SqlConnection(AppSettingsPath.GetDefaultCon());
            _ApplicationDbContext = ApplicationDbContext;
            _repoChqVendor = repoChqVendor;
            _RoleAssignImplementation = RoleAssignImplementation;
        }

        [HttpPost("GetAll")]
        public async Task<IActionResult> GetAll(ParamLoadPage AnyAuth)
        {
            try
            {
                  var roleAssign = await _RoleAssignImplementation.GetRoleAssign(AnyAuth.MenuId,AnyAuth.RoleId);
                
                var resList = new List<ChqVendorDTO>();

                //to get all chq vendors
                var vendors = await _repoChqVendor.GetManyAsync(c=> c.ItbId > 0);
                //logic for notification types
                foreach (var item in vendors)
                {
                    var res = new ChqVendorDTO();
                    if (item.NotificationType != null)
                    {
                        var notifications = item.NotificationType.Split(',');
                        foreach (var n in notifications)
                        {
                            if (n == "Api")
                            {
                                res.Api = "true";
                            }
                            if (n == "Email")
                            {
                                res.Email = "true";
                            }
                            if (n == "WebService")
                            {
                                res.Webservice = "true";
                            }
                        }
                        res.Api = res.Api == "true" ? "true" : null;
                        res.Email = res.Email == "true" ? "true" : null;
                        res.Webservice = res.Webservice == "true" ? "true" : null;
                    }
                    else
                    {
                        res.Api = null;
                        res.Email = null;
                        res.Webservice = null;
                    }

                    if (item.ChqProductCodes != null)
                    {
                        var codes = item.ChqProductCodes.Split(',');
                        foreach (var c in codes)
                        {
                            if (c == "8")
                            {
                                res.Twenty = "true";
                            }

                            if (c == "4")
                            {
                                res.Fifty = "true";
                            }

                            if (c == "5")
                            {
                                res.Hundred = "true";
                            }

                        }
                        res.Twenty = res.Twenty == "true" ? "true" : null;
                        res.Fifty = res.Fifty == "true" ? "true" : null;
                        res.Hundred = res.Hundred == "true" ? "true" : null;
                    }
                    else
                    {
                        res.Twenty = null;
                        res.Fifty = null;
                        res.Hundred = null;
                    }

                    res.VendorId = item.VendorId;
                    res.VendorName = item.VendorName;
                    res.VenEmail = item.VenEmail;
                    res.VenAltEmail = item.VenAltEmail;
                    res.VenAcctNo = item.VenAcctNo;
                    res.VenAcctType = item.VenAcctType;
                    res.VenAcctCcyCode = item.VenAcctCcyCode;
                    res.APIEndPoint = item.APIEndPoint;
                    res.SoapDefinition = item.SoapDefinition;
                    res.Status = item.Status;
                    res.ItbId = item.ItbId;
                    res.DateCreated = item.DateCreated;
                    res.UserId = item.UserId;
                    resList.Add(res);
                }

                if (resList != null)
                {
                    var res =  new {
                            roleAssign = roleAssign,
                            resList = resList
                    };
                    return Ok(res);
                }
                else
                {
                    return BadRequest(resList);
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
        public async Task<IActionResult> Add(ChqVendorDTO p)
        {
            try
            {
                var chq = new admChqVendor();
                //LOGIC for notification type
                var email = p.Email == "true" ? "Email," : "";
                var api = p.Api == "true" ? "Api," : "";
                var webServ = p.Webservice == "true" ? "WebService" : "";

                if (api == "" && webServ == "")
                {
                    chq.NotificationType = $"{email.Replace(",", "")}{api}{webServ}";
                }
                else if (email == "" && webServ == "")
                {
                    chq.NotificationType = $"{email}{api.Replace(",", "")}{webServ}";
                }
                else if (webServ == "")
                {
                    chq.NotificationType = $"{email}{api.Replace(",", "")}{webServ}";
                }
                else
                {
                    chq.NotificationType = $"{email}{api}{webServ}";
                }

                //logic forchq product codes
                var twen = p.Twenty == "true" ? "8," : "";
                var fift = p.Fifty == "true" ? "4," : "";
                var hund = p.Hundred == "true" ? "5" : "";

                if (fift == "" && hund == "")
                {
                    chq.ChqProductCodes = $"{twen.Replace(",", "")}{fift}{hund}";
                }
                else if (twen == "" && hund == "")
                {
                    chq.ChqProductCodes = $"{twen}{fift.Replace(",", "")}{hund}";
                }
                else if (hund == "")
                {
                    chq.ChqProductCodes = $"{twen}{fift.Replace(",", "")}{hund}";
                }
                else
                {
                    chq.ChqProductCodes = $"{twen}{fift}{hund}";
                }

                chq.VendorId = p.VendorId;
                chq.VendorName = p.VendorName;
                chq.VenEmail = p.VenEmail;
                chq.VenAltEmail = p.VenAltEmail;
                chq.VenAcctNo = p.VenAcctNo;
                chq.VenAcctType = p.VenAcctType;
                chq.VenAcctCcyCode = p.VenAcctCcyCode;
                chq.SoapDefinition = p.SoapDefinition;
                chq.APIEndPoint = p.APIEndPoint;
                chq.UserId = p.UserId;
                chq.DateCreated = DateTime.Now;
                chq.Status = _configuration["Statuses:ActiveStatus"];
                _ApplicationDbContext.admChqVendor.Add(chq);
                int _response = await _ApplicationDbContext.SaveChanges(chq.UserId);
                if (_response > 0)
                {
                    ApiResponse.sErrorText = _configuration["Message:AddedSuc"];
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
        public async Task<IActionResult> Update(ChqVendorDTO p)
        {
            try
            {
                var get = _ApplicationDbContext.admChqVendor.FirstOrDefault(c => c.ItbId == p.ItbId);
                if (get != null)
                {
                    get.VendorId = p.VendorId;
                    get.VendorName = p.VendorName;
                    get.VenAcctNo = p.VenAcctNo;
                    get.Status = p.Status;
                    get.VenEmail = p.VenEmail;
                    get.VenAltEmail = p.VenAltEmail;
                    get.VenAcctNo = p.VenAcctNo;
                    get.VenAcctType = p.VenAcctType;
                    get.SoapDefinition = p.SoapDefinition;
                    get.APIEndPoint = p.APIEndPoint;

                    //logic for notification type
                    var email = p.Email == "true" ? "Email," : "";
                    var api = p.Api == "true" ? "Api," : "";
                    var webServ = p.Webservice == "true" ? "WebService" : "";

                    if (api == "" && webServ == "")
                    {
                        get.NotificationType = $"{email.Replace(",", "")}{api}{webServ}";
                    }
                    else if (email == "" && webServ == "")
                    {
                        get.NotificationType = $"{email}{api.Replace(",", "")}{webServ}";
                    }
                    else if (webServ == "")
                    {
                        get.NotificationType = $"{email}{api.Replace(",", "")}{webServ}";
                    }
                    else
                    {
                        get.NotificationType = $"{email}{api}{webServ}";
                    }

                    //logic forchq product codes
                    var twen = p.Twenty == "true" ? "8," : "";
                    var fift = p.Fifty == "true" ? "4," : "";
                    var hund = p.Hundred == "true" ? "5" : "";

                    if (fift == "" && hund == "")
                    {
                        get.ChqProductCodes = $"{twen.Replace(",", "")}{fift}{hund}";
                    }
                    else if (twen == "" && hund == "")
                    {
                        get.ChqProductCodes = $"{twen}{fift.Replace(",", "")}{hund}";
                    }
                    else if (hund == "")
                    {
                        get.ChqProductCodes = $"{twen}{fift.Replace(",", "")}{hund}";
                    }
                    else
                    {
                        get.ChqProductCodes = $"{twen}{fift}{hund}";
                    }


                }
                _ApplicationDbContext.admChqVendor.Update(get);
                int _response = await _ApplicationDbContext.SaveChanges(p.UserId);
                if (_response > 0)
                {
                    ApiResponse.sErrorText = _configuration["Message:UpdateSuc"];
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

    }
}