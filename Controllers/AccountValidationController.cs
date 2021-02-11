using System.Security.AccessControl;
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
using RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO;
using System.Net.Http;
using Newtonsoft.Json;
using RevAssuranceApi.OperationImplemention;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using RevAssuranceApi.RevenueAssurance.DATA.Models;
using RevAssuranceApi.Response;

namespace RevAssuranceApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [EnableCors("CorsPolicy")]
    //[Authorize]
    public class AccountValidationController : ControllerBase
    {
         IConfiguration _configuration;
        ApiResponse ApiResponse = new ApiResponse();
        TokenGenerator TokenGenerator;  
        AppSettingsPath AppSettingsPath ;
         IDbConnection db = null;
         ApplicationDbContext   _ApplicationDbContext;

         AccountValidationImplementation _AccountValidationImplementation;
         ComputeChargesImplementation _ComputeChargesImplementation;
         Formatter _Formatter;
        public AccountValidationController(
                                        IConfiguration configuration,
                                        ApplicationDbContext   ApplicationDbContext,
                                        AccountValidationImplementation AccountValidationImplementation,
                                        ComputeChargesImplementation ComputeChargesImplementation,
                                        Formatter Formatter) 
        {
           _configuration = configuration;
           AppSettingsPath = new AppSettingsPath(_configuration);
           TokenGenerator = new TokenGenerator(_configuration);
           db = new SqlConnection(AppSettingsPath.GetDefaultCon());
           _ApplicationDbContext =  ApplicationDbContext;

           _AccountValidationImplementation = AccountValidationImplementation;
           _ComputeChargesImplementation = ComputeChargesImplementation;
           _Formatter = Formatter;
        }
   
    [HttpPost("GetAll")]
    public async Task<IActionResult> GetAll(AccountValParam AccountValParam)
    {
            try
            {

                 AccountValidationDTO values = new AccountValidationDTO();

                  if(string.IsNullOrWhiteSpace(AccountValParam.AcctType)){
                        AccountValParam.AcctType = null;
                    }

                     //if(string.IsNullOrWhiteSpace(AccountValParam.CcyCode)){
                       //AccountValParam.CcyCode = null;
                   //}
                    
                    //AccountValParam.AcctNo = p.AcctNo;
                    //AccountValParam.AcctType = p.AcctType == null ? null : p.AcctType.Trim();
                    
                   // AccountValParam.CrncyCode = p.CcyCode;
                   // AccountValParam.Username = AccountValParam.LoginUserName;
                
              values = await _AccountValidationImplementation.ValidateAccountCall(AccountValParam);
                   
            values.AcctType =     values.sAccountType;
            values.AcctStatus =     values.sStatus;
            values.AcctName =     values.sName;
            values.AvailBal =     values.nBalance;
            values.AcctType =     values.sAccountType;
            values.CcyCode = values.sCrncyIso;
            values.AcctCCy =  values.sCrncyIso;
            values.AcctNo = AccountValParam.AcctNo;
              return  Ok(values);
           }
                
            catch (Exception ex)
            {
                 ApiResponse.ResponseMessage =  ex == null ? ex.InnerException.Message : ex.Message;
                
                 ApiResponse.ResponseCode = -99;
                 return BadRequest(ApiResponse); 
            }

            
        }

          [HttpPost("GetReadEndDate")]
        public  IActionResult GetReadEndDate(ArmotizationParam param)
        {
            var rtv = new ReturnValues();
            
            DateTime dt = new DateTime();
            int Tenur = 0;
            try
            {
                if (DateTime.TryParse(param.StartDate, out dt) && int.TryParse(param.Tenure, out Tenur))
                {
                    if (param.TimeBasis == "DAYS")
                    {
                        Tenur = Tenur * 1;
                    }
                    else if (param.TimeBasis == "WEEKS")
                    {
                        Tenur = Tenur * 7;
                    }
                    else if (param.TimeBasis == "MONTHS")
                    {
                        int Month = dt.Month;
                        if (Month == 9 || Month == 4 || Month == 6 || Month == 11)
                        {
                            Tenur = Tenur * 30;
                        }
                        else if (Month == 1 || Month == 3 || Month == 5 || Month == 7 || Month == 8 || Month == 10 || Month == 12)
                        {
                            Tenur = Tenur * 31;
                        }
                        else if (Month == 2)
                        {
                            if (DateTime.IsLeapYear(dt.Year))
                            {
                                Tenur = Tenur * 29;
                            }
                            else
                            {
                                Tenur = Tenur * 28;
                            }

                        }
                    }
                    else if (param.TimeBasis == "YEARS")
                    {

                        Tenur = Tenur * 365;

                    }
                    
                    DateTime dte = dt.AddDays(Tenur);
                    rtv.nErrorCode = 0;
                    rtv.sErrorText = "Success";
                    if (param.TimeBasis == "DAYS")
                    {
                        rtv.NoInstalmt = Convert.ToInt32(Math.Abs(Math.Round(dte.Subtract(dt).TotalDays, 0)));
                    }
                    if (param.TimeBasis == "MONTHS")
                    {
                        rtv.NoInstalmt = Convert.ToInt32(Math.Abs(Math.Round(dte.Subtract(dt).Days / (365.25 / 12), 0)));
                        double dtf = dte.Subtract(dt).Days / (365.25 / 12);
                    }
                    if (param.TimeBasis == "YEARS")
                    {
                        rtv.NoInstalmt = dte.Year - dt.Year;
                    }

                    rtv.InstlmtAmount = _Formatter.FormattedAmount(Convert.ToDecimal(Math.Round(Convert.ToDouble(param.TotalAmount) / (double)rtv.NoInstalmt, 2)));

                    double dtf2 = dte.Subtract(dt).TotalDays;

                    rtv.FirstInstlmtDate = _Formatter.FormatTransDate(dt);

                    if (_ComputeChargesImplementation.GetNextRunDate(dt, param.TimeBasis) > dte)
                    {

                        rtv.NextInstlmtDate = null;
                    }
                    else if (_Formatter.FormatTransDate(_ComputeChargesImplementation.GetNextRunDate(dt, param.TimeBasis)) == _Formatter.FormatTransDate(dte))
                    {
                        rtv.NextInstlmtDate = null;
                    }
                    else
                    {
                        rtv.NextInstlmtDate = _Formatter.FormatTransDate(_ComputeChargesImplementation.GetNextRunDate(dt, param.TimeBasis));

                    }

                    rtv.FinalInstlmtDate = _Formatter.FormatTransDate(_ComputeChargesImplementation.GetEndRunDate(dt, param.TimeBasis, (int)rtv.NoInstalmt, _Formatter.FormatTransDate(_ComputeChargesImplementation.GetNextRunDate(dt, param.TimeBasis)), Tenur));
                    rtv.expiryDate = _Formatter.FormatTransDate(dt.AddDays(Tenur));
                    return Ok(rtv);
                }



            }
            catch (Exception ex)
            {

              var exM = ex;
            }

              return BadRequest(rtv);
        }

   
    
    }
}