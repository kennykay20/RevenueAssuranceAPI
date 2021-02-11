using System.ComponentModel;
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
using System.Web;
using anythingGoodApi.AnythingGood.DATA.Models;
using RevAssuranceApi.AppSettings;

using RevAssuranceApi.TokenGen;
using Dapper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RevAssuranceApi.Helper;
using RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO;
using RevAssuranceWebAPi.AnythingGood.DATA.Models;
using RevAssuranceApi.RevenueAssurance.DATA.Models;
using RevAssuranceApi.RevenueAssurance.Repository.DapperDAL;
using RevAssuranceApi.RevenueAssurance.Repository.Interface;
using RevAssuranceApi.WebServices;
using RevAssuranceApi.Response;
using Newtonsoft.Json;
using System.Net.Http;

namespace RevAssuranceApi.OperationImplemention
{
    public class LoginImplementation
    {
        IConfiguration _configuration;
        ApiResponse ApiResponse = new ApiResponse();
        AppSettingsPath AppSettingsPath;
        IDbConnection db = null;
        DeserializeSerialize<LoginReturnProperty> _DeserializeSerialize;
        IRepository<admLicenseSetUp> _repositoryLicense;
        IRepository<admClientProfile> _repoClientProfile;
        IRepository<admUserProfile> _repoadmUserProfile;
        IRepository<admRole> _repoadmRole;
        IRepository<admBankBranch> _repoadmBankBranch;
        ApplicationDbContext _ApplicationDbContext;
        IRepository<admUserLogin> _repoadmUserLogin;
        HeaderLogin _HeaderLogin;
        WebServiceCaller _clientcaller;
        LogManager _LogManager;
        IRepository<admBankServiceSetup> _repoadmBankServiceSetup;
        Formatter _Formatter = new Formatter();
        FunctionApiSetUpImplementation _FunctionApiSetUpImplementation;
        public LoginImplementation(
                                          IConfiguration configuration,
                                          DeserializeSerialize<LoginReturnProperty> DeserializeSerialize,
                                          IRepository<admLicenseSetUp> repositoryLicense,
                                          IRepository<admClientProfile> repoClientProfile,
                                          IRepository<admUserProfile> repoadmUserProfile,
                                          ApplicationDbContext ApplicationDbContext,
                                          HeaderLogin HeaderLogin,
                                          WebServiceCaller clientcaller,
                                          IRepository<admUserLogin> repoadmUserLogin,
                                          IRepository<admRole> repoadmRole,
                                          IRepository<admBankBranch> repoadmBankBranch,
                                          LogManager LogManager,
                                          IRepository<admBankServiceSetup> repoadmBankServiceSetup,
                                          FunctionApiSetUpImplementation FunctionApiSetUpImplementation
                                          )
        {

            _configuration = configuration;
            _DeserializeSerialize = DeserializeSerialize;
            _repositoryLicense = repositoryLicense;
            _repoClientProfile = repoClientProfile;
            _repoadmUserProfile = repoadmUserProfile;
            _ApplicationDbContext = ApplicationDbContext;
            _HeaderLogin = HeaderLogin;
            _clientcaller = clientcaller;
            _repoadmUserLogin = repoadmUserLogin;
            AppSettingsPath = new AppSettingsPath(_configuration);
            db = new SqlConnection(AppSettingsPath.GetDefaultCon());
            _repoadmRole = repoadmRole;
            _repoadmBankBranch = repoadmBankBranch;
            _LogManager = LogManager;
            _repoadmBankServiceSetup = repoadmBankServiceSetup;
            _FunctionApiSetUpImplementation = FunctionApiSetUpImplementation;
        }

        public async Task<LoginReturnProperty> AutheticateUser(string Username, string Pwd)
        {
            try
            {
                var LoginReturnProperty = new LoginReturnProperty();
                var clientProfile = await _repoClientProfile.GetAsync(null);
                var License = await getLicense(clientProfile);
                if (License.ErrorCode != 0)
                    return License;
                var user = await ChechExsist(Username);
                if (user == null)
                {

                    License.ErrorCode = -2;
                    License.ErrorMessage = "Incorrect UserName / Password ";
                    return License;

                }

                if (user.UseCbsAuth == null || user.UseCbsAuth == false)
                {

                    var pwdUser = await PasswordUser(user, Pwd, clientProfile);
                    LoginReturnProperty = pwdUser;

                }

                if (user.UseCbsAuth == true)
                {

                    var coreBankingUser = await CoreBankingUser(user, Pwd, clientProfile);
                    LoginReturnProperty = coreBankingUser;

                }
                if (LoginReturnProperty.ErrorCode == 0)
                {
                    var values = await _HeaderLogin.FetchStartOfDayCoreBanking();

                    LoginReturnProperty.BankingDate = values.date;
                    LoginReturnProperty.Status = values.status;
                }
                return LoginReturnProperty;

            }
            catch (Exception ex)
            {
                var LoginReturnProperty = new LoginReturnProperty();
                LoginReturnProperty.ErrorCode = -99;
                LoginReturnProperty.ErrorMessage = "Error Occured, Try later";

                var exM = ex != null ? ex.InnerException.Message : ex.Message;

                _LogManager.SaveLog($"Auth CBS UserName {Username} Error:  {exM}");

                return LoginReturnProperty;
            }   
        }
        private async Task<LoginReturnProperty> CoreBankingUser(admUserProfile admUserProfile, string Pwd, admClientProfile cp)
        {
            short comp = 0;
            var returnProp = new LoginReturnProperty();

            string UserName = admUserProfile.LoginId.Trim().ToUpper();
            string pass = Pwd.Length > 8 ? Pwd.ToUpper().Substring(0, 8) : Pwd.ToUpper();

             var get = await _FunctionApiSetUpImplementation.GetConnectDetails("AuthenticateUser");

            var postedvalues = new
            {
                username = UserName,
                password = pass,
                ConnectionStringId = get.ConnectionId
            };

            var bankServ = await _repoadmBankServiceSetup.GetAsync(c => c.Itbid == get.ConnectionId);
            var json = JsonConvert.SerializeObject(postedvalues);
            var postdata = new StringContent(json, Encoding.UTF8, "application/json");

            var client = new HttpClient();
            var url = bankServ.WebServiceUrl.Trim() + "/api/Services/AuthenticateUser";

            _LogManager.SaveLog($"Auth CBS UserName {postedvalues.username} Start");

            var response = await client.PostAsync(url, postdata);

            string result = response.Content.ReadAsStringAsync().Result;

             string paramForUserAuth = JsonConvert.SerializeObject(result);
            _LogManager.SaveLog($"Auth CBS UserName {postedvalues.username} response:  {result}");

            var Response = JsonConvert.DeserializeObject<UserCoreBannkingDetailsDTO>(result);

            /*
            All the below property is to hard code to test
            Response.nErrorCode  =  0;
            Response.sErrorText  = "Active" ; 
            Response.EmployeeId  = 363; 
            Response.UserName  = "SAIJOOF"; 
            Response.FullName  = "SAINABOU JOOF";; 
            Response.UserStatus  = "Active"; 
            Response.BranchNo  = "6"; 
            Response.Email  = "email@"; 
            Response.LoginId  = "SAIJOOF"; 
            */
            if (Response.nErrorCode != null)
            {
                if (Response != null)
                {
                    if (Response.FullName != null)
                    {
                        #region When Response from Ethix is Successfull 

                        returnProp.ErrorCode = 0;
                        returnProp.ErrorMessage = "Login Successful";

                        returnProp.ErrorCode = 0;
                        returnProp.EnforcePassChange = admUserProfile.EnforcePswdChange;

                        returnProp.RoleId = admUserProfile.RoleId;

                        var getRole = await _repoadmRole.GetAsync(c => c.RoleId == returnProp.RoleId);

                        returnProp.RoleName = getRole.RoleName;

                        returnProp.FullName = admUserProfile.FullName.ToUpper();
                        returnProp.UserId = admUserProfile.UserId;
                        returnProp.branchNo = admUserProfile.BranchNo;
                        var getBra = await _repoadmBankBranch.GetAsync(c => c.BranchNo == returnProp.branchNo);
                        returnProp.BranchName = getBra.BranchName;

                        returnProp.deptId = admUserProfile.DeptId;
                        var data = await _repoadmUserLogin.GetManyAsync(p => p.UserId == admUserProfile.UserId);
                        returnProp.LastLoginDate = string.Format("{0:dd/MM/yyyy HH:mm:ss}", data.Max(o => o.loginDate)) == string.Empty ?
                            string.Format("{0:dd-MM-yyyy HH:mm:ss}", DateTime.Now) : string.Format("{0:dd-MM-yyyy HH:mm:ss}", data.Max(o => o.loginDate));

                        comp = 1;
                        admUserProfile.lockcount = 0;
                        admUserProfile.loginstatus = 0;
                        admUserProfile.logincount = 0;

                        admUserProfile.BranchNo = Response.BranchNo != null ? _Formatter.ValIntergers(Response.BranchNo) : (int?)null;
                        admUserProfile.EmailAddress = Response.Email;
                        admUserProfile.FullName = Response.FullName ; 
                        admUserProfile.Status =   Response.UserStatus;
                        _repoadmUserProfile.Update(admUserProfile);

                        var updateUser =  await _ApplicationDbContext.SaveChanges(admUserProfile.UserId);


                        #endregion
                    }
                    else
                    {
                        #region When Response from Ethix is UnSuccessfull
                        if (admUserProfile.logincount >= cp.LoginCount)
                        {
                            admUserProfile.lockcount = 1;
                            _repoadmUserProfile.Update(admUserProfile);
                            await _ApplicationDbContext.SaveChanges(admUserProfile.UserId);

                            returnProp.ErrorCode = -1;
                            returnProp.ErrorMessage = string.Format("User Locked. Contact administrator");

                            return returnProp;
                        }
                        if (admUserProfile.logincount < cp.LoginCount)
                        {
                            admUserProfile.logincount = Convert.ToInt16(admUserProfile.logincount + 1);
                            _repoadmUserProfile.Update(admUserProfile);
                            await _ApplicationDbContext.SaveChanges(admUserProfile.UserId);
                            returnProp.ErrorCode = -1;
                            returnProp.ErrorMessage = $"Invalid UserName/Password. Tries Count ( { admUserProfile.logincount } / { cp.LoginCount } )";
                        }

                        #endregion
                    }
                }
                else
                {
                    #region When Response from Ethix is UnSuccessfull
                    if (admUserProfile.logincount >= cp.LoginCount)
                    {
                        admUserProfile.lockcount = 1;
                        _repoadmUserProfile.Update(admUserProfile);
                        await _ApplicationDbContext.SaveChanges(admUserProfile.UserId);
                        returnProp.ErrorCode = -1;

                        returnProp.ErrorCode = -1;
                        returnProp.ErrorMessage = string.Format("User Locked. Contact administrator");

                        return returnProp;
                    }
                    if (admUserProfile.logincount < cp.LoginCount)
                    {

                        admUserProfile.logincount = Convert.ToInt16(admUserProfile.logincount + 1);
                        _repoadmUserProfile.Update(admUserProfile);
                        await _ApplicationDbContext.SaveChanges(admUserProfile.UserId);
                        returnProp.ErrorCode = -1;
                        returnProp.ErrorMessage = $"Invalid UserName/Password. Tries Count ( { admUserProfile.logincount } / { cp.LoginCount } )";

                    }

                    #endregion
                }

            }
            else
            {
                #region No Response from Autheticate API Call
                if (admUserProfile.logincount >= cp.LoginCount)
                {
                    admUserProfile.lockcount = 1;
                    _repoadmUserProfile.Update(admUserProfile);
                    await _ApplicationDbContext.SaveChanges(admUserProfile.UserId);
                    returnProp.ErrorCode = -1;

                    returnProp.ErrorCode = -1;
                    returnProp.ErrorMessage = string.Format("User Locked. Contact administrator");

                    return returnProp;
                }
                if (admUserProfile.logincount < cp.LoginCount)
                {

                    admUserProfile.logincount = Convert.ToInt16(admUserProfile.logincount + 1);
                    _repoadmUserProfile.Update(admUserProfile);
                    await _ApplicationDbContext.SaveChanges(admUserProfile.UserId);


                    returnProp.ErrorCode = -1;
                    returnProp.ErrorMessage = "Invalid Login Id/Password.Enter Password (" + admUserProfile.logincount + "/" + cp.LoginCount + ")";
                }
            }
            #endregion

            // }
            return returnProp;
        }

        private async Task<LoginReturnProperty> PasswordUser(admUserProfile admUserProfile, string Pwd, admClientProfile cp)
        {
            short comp = 0;
            var returnProp = new LoginReturnProperty();
            #region Use RevAssurance Custom Login
            #region Check for If User is Locked Before Attempting to Login

            if (admUserProfile.logincount >= cp.LoginCount)
            {
                admUserProfile.lockcount = 1;
                _repoadmUserProfile.Update(admUserProfile);
                await _ApplicationDbContext.SaveChanges(admUserProfile.UserId);
                returnProp.ErrorCode = -1;

                returnProp.ErrorCode = -1;
                returnProp.ErrorMessage = string.Format("User Locked. Contact administrator");

                return returnProp;
            }
            #endregion

            #region Check for Enforce Password Change 
            if (admUserProfile.EnforcePswdChange == "Y")
            {
                returnProp.UserId = admUserProfile.UserId;



                returnProp.ErrorCode = -5;
                returnProp.ErrorMessage = string.Format("Enforce Change Password");

                return returnProp;
            }
            #endregion

            string compare = Cryptors.Encrypt(Pwd);
            var com = await _repoadmUserProfile.GetAsync(i => i.LoginId.Trim().ToUpper().Equals(admUserProfile.LoginId.Trim().ToUpper(), StringComparison.InvariantCultureIgnoreCase) && i.Password == compare);
            if (com != null)
            {


                returnProp.ErrorCode = 0;
                returnProp.ErrorMessage = "Login Successfull";

                returnProp.ErrorCode = 0;
                returnProp.EnforcePassChange = admUserProfile.EnforcePswdChange;
                returnProp.RoleId = admUserProfile.RoleId;
                returnProp.FullName = admUserProfile.FullName.ToUpper();
                returnProp.UserId = admUserProfile.UserId;
                returnProp.branchNo = admUserProfile.BranchNo;

                var getBra = await _repoadmBankBranch.GetAsync(c => c.BranchNo == returnProp.branchNo);
                returnProp.BranchName = getBra.BranchName;

                var getRole = await _repoadmRole.GetAsync(c => c.RoleId == returnProp.RoleId);

                returnProp.RoleName = getRole.RoleName;

                returnProp.deptId = admUserProfile.DeptId;
                var data = await _repoadmUserLogin.GetManyAsync(p => p.UserId == admUserProfile.UserId);
                returnProp.LastLoginDate = string.Format("{0:dd/MM/yyyy HH:mm:ss}", data.Max(o => o.loginDate)) == string.Empty ?
                    string.Format("{0:dd-MM-yyyy HH:mm:ss}", DateTime.Now) : string.Format("{0:dd-MM-yyyy HH:mm:ss}", data.Max(o => o.loginDate));

                comp = 1;
                admUserProfile.lockcount = 0;
                admUserProfile.loginstatus = 0;
                admUserProfile.logincount = 0;
                admUserProfile.EnforcePswdChange = "N";
                _repoadmUserProfile.Update(admUserProfile);
                await _ApplicationDbContext.SaveChanges(admUserProfile.UserId);

            }
            else
            {
                if (admUserProfile.logincount >= cp.LoginCount)
                {
                    admUserProfile.lockcount = 1;
                    _repoadmUserProfile.Update(admUserProfile);
                    await _ApplicationDbContext.SaveChanges(admUserProfile.UserId);
                    returnProp.ErrorCode = -1;

                    returnProp.ErrorCode = -1;
                    returnProp.ErrorMessage = string.Format("User Locked. Contact administrator");

                    return returnProp;
                }
                if (admUserProfile.logincount < cp.LoginCount)
                {

                    admUserProfile.logincount = Convert.ToInt16(admUserProfile.logincount + 1);
                    _repoadmUserProfile.Update(admUserProfile);
                    await _ApplicationDbContext.SaveChanges(admUserProfile.UserId);
                    returnProp.ErrorCode = -1;

                    returnProp.ErrorCode = -1;
                    returnProp.ErrorMessage = $"Invalid UserName/Password. Tries Count ( { admUserProfile.logincount } / { cp.LoginCount } )";
                }
            }

            return returnProp;
            #endregion
        }

        public async Task<LoginReturnProperty> getLicense(admClientProfile clientProfile)
        {
            var retValue = new LoginReturnProperty();
            if (clientProfile != null)
            {
                

               // To generate the license is just as below

                string todadate = Cryptors.EncryptNoKey("2021-10-25");

               //  Below are to  hard code to test
                 //   string todadate = Cryptors.EncryptNoKey("2020-10-25");
               //     clientProfile.LicenceKey = todadate; 
                
               
                string dt = Cryptors.DecryptNoKey(clientProfile.LicenceKey);
                double days = (Convert.ToDateTime(dt) - DateTime.Now).TotalDays;

                retValue.LicenseNumberOfDay = Convert.ToInt32(days);
                if (DateTime.Now > Convert.ToDateTime(dt))
                {
                    retValue.ErrorCode = -1000;
                    retValue.LicenseErrorCode = -1000;
                    string daysExpired = retValue.LicenseNumberOfDay.ToString();
                    string contains = "-";
                    if(daysExpired.Contains(contains)){
                        daysExpired = daysExpired.Replace(contains, string.Empty);
                    }


                    retValue.ErrorMessage = $"License has Expires { daysExpired } Days ago. Please contact Information Engineering Technologies Limited for Renuewal";

                    return retValue;

                }
                else if (Convert.ToInt32(Math.Abs(retValue.LicenseNumberOfDay)) <= 31)
                {

                    retValue.ErrorCode = 0;
                    retValue.LicenseErrorCode = 2000;
                    retValue.ErrorMessage = $"License will be Expiring in { retValue.LicenseNumberOfDay } Days time . Please contact Information Engineering Technologies Limited for Renuewal";

                    return retValue;
                }

                retValue.ErrorCode = 0;
                retValue.ErrorMessage = "Success";

                return retValue;

            }
            retValue.ErrorCode = -99;
            return retValue;
        }

        private async Task<admUserProfile> ChechExsist(string Username)
        {
            var get = await _repoadmUserProfile.GetAsync(c => c.LoginId == Username);
            return get;
        }

        public async Task<List<admMenuControl>> getUserMenu(int? RoleId)
        {
            string script = @"	SELECT	   
                            b.MenuId, 
                            b.MenuName, 
                            b.RouterLink + Convert(varchar, b.MenuId) RouterLink, 
                            b.Icon, 
                            b.IconAddress, 
                            b.IsParent, 
                            b.ParentId,
                            a.CanView,
                            a.CanAdd,
                            a.CanEdit,
                            a.CanAuth,
                            a.CanDelete,
                            a.IsGlobalSupervisor
                            from admRoleAssignment a 
                            join admMenuControl b
                            on a.MenuId = b.MenuId
                            where a.RoleId = @RoleId
                            and a.CanView = 1
                            order by b.MenuName asc".Replace("@RoleId", RoleId.ToString());

            var rtn = new DapperDATAImplementation<admMenuControl>();

            var _response = await rtn.LoadListNoParam(script, db);

            var getSystemAdmin = _response.Where(c => c.MenuId == 1 || c.ParentId == 1).ToList();
            var getOperation = _response.Where(c => c.MenuId == 2 || c.ParentId == 2).ToList();
            var getReport = _response.Where(c => c.MenuId == 3 || c.ParentId == 3).ToList();

            var con = getSystemAdmin.Concat(getOperation).Concat(getReport).ToList();

            return con;
        }


    }
}

