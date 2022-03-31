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
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

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
        IRepository<TwoFactorCodes> _twoFactorCode;
        EmailImplementation _emailImplementation;
        private readonly IHostingEnvironment _hostingEnvironment;
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
                                          FunctionApiSetUpImplementation FunctionApiSetUpImplementation,
                                          IRepository<TwoFactorCodes> twoFactorCode,
                                          EmailImplementation emailImplementation,
                                          IHostingEnvironment hostingEnvironment
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
            _twoFactorCode = twoFactorCode;
            _emailImplementation = emailImplementation;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<LoginReturnProperty> AuthenticateUser(string Username, string Pwd, string staffId, string loginTy)
        {
            try
            {
                admUserProfile user = null;
                var LoginReturnProperty = new LoginReturnProperty();

                _LogManager.SaveLog("call _repoClientProfile.getAsync with null parameter");
                var clientProfile = await _repoClientProfile.GetAsync(null);

                _LogManager.SaveLog("call getLicense");
                var License = await getLicense(clientProfile);

                _LogManager.SaveLog("connect result for clientoprofile status " + clientProfile.Status);

                _LogManager.SaveLog(" license errorcode result " + License.ErrorCode);
                if (License.ErrorCode != 0)
                    return License;


                if (Username != "")
                {
                    _LogManager.SaveLog("checkUserExit call");
                    user = await checkUserExist(Username);
                    _LogManager.SaveLog("user status " + (user == null ? "null" : user.Status));
                }
                else if (staffId != "")
                {

                    _LogManager.SaveLog("checkStaffExit call");
                    user = await checkStaffExist(staffId);
                    _LogManager.SaveLog("staff status " + (user == null ? "null" : user.Status));
                }



                //_LogManager.SaveLog("User " + user);
                if (user == null)
                {

                    License.ErrorCode = -2;
                    License.ErrorMessage = "User Account Does't Exist in the Database, Please Contact System Administator";
                    return License;

                }

                if (user.loginType != loginTy)
                {
                    var loginDescription = (user.loginType == "App" ? "Application" : (user.loginType == "CBS") ? "CBS Authentication" : (user.loginType == "2Fpin") ? "2 Factor Authentication(Pin+Token)" : (user.loginType == "2Fotp") ? "2 Factor Authentication(Otp)" : "");

                    License.ErrorCode = -99;
                    License.ErrorMessage = "Invalid Login Contact System Administrator";
                    return License;
                }

                if (user.Status.Trim() != "Active")
                {

                    License.ErrorCode = -2;
                    License.ErrorMessage = "Your user record is not active, Please Contact System Administator";
                    return License;

                }

                _LogManager.SaveLog("call user.loginType " + user.loginType);
                if (user.loginType == "App")
                {
                    if (Username != null || Username != "")
                    {

                        _LogManager.SaveLog("call PasswordUser for App ");

                        var pwdUser = await PasswordUser(user, Pwd, clientProfile);

                        //_LogManager.SaveLog(" pwdUser " + pwdUser); 
                        LoginReturnProperty = pwdUser;
                    }

                }

                if (user.loginType == "2Fotp")
                {
                    if (Username != null || Username != "")
                    {

                        _LogManager.SaveLog("call PasswordUser for otp ");

                        var pwdUser = await PasswordUser(user, Pwd, clientProfile);

                        //_LogManager.SaveLog(" pwdUser " + pwdUser); 
                        LoginReturnProperty = pwdUser;
                    }
                }
                if (user.loginType == "CBS")
                {
                    var userNam = Username;
                    var coreBankingUser = await CoreBankingUser(user, Pwd, clientProfile, userNam);
                    LoginReturnProperty = coreBankingUser;

                }


                if (user.loginType == "2Fpin")
                {
                    var userNam = staffId;
                    var coreBankingUser = await CoreBankingUser(user, Pwd, clientProfile, userNam);
                    LoginReturnProperty = coreBankingUser;
                }

                if (LoginReturnProperty.ErrorCode == 0 || LoginReturnProperty.ErrorCode == 9)
                {
                    _LogManager.SaveLog("call FetchStartOfDayCoreBanking ");

                    var values = await _HeaderLogin.FetchStartOfDayCoreBanking();

                    LoginReturnProperty.BankingDate = values.date;
                    LoginReturnProperty.Status = values.status;
                }

                //_LogManager.SaveLog("call FetchStartOfDayCoreBanking "); 

                return LoginReturnProperty;

            }
            catch (Exception ex)
            {
                var LoginReturnProperty = new LoginReturnProperty();
                LoginReturnProperty.ErrorCode = -99;
                LoginReturnProperty.ErrorMessage = "Error Occured, Try later";

                var exM = ex != null ? ex.InnerException.Message : ex.Message;

                _LogManager.SaveLog($"Login Implementation Error:  {exM}");

                _LogManager.SaveLog($"Auth CBS ex trace { ex.StackTrace } ");

                return LoginReturnProperty;
            }
        }
        private async Task<LoginReturnProperty> CoreBankingUser(admUserProfile admUserProfile, string Pwd, admClientProfile cp, string usernames)
        {
            short comp = 0;
            var returnProp = new LoginReturnProperty();

            string UserName = usernames.Trim().ToUpper();
            string pass = Pwd.Length > 8 ? Pwd.ToUpper().Substring(0, 8) : Pwd.ToUpper();
            string loginTy = (admUserProfile.UseCbsAuth == true ? "cbs" : (cp.twoFactorOn == true ? "2f" : ""));

            var get = await _FunctionApiSetUpImplementation.GetConnectDetails("AuthenticateUser");



            var bankServ = await _repoadmBankServiceSetup.GetAsync(c => c.Itbid == get.ConnectionId);

            var postedvalues = new
            {
                username = UserName,
                password = pass,
                ConnectionStringId = get.ConnectionId,
                loginType = admUserProfile.loginType
            };

            var json = JsonConvert.SerializeObject(postedvalues);
            var postdata = new StringContent(json, Encoding.UTF8, "application/json");

            var client = new HttpClient();
            var url = bankServ.WebServiceUrl.Trim() + "/api/Services/AuthenticateUser";


            var response = await client.PostAsync(url, postdata);

            string result = response.Content.ReadAsStringAsync().Result;

            string paramForUserAuth = JsonConvert.SerializeObject(result);

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
                        returnProp.staffId = admUserProfile.StaffId != null ? admUserProfile.StaffId : "";
                        returnProp.userLoginType = admUserProfile.loginType != null ? admUserProfile.loginType : "";
                        var data = await _repoadmUserLogin.GetManyAsync(p => p.UserId == admUserProfile.UserId);
                        returnProp.LastLoginDate = string.Format("{0:dd/MM/yyyy HH:mm:ss}", data.Max(o => o.loginDate)) == string.Empty ?
                            string.Format("{0:dd-MM-yyyy HH:mm:ss}", DateTime.Now) : string.Format("{0:dd-MM-yyyy HH:mm:ss}", data.Max(o => o.loginDate));

                        comp = 1;
                        admUserProfile.lockcount = 0;
                        admUserProfile.loginstatus = 0;
                        admUserProfile.logincount = 0;

                        admUserProfile.BranchNo = Response.BranchNo != null ? _Formatter.ValIntergers(Response.BranchNo) : (int?)null;
                        admUserProfile.EmailAddress = Response.Email;
                        admUserProfile.FullName = Response.FullName;
                        admUserProfile.Status = Response.UserStatus;
                        admUserProfile.StaffId = Response.StaffId;
                        _repoadmUserProfile.Update(admUserProfile);

                        var updateUser = await _ApplicationDbContext.SaveChanges(admUserProfile.UserId);


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
                            returnProp.ErrorMessage = string.Format("Your User Account has been Locked. Please Contact System Administrator");

                            return returnProp;
                        }
                        if (admUserProfile.logincount < cp.LoginCount)
                        {
                            admUserProfile.logincount = Convert.ToInt16(admUserProfile.logincount + 1);
                            _repoadmUserProfile.Update(admUserProfile);
                            await _ApplicationDbContext.SaveChanges(admUserProfile.UserId);
                            returnProp.ErrorCode = -1;
                            returnProp.ErrorMessage = $"Invalid UserName/Password. ( { admUserProfile.logincount } / { cp.LoginCount } ) more attempt";
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
                        returnProp.ErrorMessage = string.Format("Your User Account has been Locked. Please Contact System Administrator");

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
                    returnProp.ErrorMessage = string.Format("Your User Account has been Locked. Please Contact System Administrator");

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
                returnProp.ErrorMessage = string.Format("Your User Account has been Locked. Please Contact System Administrator");

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

            string compare = Cryptors.EncryptNoKey(Pwd);
            var com = await _repoadmUserProfile.GetAsync(i => i.LoginId.Trim().ToUpper().Equals(admUserProfile.LoginId.Trim().ToUpper(), StringComparison.InvariantCultureIgnoreCase) && i.Password == compare);
            if (com != null)
            {



                returnProp.EnforcePassChange = admUserProfile.EnforcePswdChange;
                returnProp.RoleId = admUserProfile.RoleId;
                returnProp.FullName = admUserProfile.FullName.ToUpper();
                returnProp.UserId = admUserProfile.UserId;
                returnProp.branchNo = admUserProfile.BranchNo;
                returnProp.loginType = cp.loginType;
                returnProp.staffId = admUserProfile.StaffId != null ? admUserProfile.StaffId : "";
                returnProp.userLoginType = admUserProfile.loginType != null ? admUserProfile.loginType : "";

                var getBra = await _repoadmBankBranch.GetAsync(c => c.BranchNo == returnProp.branchNo);
                returnProp.BranchName = getBra.BranchName;

                var getRole = await _repoadmRole.GetAsync(c => c.RoleId == returnProp.RoleId);

                returnProp.RoleName = getRole.RoleName;

                returnProp.deptId = admUserProfile.DeptId;
                var data = await _repoadmUserLogin.GetManyAsync(p => p.UserId == admUserProfile.UserId);
                returnProp.LastLoginDate = string.Format("{0:dd/MM/yyyy HH:mm:ss}", data.Max(o => o.loginDate)) == string.Empty ?
                    string.Format("{0:dd-MM-yyyy HH:mm:ss}", DateTime.Now) : string.Format("{0:dd-MM-yyyy HH:mm:ss}", data.Max(o => o.loginDate));


                var OneTimePinCode = "";
                DateTime? expiryOtp = null;

                if (admUserProfile.loginType == "2Fotp")
                {
                    //generate one time code here
                    var returnCode = _emailImplementation.GenerateOneTimePin();

                    //and send code to the user email or phoneNo
                    if (returnCode.ErrorCode == 0)
                    {
                        OneTimePinCode = returnCode.OneTimePinCode;
                        expiryOtp = returnCode.ExpiryDate;

                        //email
                        var respon = await _emailImplementation.SendCodeEmailAsync(admUserProfile.EmailAddress, "One Time Verification Code", " Code = " + OneTimePinCode, "TwoFactorAuthentication.html");
                        if (respon == "Sent")
                        {
                            returnProp.ErrorCode = 9;
                            returnProp.ErrorMessage = "One Time Verification Code Sent";
                        }
                        //_emailImplementation.SendSmsAsync(admUserProfile.MobileNo, " Code = " + OneTimePinCode);
                    }


                }
                else
                {
                    returnProp.ErrorCode = 0;
                    returnProp.ErrorMessage = "Login Successfull";
                }
                comp = 1;
                admUserProfile.lockcount = 0;
                admUserProfile.loginstatus = 0;
                admUserProfile.logincount = 0;
                admUserProfile.EnforcePswdChange = "N";
                admUserProfile.OneTimeCode = OneTimePinCode != "" ? Cryptors.EncryptNoKey(OneTimePinCode) : "";
                admUserProfile.OtpExpiryDateTime = expiryOtp != null ? expiryOtp : null;
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
                    returnProp.ErrorMessage = string.Format("Your User Account has been Locked. Please Contact System Administrator");

                    return returnProp;
                }
                if (admUserProfile.logincount < cp.LoginCount)
                {

                    admUserProfile.logincount = Convert.ToInt16(admUserProfile.logincount + 1);
                    _repoadmUserProfile.Update(admUserProfile);
                    await _ApplicationDbContext.SaveChanges(admUserProfile.UserId);
                    returnProp.ErrorCode = -1;

                    returnProp.ErrorCode = -1;
                    returnProp.ErrorMessage = $"Invalid UserName/Password. ( { admUserProfile.logincount } / { cp.LoginCount } ) more attempt";
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
                    if (daysExpired.Contains(contains))
                    {
                        daysExpired = daysExpired.Replace(contains, string.Empty);
                    }


                    retValue.ErrorMessage = $"License has Expires { daysExpired } Days ago. Please contact Information Engineering Technologies Limited for Renuewal";

                    return retValue;

                }
                else if (Convert.ToInt32(Math.Abs(retValue.LicenseNumberOfDay)) <= 31)
                {

                    retValue.ErrorCode = 0;
                    retValue.LicenseErrorCode = 2000;
                    retValue.ErrorMessage = $"License will be Expiring in { retValue.LicenseNumberOfDay } Days time . Please contact Information Engineering Technologies Limited for Renewal";

                    return retValue;
                }

                retValue.ErrorCode = 0;
                retValue.ErrorMessage = "Success";

                return retValue;

            }
            retValue.ErrorCode = -99;
            return retValue;
        }

        private async Task<admUserProfile> checkUserExist(string Username)
        {
            var get = await _repoadmUserProfile.GetAsync(c => c.LoginId == Username);
            return get;
        }

        private async Task<admUserProfile> checkStaffExist(string staffLogin)
        {
            var get = await _repoadmUserProfile.GetAsync(c => c.StaffId == staffLogin);
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

            var getSystemAdmin = _response.Where(c => c.MenuId == 1 || (c.ParentId == 1 || c.ParentId == 11)).ToList();
            var getOperation = _response.Where(c => c.MenuId == 2 || c.ParentId == 2).ToList();
            var getReport = _response.Where(c => c.MenuId == 3 || c.ParentId == 3).ToList();

            var con = getSystemAdmin.Concat(getOperation).Concat(getReport).ToList();

            return con;
        }

        public async Task<LoginReturnProperty> PasswordReset(string Username, string Pwd)
        {

            var returnProp = new LoginReturnProperty();
            var admUserProfile = await _repoadmUserProfile.GetAsync(x => x.LoginId == Username);
            var clientProfile = await _repoClientProfile.GetAsync(null);



            if (admUserProfile != null)
            {
                if (admUserProfile.logincount >= clientProfile.LoginCount)
                {
                    admUserProfile.lockcount = 1;
                    _repoadmUserProfile.Update(admUserProfile);
                    await _ApplicationDbContext.SaveChanges(admUserProfile.UserId);


                    returnProp.ErrorCode = -1;
                    returnProp.ErrorMessage = string.Format("Your User Account has been Locked. Please Contact System Administrator");

                    return returnProp;
                }


                if (clientProfile != null)
                {
                    if (clientProfile.MaxiPasswordLength == Pwd.Count())
                    {
                        if (clientProfile.ComplexPassword == "C")
                        {
                            if (Pwd.Count(char.IsUpper) >= clientProfile.Uppercase && Pwd.Count(char.IsDigit) >= clientProfile.NumericNumber && Pwd.Count(c => !char.IsLetterOrDigit(c)) >= clientProfile.SpecialCharacter)
                            {


                                var encryptyPass = Cryptors.EncryptNoKey(Pwd);
                                admUserProfile.Password = encryptyPass;
                                admUserProfile.EnforcePswdChange = "N";
                                await _ApplicationDbContext.SaveChanges(admUserProfile.UserId);


                                returnProp.ErrorCode = 0;
                                returnProp.ErrorMessage = "Password Reset Successfully";
                                return returnProp;
                            }
                            else
                            {
                                returnProp.ErrorCode = -5;
                                return returnProp;
                            }
                        }
                        else
                        {

                            var encryptyPass = Cryptors.EncryptNoKey(Pwd);
                            admUserProfile.Password = encryptyPass;
                            admUserProfile.EnforcePswdChange = "N";
                            await _ApplicationDbContext.SaveChanges(admUserProfile.UserId);

                            returnProp.ErrorCode = 0;
                            returnProp.ErrorMessage = "Password Reset Successfully";
                            return returnProp;
                        }
                    }
                    else
                    {

                        return returnProp;
                    }
                }

            }
            return returnProp;
        }

        public async Task<LoginReturnProperty> CheckPasswordOtp(string passwordOtp, string loginId)
        {
            var returnProp = new LoginReturnProperty();
            var encryOtp = Cryptors.EncryptNoKey(passwordOtp);
            var admUserCheck = await _repoadmUserProfile.GetAsync(x => x.LoginId == loginId && x.OneTimeCode == encryOtp && x.OtpExpiryDateTime > DateTime.Now);

            if (admUserCheck != null)
            {
                returnProp.ErrorCode = 0;
                returnProp.ErrorMessage = "Success";
            }
            else
            {
                returnProp.ErrorCode = -1;
                returnProp.ErrorMessage = "Error";
            }

            return returnProp;
        }


    }


}

