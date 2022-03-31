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
    public class ApplicationReturnMessage
    {
         IConfiguration _configuration;
        IDbConnection db = null;
         AppSettingsPath AppSettingsPath ;
         public ApplicationReturnMessage( IConfiguration configuration){
            _configuration = configuration;
            AppSettingsPath = new AppSettingsPath(_configuration);
            db = new SqlConnection(AppSettingsPath.GetDefaultCon());
         }

        public async Task<admErrorMsg> returnMessage(int MessageCode)
        {
            // string scripts = $"Select Message from admAppReturnMsg where MessageCode = {MessageCode}";
            // var record = (await db.QueryAsync<admAppReturnMsg>(sql: scripts,
            //    commandType: CommandType.Text
            //    )).FirstOrDefault();

            // return record;

             var rtn = new DapperDATAImplementation<admErrorMsg>();

            string scripts = $"Select Message from admErrorMsg where ErrorId = {MessageCode}";
            
            var record = await rtn.LoadSingle(scripts, db);
            // string scripts = $"select CreditLimit, DebitLimit, GLDebitLimit, GLCreditLimit from admUserLimits where UserItbId = {UserItbId}";
            // var record = (await db.QueryAsync<admUserLimit>(sql: scripts, commandType: CommandType.Text )).FirstOrDefault();
            
            return record;
        }

    }
}