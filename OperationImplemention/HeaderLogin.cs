using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO;
using RevAssuranceApi.RevenueAssurance.Repository.Interface;
using RevAssuranceWebAPi.AnythingGood.DATA.Models;

namespace RevAssuranceApi.OperationImplemention
{
     public class HeaderLogin
    {
        private StartOfDayResponse oStartOfDayResponse;
        private readonly IRepository<admUserProfile>  _repoUserProfile;
        private readonly IRepository<admBankServiceSetup> _repoBankServiceSetUp;
        private readonly IRepository<admRole>  _repoRoles;
        private readonly  IRepository<admBroadCast> _repoBroadCast;
         
        FunctionApiSetUpImplementation _FunctionApiSetUpImplementation;
        public HeaderLogin(IRepository<admUserProfile>  repoUserProfile, 
                        IRepository<admBankServiceSetup> repoBankServiceSetUp,
                        IRepository<admRole>  repoRoles,IRepository<admBroadCast> repoBroadCast,
                        FunctionApiSetUpImplementation FunctionApiSetUpImplementation)
        {
            oStartOfDayResponse = new StartOfDayResponse();
            _repoUserProfile = repoUserProfile;
            _repoBankServiceSetUp = repoBankServiceSetUp;
            _repoRoles = repoRoles;
            _repoBroadCast = repoBroadCast;
            _FunctionApiSetUpImplementation = FunctionApiSetUpImplementation;
        }

        // public async Task<CountResponse> FetchTransactionCount(int userId, int branchCode, int deptId)
        // {
        //     DapperConnection odapper = new DapperConnection();
        //     var resp = odapper.TransactionHeaderCount(userId, deptId, branchCode);
        //     resp.Total = resp.UnAuthorized + resp.UnPosted + resp.Verified + resp.AmortVerified;
        //     return resp;
        // }
        public async Task<BroadCastResponse> FetchBroadCast()
        {
            var res = new BroadCastResponse();
            try
            {
                var value = await _repoBroadCast.GetAsync(o => o.Status == "Active");

                if (value != null)
                {
                    res.nErrorCode = 0;
                    res.Message = value.Message;
                    return res;

                }
                else
                {
                    res.nErrorCode = -1;
                    return res;

                }

            }
            catch (Exception ex)
            {
                var exM  =  ex == null ? ex.InnerException.Message : ex.Message;
                res.nErrorCode = -1;
                return res;

            }
        }

        public async Task<string> GetFullName(int ItbId)
        {
            var get = await _repoUserProfile.GetAsync(c=> c.UserId == ItbId);

             return get.FullName;
        }

        public async Task<string> GetBranchName(int ItbId)
        {

            var user = await _repoUserProfile.GetAsync(o => o.UserId == ItbId);
            if (user != null)
            {
                return user.BranchName;
            }
            return null;
        }

        public async Task<string> GetRole(int ItbId)
        {
            try
            {
                var get = await _repoRoles.GetAsync(c=> c.RoleId == ItbId);
                
                return  get.RoleName;
            }
            catch (Exception ex)
            {

                var exM = ex;
            }

            return null;
        }
      

        public async Task<StartOfDay> FetchStartOfDayCoreBanking()
        {
            DateTime dt = new DateTime();
            var rtv = new StartOfDay();
            try
            {

                var http = new HttpClient();
                  var get = await _FunctionApiSetUpImplementation.GetConnectDetails("StartOfDay");
      

                var serviceSetUp = await  _repoBankServiceSetUp.GetAsync(o=> o.Itbid  == get.ConnectionId);

                var url = $"{serviceSetUp.WebServiceUrl}/api/Services/StartOfDay?ConnectionStringId={get.ConnectionId}" ;

                using (HttpClient httpClient = new HttpClient())
                {
                    var response = httpClient.GetAsync(url).GetAwaiter().GetResult();
                    var result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    oStartOfDayResponse = JsonConvert.DeserializeObject<StartOfDayResponse>(result);
                }

                if (oStartOfDayResponse != null)
                {
                    if (!string.IsNullOrEmpty(oStartOfDayResponse.BankingDate))
                    {
                        if (DateTime.TryParse(oStartOfDayResponse.BankingDate, out dt))
                        {

                            rtv.date = string.Format("{0:dd-MMM-yyyy }", dt);
                            rtv.status = "ONLINE";
                            return rtv;
                        }


                    }

                }

                rtv.date = string.Format("{0:dd-MMM-yyyy }", DateTime.Now);
                rtv.status = "OFFLINE";
                return rtv;


            }
            catch (Exception ex)
            {
                var exM = ex;
                rtv.date = string.Format("{0:dd-MMM-yyyy }", DateTime.Now);
                rtv.status = "OFFLINE";

            }
            return rtv;
        }
    
    
    }
}