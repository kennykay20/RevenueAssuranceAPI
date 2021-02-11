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
using RevAssuranceApi.RevenueAssurance.Repository.DapperDAL;
using RevAssuranceApi.Response;
using RevAssuranceApi.AppSettings;
using RevAssuranceApi.RevenueAssurance.DATA.Models;
using RevAssuranceApi.TokenGen;
using RevAssuranceApi.RevenueAssurance.Repository.Interface;
using RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO;

namespace RevAssuranceApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [EnableCors("CorsPolicy")]
    public class ChargeController : ControllerBase
    {
        IConfiguration _configuration;
        ApiResponse ApiResponse = new ApiResponse();
        TokenGenerator TokenGenerator;
        AppSettingsPath AppSettingsPath;
        IDbConnection db = null;
        ApplicationDbContext _ApplicationDbContext;

        IRepository<admChargeExemption> _repoChargeExemption;

        IRepository<admCharge> _repoCharge;
        public ChargeController(IRepository<admChargeExemption> repoChargeExemption,
                                IRepository<admCharge> repoCharge, IConfiguration configuration, ApplicationDbContext ApplicationDbContext)
        {
            _configuration = configuration;
            AppSettingsPath = new AppSettingsPath(_configuration);
            TokenGenerator = new TokenGenerator(_configuration);
            db = new SqlConnection(AppSettingsPath.GetDefaultCon());
            _ApplicationDbContext = ApplicationDbContext;
            _repoChargeExemption = repoChargeExemption;
            _repoCharge = repoCharge;
        }


        [HttpPost("GetAll")]
        public async Task<IActionResult> GetAll([FromBody] AAuth AnyAuth)
        {
            try
            {
                var res = new DapperDATAImplementation<admCharge>();

                string script = "Select * from admCharge";

                var _response = await res.LoadListNoParam(script, db);

                if (_response != null)
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
                ApiResponse.ResponseMessage = ex.InnerException.Message ?? ex.Message;

                ApiResponse.ResponseCode = -99;
                return BadRequest(ApiResponse);
            }
        }

        [HttpPost("GetByChargeCode")]
        public async Task<IActionResult> GetByChargeCode(ChargeDTO c)
        {

            try
            {
                var charge = await _repoCharge.GetAsync(p => p.ChargeCode == c.ChargeCode);
                if (charge != null)
                {
                    return Ok(charge);
                }
                return BadRequest(new ApiResponse
                {
                    sErrorText = "Chargecode does not exist",
                    ResponseCode = -1
                });
            }
            catch (Exception ex)
            {
                ApiResponse.ResponseMessage = ex.InnerException.Message ?? ex.Message;

                ApiResponse.ResponseCode = -99;
                return BadRequest(ApiResponse);
            }
        }

        [HttpPost("Add")]
        public async Task<IActionResult> Add(admCharge p)

        {
            try
            {
                //would need to initialise every thing tax
                //  p.TaxAbbrevation="";
                // p.TaxAcctNo="";
                // p.TaxAcctType = "";
                // p.TaxCurrency ="";

                p.DateCreated = DateTime.Now;
                p.Status = _configuration["Statuses:ActiveStatus"];
               // p.TimeBased = "N";
                p.SubjectToTax = p.SubjectToTax == "true" ? "Y" : "N";
                _ApplicationDbContext.admCharge.Add(p);
                int _response = await _ApplicationDbContext.SaveChanges(p.UserId);
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
        public async Task<IActionResult> Update([FromBody] admCharge p)
        {
            try
            {
                var get = _ApplicationDbContext.admCharge.FirstOrDefault(c => c.ItbId == p.ItbId);
                 if(get != null)
                 {
                     get.Description = p.Description;
                     get.ServiceId = p.ServiceId;
                     get.RecipientBranch = p.RecipientBranch;
                     get.ChargeType = p.ChargeType;
                     get.DRCR = p.DRCR;
                     get.ChargeBasis = p.ChargeBasis;
                     get.Direction = p.Direction;
                     get.CurrencyIso = p.CurrencyIso;
                     get.ChargeValue = p.ChargeValue;
                     get.ChargeAcctType = p.ChargeAcctType;
                     get.ChargeAcctNo = p.ChargeAcctNo;
                     get.MinimumChargeCurr = p.MinimumChargeCurr;
                     get.MinimumCharge = p.MinimumCharge;
                     get.MaximumChargeCurr = p.MaximumChargeCurr;
                     get.MaximumCharge = p.MaximumCharge;
                     get.ChargeFloorCurr = p.ChargeFloorCurr;
                     get.ChargeFloor = p.ChargeFloor;
                     get.Ammendment =p.Ammendment;
                     get.RePrint  =p.RePrint;
                     get.TemplateId = p.TemplateId;

                 }
                _ApplicationDbContext.admCharge.Update(get);
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
                ApiResponse.ResponseMessage = ex.InnerException.Message ?? ex.Message;

                ApiResponse.ResponseCode = -99;
                return BadRequest(ApiResponse);
            }


        }


        //for charge concession
        [HttpPost("GetConcessionByChargeCode")]
        public async Task<IActionResult> GetConcession(ChargeDTO concession)
        {
            try
            {
                var _response = await _repoChargeExemption.GetManyAsync(p => p.ChargeCode == concession.ChargeCode);

                if (_response != null)
                {
                    return Ok(_response);
                }
                else
                {
                    return Ok(new List<admChargeExemption>());
                }
            }
            catch (Exception ex)
            {
                ApiResponse.ResponseMessage = ex.InnerException.Message ?? ex.Message;

                ApiResponse.ResponseCode = -99;
                return BadRequest(ApiResponse);
            }
        }

        [HttpPost("AddConcession")]
        public async Task<IActionResult> AddConcession(admChargeExemption p)
        {
            try
            {
                p.DateCreated = DateTime.Now;
                p.Status = _configuration["Statuses:ActiveStatus"];
                p.SubjectToTax = p.SubjectToTax == "true" ? "Y" : null;
                p.Exempted = p.Exempted == "true" ? "Y" : "N";
               //  p.TimeBased = "N";
                _ApplicationDbContext.admChargeExemptions.Add(p);
                int _response = await _ApplicationDbContext.SaveChanges(p.UserId);
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

        [HttpPost("UpdateConcession")]
        public async Task<IActionResult> UpdateConcession(admChargeExemption p)
        {
            try
            {
                var get = _ApplicationDbContext.admChargeExemptions.FirstOrDefault(c => c.ItbId == p.ItbId);
                 if(get != null)
                 {
                     get.ChargeType = p.ChargeType;
                     get.ChargeBasis = p.ChargeBasis;
                     get.ChargeValue = p.ChargeValue;
                     get.CurrencyIso = p.CurrencyIso;
                     get.Exempted = p.Exempted;
                     get.Narration = p.Narration;
                     get.ChargeAcctNo = p.ChargeAcctNo;
                     get.ChargeAcctType = p.ChargeAcctType;
                     get.MinimumCharge = p.MinimumCharge;
                     get.MaximumCharge = p.MaximumCharge;
                     get.MinimumChargeCurr = p.MinimumChargeCurr;
                     get.MaximumChargeCurr = p.MaximumChargeCurr;
                     get.ChargeFloor = p.ChargeFloor;
                     get.ChargeFloorCurr = p.ChargeFloorCurr;
                     get.AcctNumber = p.AcctNumber;
                     get.AcctType = p.AcctType;
                     get.CustomerNo = p.CustomerNo;


                 }
                _ApplicationDbContext.admChargeExemptions.Update(get);
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
                ApiResponse.ResponseMessage = ex.InnerException.Message ?? ex.Message;

                ApiResponse.ResponseCode = -99;
                return BadRequest(ApiResponse);
            }

        }
    }

}