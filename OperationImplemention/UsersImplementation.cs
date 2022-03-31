using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using anythingGoodApi.AnythingGood.DATA;
using RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO;
using RevAssuranceWebAPi.AnythingGood.DATA.Models;
using RevAssuranceApi.RevenueAssurance.DATA.Models;
using RevAssuranceApi.RevenueAssurance.Repository.Interface;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using RevAssuranceApi.AppSettings;
using System.Data;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using RevAssuranceApi.RevenueAssurance.Repository.DapperDAL;

namespace RevAssuranceApi.OperationImplemention
{
    public class UsersImplementation
    {
        IRepository<admUserProfile> _repoadmUserProfile;
        IRepository<admBankBranch> _repoadmBankBranch;
        IRepository<admBankServiceSetup> _repoBankServiceSetUp;
        AppSettingsPath AppSettingsPath;
        IDbConnection db = null;
        IConfiguration _configuration;
        ApplicationDbContext _ApplicationDbContext;
        
 FunctionApiSetUpImplementation _FunctionApiSetUpImplementation; 
        public UsersImplementation(IRepository<admUserProfile> repoadmUserProfile,
                                    ApplicationDbContext ApplicationDbContext,
                                    IRepository<admBankServiceSetup> repoBankServiceSetUp,
                                     IConfiguration configuration,
                                     IRepository<admBankBranch> repoadmBankBranch,
                                    FunctionApiSetUpImplementation FunctionApiSetUpImplementation)
        {
            _configuration = configuration;
            _repoadmUserProfile = repoadmUserProfile;
            _ApplicationDbContext = ApplicationDbContext;
            _repoBankServiceSetUp = repoBankServiceSetUp;
            _repoadmBankBranch = repoadmBankBranch;
            AppSettingsPath = new AppSettingsPath(_configuration);
            db = new SqlConnection(AppSettingsPath.GetDefaultCon());
            _FunctionApiSetUpImplementation = FunctionApiSetUpImplementation;
        }

        public async Task<admUserProfile> GetUser(int? userId)
        {
            try
            {

                var get = await _repoadmUserProfile.GetAsync(c => c.UserId == userId);

                return get;

            }
            catch (Exception ex)
            {
                //  var msg = await _applicationReturnMessage.returnMessage(20008);
                var exM = ex == null ? ex.InnerException.Message : ex.Message;

                return null;

            }


        }


        public async Task<AllActionUser> GetAllUser(int? userId, int? Rejectedby, int? DismissedBy, string RejectionReasons = null,
                                                    int? SupervisorId = null, int? OriginatingBranch = null)
        {
            try
            {

                var CreatedBy = await _repoadmUserProfile.GetAsync(c => c.UserId == userId);
                var rejectedBy = await _repoadmUserProfile.GetAsync(c => c.UserId == Rejectedby);
                var dismissedBy = await _repoadmUserProfile.GetAsync(c => c.UserId == DismissedBy);
                var SupervisedBy = await _repoadmUserProfile.GetAsync(c => c.UserId == SupervisorId);
                var OriginBranch = await _repoadmBankBranch.GetAsync(c => c.BranchNo == OriginatingBranch);

                string reasons = string.Empty;

                if (!string.IsNullOrWhiteSpace(RejectionReasons))
                {
                    var rtn1 = new DapperDATAImplementation<admRejectionReason>();
                    string scr = $"select * from admRejectionReason where ItbId in ({RejectionReasons})";

                    var loadReason = await rtn1.LoadListNoParam(scr, db);

                    if (loadReason != null)
                    {
                        int count = 0;
                        foreach (var b in loadReason)
                        {
                            count += 1;
                            reasons += count + ". " + b.Description + "\n\n";
                        }
                    }
                }

                var all = new AllActionUser
                {
                    CreatedBy = CreatedBy != null ? CreatedBy.FullName : string.Empty,
                    RejectedBy = rejectedBy != null ? rejectedBy.FullName : string.Empty,
                    DismissedBy = dismissedBy != null ? dismissedBy.FullName : string.Empty,
                    Supervisor = SupervisedBy != null ? SupervisedBy.FullName : string.Empty,
                    RejectionReasons = reasons,
                    OriginBranch = OriginBranch != null ? OriginBranch.BranchName : string.Empty,
                };


                return all;

            }
            catch (Exception ex)
            {
                //  var msg = await _applicationReturnMessage.returnMessage(20008);
                var exM = ex == null ? ex.InnerException.Message : ex.Message;

                return null;

            }


        }

        public async Task<admUserProfile> ViewDetails(int UserId)
        {
            try
            {
                var y = await _repoadmUserProfile.GetAsync(p => p.UserId == UserId);
                return y;
            }
            catch (Exception ex)
            {
                var exM = ex;
            }
            return null;
        }

        public async Task<UserCoreBannkingDetailsDTO> GetUserDetailsCore(GetUserDetailsParamCoreBnkingDTO param)
        {
            UserCoreBannkingDetailsDTO oUserCoreBannkingDetailsDTO = new UserCoreBannkingDetailsDTO();
            try
            {
                 
                var bankServ = await _repoBankServiceSetUp.GetAsync(null);
                using (var client = new HttpClient())
                {
                    string webUrl = bankServ.WebServiceUrl.Trim();
                    client.BaseAddress = new Uri(webUrl);

                    GetUserDetailsParamCoreBnkingDTO values = new GetUserDetailsParamCoreBnkingDTO()
                    {
                        loginId = param.loginId,
                        staffId = param.staffId,
                        ConnectionStringId = bankServ.Itbid
                    };

                    var url = webUrl + "/api/Services/ValidateUser";


                    //  var settings = new JsonSerializerSettings();
                    // settings.ContractResolver = new LowercaseContractResolver();
                    var json = JsonConvert.SerializeObject(values);//, Formatting.Indented, settings);


                    var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = client.PostAsync(url, stringContent).Result;

                    //Console.WriteLine(response);

                    var res = await response.Content.ReadAsStringAsync();

                    //var resVal = res;
                    if (response.IsSuccessStatusCode)
                    {
                        oUserCoreBannkingDetailsDTO = JsonConvert.DeserializeObject<UserCoreBannkingDetailsDTO>(res.ToString());
                        return oUserCoreBannkingDetailsDTO;
                    }
                }

            }
            catch (Exception ex)
            {
                var exM = ex;
                // LogManager.SaveLog(ex.Message == null ? ex.InnerException.ToString() : ex.Message.ToString());

            }

            return oUserCoreBannkingDetailsDTO;

        }

    }



}