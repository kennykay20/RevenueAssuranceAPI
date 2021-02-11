using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols;
using RevAssuranceApi.AppSettings;
using RevAssuranceApi.RevenueAssurance.Repository.DapperDAL;
using RevAssuranceWebAPi.AnythingGood.DATA.Models;

namespace RevAssuranceApi.OperationImplemention
{
    public class ApprovalValidation
    {
         IConfiguration _configuration;
        IDbConnection db = null;
         AppSettingsPath AppSettingsPath ;
        public ApprovalValidation( IConfiguration configuration) 
        {
            _configuration = configuration;
             AppSettingsPath = new AppSettingsPath(_configuration);
            db = new SqlConnection(AppSettingsPath.GetDefaultCon());
        }
        
        public async Task<admUserLimit> ValidateLimit(int UserItbId)
        {
            var rtn = new DapperDATAImplementation<admUserLimit>();

            string script = "Select * from admUserLimit";
            
            var record = await rtn.LoadSingle(script, db);
            // string scripts = $"select CreditLimit, DebitLimit, GLDebitLimit, GLCreditLimit from admUserLimits where UserItbId = {UserItbId}";
            // var record = (await db.QueryAsync<admUserLimit>(sql: scripts, commandType: CommandType.Text )).FirstOrDefault();
            
            return record;
        }

        public async Task<admUserLimit> ValidateLimitWithCurrency(int UserItbId, string Currency)
        {
            // string scripts = $"select CreditLimit, DebitLimit ,GLDebitLimit ,GLCreditLimit from admUserLimits where UserItbId  = {UserItbId} and CurrencyIso = '{Currency}'";
            // var record = (await db.QueryAsync<admUserLimit>(sql: scripts,
            //    commandType: CommandType.Text
            //    )).FirstOrDefault();

            // return record;
            var rtn = new DapperDATAImplementation<admUserLimit>();

            string script = $"select CreditLimit, DebitLimit ,GLDebitLimit ,GLCreditLimit from admUserLimits where UserItbId  = {UserItbId} and CurrencyIso = '{Currency}'";
            
            var record = await rtn.LoadSingle(script, db);
            return record;
        }

        public ValidationTransLimit ValidateAmountLimit(CbsTransaction cbs, admUserLimit admUserLimit)
        {
            ValidationTransLimit ValidationTransLimit = new ValidationTransLimit();
           if (cbs.DrAcctType == "GL")
            {
                if(cbs.Amount > admUserLimit.GLDebitLimit)
                {
                    ValidationTransLimit.GLDebitLimit = true;
                }
            }
            if (cbs.DrAcctType != "GL")
            {
                if (cbs.Amount > admUserLimit.DebitLimit)
                {
                    ValidationTransLimit.GLDebitLimit = true;
                }
            }
            if (cbs.CrAcctType == "GL")
            {
                if (cbs.Amount > admUserLimit.GLCreditLimit)
                {
                    ValidationTransLimit.GLDebitLimit = true;
                }
            }
            if (cbs.CrAcctType != "GL")
            {
                if (cbs.Amount > admUserLimit.CreditLimit)
                {
                    ValidationTransLimit.GLDebitLimit = true;
                }
            }

            return ValidationTransLimit;
        }

        public ValidationTransLimit ValidateAmountLimitCoreBnking(CbsTransaction cbs, UserLimit UserLimit)
        {
            ValidationTransLimit ValidationTransLimit = new ValidationTransLimit();
            if (cbs.DrAcctType == "GL")
            {
                if (cbs.Amount > UserLimit.GLDrLimit)
                {
                    ValidationTransLimit.GLDebitLimit = true;
                }
            }
            if (cbs.DrAcctType != "GL")
            {
                if (cbs.Amount > UserLimit.DepDrLimit)
                {
                    ValidationTransLimit.GLDebitLimit = true;
                }
            }
            if (cbs.CrAcctType == "GL")
            {
                if (cbs.Amount > UserLimit.GLCrLimit)
                {
                    ValidationTransLimit.GLDebitLimit = true;
                }
            }
            if (cbs.CrAcctType != "GL")
            {
                if (cbs.Amount > UserLimit.DepCrLimit)
                {
                    ValidationTransLimit.GLDebitLimit = true;
                }
            }

            return ValidationTransLimit;
        }


    }
}