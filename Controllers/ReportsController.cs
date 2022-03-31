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
using RevAssuranceApi.Helper;
using RevAssuranceApi.RevenueAssurance.DATA.Models;
using RevAssuranceApi.TokenGen;
using RevAssuranceApi.Response;
using RevAssuranceApi.AppSettings;
using RevAssuranceApi.RevenueAssurance.Repository.DapperDAL;
using RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO;
using RevAssuranceApi.OperationImplemention;
using Newtonsoft.Json;


namespace RevAssuranceApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [EnableCors("CorsPolicy")]

    public class ReportsController : ControllerBase
    {
         IConfiguration _configuration;
         ApiResponse ApiResponse = new ApiResponse();
         TokenGenerator TokenGenerator;  
         AppSettingsPath AppSettingsPath ;
         IDbConnection db = null;
         ApplicationDbContext   _ApplicationDbContext;
         Formatter _Formatter = new Formatter();
        public ReportsController(IConfiguration configuration,ApplicationDbContext   ApplicationDbContext)
        {      
           _configuration = configuration;
           AppSettingsPath = new AppSettingsPath(_configuration);
           TokenGenerator = new TokenGenerator(_configuration);
           db = new SqlConnection(AppSettingsPath.GetDefaultCon());
           _ApplicationDbContext =  ApplicationDbContext;
        }

        [HttpPost("AuditTrail")]
        public async Task<IActionResult> AuditTrail(QueryDto AnyAuth)
        {
                try
                {
                    DynamicParameters param = new DynamicParameters();

                    string frmDate = AnyAuth.FromDate != null ? _Formatter.FormatToDateYearMonthDay(AnyAuth.FromDate): null;
                    string ToDate = AnyAuth.ToDate != null ? _Formatter.FormatToDateYearMonthDay(AnyAuth.ToDate) : null;

                    param.Add("@pdtFromDate", frmDate);
                    param.Add("@pdtToDate", ToDate);
                    param.Add("@pnUserId", AnyAuth.UserId);
                    param.Add("@psTableName", AnyAuth.TableName);

                   var rtn = new DapperDATAImplementation<auditTrailDTO>();
                   var _response = await rtn.LoadData("Isp_AuditTRailRpt", param, db);
                    return Ok(_response); 
                }
                catch (Exception ex)
                {
                     ApiResponse.ResponseMessage = ex.InnerException.Message ?? ex.Message;
                    ApiResponse.ResponseCode = -99;
                      ApiResponse.ResponseMessage = "Could'nt Execute Reports";
                    return BadRequest(ApiResponse);
                }
        }

        [HttpPost("Transaction")]
        public async Task<IActionResult> Transaction(ReportParam AnyAuth)
        {
                try
                {
                    DynamicParameters param = new DynamicParameters();

                    string frmDate = AnyAuth.pdtStartDate != null ? _Formatter.FormatToDateYearMonthDay(AnyAuth.pdtStartDate): null;
                    string ToDate = AnyAuth.pdtEndDate != null ? _Formatter.FormatToDateYearMonthDay(AnyAuth.pdtEndDate) : null;

                    param.Add("@pnUserId", AnyAuth.pnUserId);
                    param.Add("@psBranchCode", AnyAuth.psBranchCode);
                    param.Add("@psCurrencyIso", AnyAuth.psCurrencyIso);
                    param.Add("@psDrAcctNo", AnyAuth.psDrAcctNo);
                    param.Add("@psCrAcctNo", AnyAuth.psCrAcctNo);
                    param.Add("@pdtStartDate", frmDate);
                    param.Add("@pdtEndDate", ToDate);
                    param.Add("@psTranRef", AnyAuth.psTranRef);
                    param.Add("@psStatus", AnyAuth.psStatus);
                    param.Add("@pnAmnt", AnyAuth.TotalAmt);
                    param.Add("@pnServiceId", AnyAuth.pnServiceId);
                    param.Add("@pnDeptId", AnyAuth.pnDeptId);
                    param.Add("@pnbatchId", AnyAuth.pnbatchId);
                    
                   var rtn = new DapperDATAImplementation<CbsTransactionDTOReport>();
                   var _response = await rtn.LoadData("Isp_TransactionReport", param, db);

                   var res = new {
                       _response = _response
                   };
                   return Ok(res); 
                }
                catch (Exception ex)
                {
                     ApiResponse.ResponseMessage = ex.InnerException.Message ?? ex.Message;
                    ApiResponse.ResponseCode = -99;
                      ApiResponse.ResponseMessage = "Could'nt Execute Reports";
                    return BadRequest(ApiResponse);
                }        
        }

        [HttpPost("Contigency")]
        public async Task<IActionResult> Contigency(ReportParam AnyAuth)
        {
                try
                {
                    DynamicParameters param = new DynamicParameters();

                    string frmDate = AnyAuth.pdtStartDate != null ? _Formatter.FormatToDateYearMonthDay(AnyAuth.pdtStartDate): null;
                    string ToDate = AnyAuth.pdtEndDate != null ? _Formatter.FormatToDateYearMonthDay(AnyAuth.pdtEndDate) : null;

                    param.Add("@pnUserId", AnyAuth.pnUserId);
                    param.Add("@psBranchCode", AnyAuth.psBranchCode);
                    param.Add("@psCurrencyIso", AnyAuth.psCurrencyIso);
                    param.Add("@psDrAcctNo", AnyAuth.psDrAcctNo);
                    param.Add("@psCrAcctNo", AnyAuth.psCrAcctNo);
                    param.Add("@pdtStartDate", frmDate);
                    param.Add("@pdtEndDate", ToDate);
                    param.Add("@psTranRef", AnyAuth.psTranRef);
                    param.Add("@psStatus", AnyAuth.psStatus);
                    param.Add("@pnAmnt", AnyAuth.TotalAmt);
                    param.Add("@pnServiceId", AnyAuth.pnServiceId);
                    param.Add("@pnDeptId", AnyAuth.pnDeptId);
                    param.Add("@pnbatchId", AnyAuth.pnbatchId);
                    
                   var rtn = new DapperDATAImplementation<CbsTransactionDTOReport>();
                   var _response = await rtn.LoadData("Isp_ContigencyReport", param, db);

                   var res = new {
                       _response = _response
                   };
                   return Ok(res); 
                }
                catch (Exception ex)
                {
                     ApiResponse.ResponseMessage = ex.InnerException.Message ?? ex.Message;
                    ApiResponse.ResponseCode = -99;
                      ApiResponse.ResponseMessage = "Could'nt Execute Reports";
                    return BadRequest(ApiResponse);
                }        
        }


        [HttpPost("GetAuditTrailByTableName")]
        public async Task <IEnumerable<auditTrailDTO>> GetAuditTrailByTableName(auditTables param)
        {
            List<auditTrailDTO> auditTrailDTOs = new List<auditTrailDTO>();

            var appUsers = _ApplicationDbContext.admUserProfile.Select(x => new admUserProfile { UserId = x.UserId, FullName = x.FullName});

            var auditTrails = _ApplicationDbContext.admAuditTrail.Where(x => x.Tablename == param.tableName);

            if (!auditTrails.Any())
            {
                return null;
            }

            var query = auditTrails.Select(x => new auditTrailDTO
            {
                Eventdateutc = x.Eventdateutc,
                Eventtype = x.Eventtype,
                Newvalue = x.Newvalue,
                Originalvalue = x.Originalvalue,
                UserFullName = appUsers.FirstOrDefault(y => y.UserId == x.Userid).FullName,
                Recordid = x.Recordid,
                ColumnName = x.ColumnName
            });

            foreach (var item in query)
            {
                //var objTrails = ConvertJsonToDic(item.Originalvalue);

                // foreach (var obj in objTrails)
                // {
                   
                // }
                var newAuditTrail = new auditTrailDTO
                {
                    ColumnName = item.ColumnName,
                    Originalvalue = item.Originalvalue,
                    Newvalue = item.Newvalue,
                    Eventdateutc = item.Eventdateutc,
                    Eventtype = item.Eventtype,
                    Recordid = item.Recordid,
                    UserFullName = item.UserFullName

                };
                auditTrailDTOs.Add(newAuditTrail);
            }

            return auditTrailDTOs.AsEnumerable();
        }

        private Dictionary <string, string> ConvertJsonToDic(string json)
        {
            var converted = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            return converted;
        }
    }

}

