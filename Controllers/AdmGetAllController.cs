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
using RevAssuranceApi.OperationImplemention;
using RevAssuranceApi.Response;
using RevAssuranceApi.RevenueAssurance.DATA.Models;
using RevAssuranceApi.RevenueAssurance.Repository.Interface;
using RevAssuranceWebAPi.AnythingGood.DATA.Models;



namespace RevAssuranceApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [EnableCors("CorsPolicy")]
    //[Authorize]
    public class AdmGetAllController : ControllerBase
    {
         IConfiguration _configuration;
        ApiResponse ApiResponse = new ApiResponse();
        TokenGenerator TokenGenerator;  
        AppSettingsPath AppSettingsPath ;
         IDbConnection db = null;
         ApplicationDbContext   _ApplicationDbContext;
         ColCollateralTypeImplementation _ColCollateralTypeImplementation;
          IRepository<admTemplate> _repoadmTemplate;
          IRepository<admIssuranceCoverType> _repoadmIssuranceCoverType;
          IRepository<admCurrency> _repoadmCurrency;
          IRepository<admIndustrySecMapg> _repoadmIndustrySecMapg;
         IRepository<admCouterChqReason> _repoadmCouterChqReason;
         IRepository<admBankBranch> _repoadmBankBranch;
         IRepository<admChequeProduct> _repoadmChequeProduct;
         IRepository<admService> _repoadmService;
        public AdmGetAllController( 
                                        IConfiguration configuration,
                                        ApplicationDbContext   ApplicationDbContext,
                                        ColCollateralTypeImplementation ColCollateralTypeImplementation,
                                        IRepository<admTemplate> repoadmTemplate,
                                        IRepository<admIssuranceCoverType> repoadmIssuranceCoverType,
                                        IRepository<admCurrency> repoadmCurrency,
                                        IRepository<admIndustrySecMapg> repoadmIndustrySecMapg,
                                        IRepository<admCouterChqReason> repoadmCouterChqReason,
                                        IRepository<admBankBranch> repoadmBankBranch,
                                        IRepository<admChequeProduct> repoadmChequeProduct,
                                        IRepository<admService> repoadmService
) 
        {

           _configuration = configuration;
           AppSettingsPath = new AppSettingsPath(_configuration);
           TokenGenerator = new TokenGenerator(_configuration);
           db = new SqlConnection(AppSettingsPath.GetDefaultCon());
           _ApplicationDbContext =  ApplicationDbContext;
           _ColCollateralTypeImplementation = ColCollateralTypeImplementation;
           _repoadmTemplate = repoadmTemplate;
           _repoadmIssuranceCoverType = repoadmIssuranceCoverType;
           _repoadmCurrency = repoadmCurrency;
           _repoadmIndustrySecMapg = repoadmIndustrySecMapg;
           _repoadmCouterChqReason = repoadmCouterChqReason;
           _repoadmBankBranch = repoadmBankBranch;
           _repoadmChequeProduct = repoadmChequeProduct;
           _repoadmService = repoadmService;
        }

        [HttpPost("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var colType  = await _ColCollateralTypeImplementation.GetAll();
                var temp = await _repoadmTemplate.GetManyAsync(c => c.ItbId > 0);

                var reqReason = await _repoadmCouterChqReason.GetManyAsync(c=> c.ItbId > 0);

                var IssuranceCoverType = await _repoadmIssuranceCoverType.GetManyAsync(c=> c.ItbId > 0);

                var branch = await _repoadmBankBranch.GetManyAsync(c=> c.Itbid > 0);
                var sectors = await _repoadmIndustrySecMapg.GetManyAsync(c=> c.Status == "Active");
                var currencies = await _repoadmCurrency.GetManyAsync(c=> c.IsoCode != null);
                var chqProduct = await _repoadmChequeProduct.GetManyAsync(c => c.ItbId > 0);
                var admService = await _repoadmService.GetManyAsync(c => c.ServiceId> 0);
                
                
                return Ok( new {
                    colType = colType,
                    temp = temp,
                    IssuranceCoverType = IssuranceCoverType,
                    currencies = currencies,
                    sectors = sectors,
                    reqReason = reqReason,
                    branch = branch,
                    chqProduct = chqProduct,
                    admService = admService
                });
                
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