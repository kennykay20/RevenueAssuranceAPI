

using System;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using RevAssuranceApi.AppSettings;
using RevAssuranceApi.Helper;
using RevAssuranceApi.Response;
using RevAssuranceApi.RevenueAssurance.DATA.Models;
using RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO;
using RevAssuranceApi.RevenueAssurance.Repository.Interface;
using RevAssuranceWebAPi.AnythingGood.DATA.Models;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace RevAssuranceApi.OperationImplemention
{
    public class EmailImplementation
    {
        IConfiguration _configuration;
        LogManager _LogManager;
        ApiResponse ApiResponse = new ApiResponse();
        AppSettingsPath AppSettingsPath;
        IDbConnection db = null;
        DeserializeSerialize<LoginReturnProperty> _DeserializeSerialize;
        IRepository<admLicenseSetUp> _repositoryLicense;
        IRepository<admClientProfile> _repoClientProfile;
        IRepository<admUserProfile> _repoadmUserProfile;
        ApplicationDbContext _ApplicationDbContext;
        private readonly IHostingEnvironment _hostingEnvironment;
        IRepository<TwoFactorCodes> _twoFactorCode;
        public EmailImplementation(IConfiguration configuration,
                                    LogManager LogManager,
                                    DeserializeSerialize<LoginReturnProperty> DeserializeSerialize,
                                    IRepository<admLicenseSetUp> repositoryLicense,
                                    IRepository<admClientProfile> repoClientProfile,
                                    IRepository<admUserProfile> repoadmUserProfile,
                                    IRepository<TwoFactorCodes> twoFactorCode,
                                    ApplicationDbContext ApplicationDbContext,
                                    IHostingEnvironment hostingEnvironment
                                    )
        {
            _configuration = configuration;
            _LogManager = LogManager;
            _DeserializeSerialize = DeserializeSerialize;
            _repositoryLicense = repositoryLicense;
            _repoClientProfile = repoClientProfile;
            _repoadmUserProfile = repoadmUserProfile;
            _twoFactorCode = twoFactorCode;
            _ApplicationDbContext = ApplicationDbContext;
            _hostingEnvironment = hostingEnvironment;
        }


        public LoginReturnProperty GenerateOneTimePin()
        {
            var returnProp = new LoginReturnProperty();

            try
            {
                
                string timetoelapse = _configuration["ElapseTimeMinutes"].ToString();;
                Random rdn = new Random();
                int randomValue = rdn.Next(111111, 999999);
                string code = randomValue.ToString();
                    
                returnProp.ErrorCode = 0;
                returnProp.ErrorMessage = "OneTimePinCode Generated";
                returnProp.OneTimePinCode = code;
                returnProp.ExpiryDate = DateTime.Now.AddMinutes(Convert.ToDouble(timetoelapse));
                returnProp.CreatedDate = DateTime.Now;
                _LogManager.SaveLog("OneTimePinCode Generated");
                return returnProp;
            }
            catch(Exception ex)
            {

            }
            
            return returnProp;   
        }

        //One Time Verification Code 
        public async Task<string> SendCodeEmailAsync(string email, string subject, string message, string template)
        {
            string strGmail = "gmail.com";
            string resp = "";
            var strMessageBody = BuildEmailBody(message, template, subject);
            if(!string.IsNullOrEmpty(strMessageBody))
            {
                if(email.Contains(strGmail)){
                    resp = await SendEmailByGmailAsync(email, subject, strMessageBody);
                    if(resp == "Sent")
                    {
                        return resp;
                    }
                }
                else{
                    resp = await SendEmailAsync(subject, strMessageBody, email);
                    if(resp == "Sent")
                    {
                        return resp;
                    }
                }
                
            }
            return resp;
        }

        private string BuildEmailBody(string message, string templateName, string subject)
        {
            var strMessage = "";

            try
            {
                _LogManager.SaveLog("Start BuildEmail Body");
                var strTemplateFilePath = _hostingEnvironment.ContentRootPath + "/EmailTemplates/" + templateName;
                var reader = new StreamReader(strTemplateFilePath);
                strMessage = reader.ReadToEnd();
                reader.Close();
            }
            catch (Exception ex)
            {
                _LogManager.SaveLog($"An error occurred while seeding the database  {ex.Message} {ex.StackTrace} {ex.InnerException} {ex.Source}");
            }
            strMessage = strMessage.Replace("[[[Title]]]", string.IsNullOrEmpty(subject) ? "Notification " : subject);
            strMessage = strMessage.Replace("[[[message]]]", message);
            return strMessage;
        }
        private async Task<string> SendEmailByGmailAsync(string toEmail, string subject, string messageBody)
        {
            var resp = string.Empty;
            try
                {
                    var body = messageBody;
                    MailMessage message = new MailMessage();
                    message.To.Add(new MailAddress(toEmail));
                    //message.From = new MailAddress(_configuration["EmailFrom"]);
                    message.From = new MailAddress("kennyoluwadamilare20@gmail.com");
                    message.Subject = subject;
                    message.Body = body;
                    message.IsBodyHtml = true;

                    
                    var smtp = new SmtpClient();
                    var credential = new NetworkCredential
                    {
                        //UserName = _configuration["Username"],
                        //Password = _configuration["UserPassword"]
                        UserName = "kennyoluwadamilare20@gmail.com",
                        Password = "florence202"
                    };
                    smtp.Credentials = credential;
                    smtp.Host = _configuration["Emailhost"];
                    // smtp.Port = 587;
                    smtp.Port = 587;
                    smtp.EnableSsl = true;
                    //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    //ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyError) => true);
                    await smtp.SendMailAsync(message);
                    resp="Sent";
                }
                catch (Exception ex)
                {
                    _LogManager.SaveLog($"An error occurred while seeding the database  {ex.Message} {ex.StackTrace} {ex.InnerException} {ex.Source}");
                    
                }

                return resp;
        }
        
        private async Task<string> SendEmailAsync(string toEmail, string subject, string messageBody)
        {
            var resp = string.Empty;
            try
            {
                var body = messageBody;
                var message = new MailMessage();
                message.To.Add(new MailAddress(toEmail));
                message.From = new MailAddress(_configuration["EmailFrom"]);
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = true;

                // UserName = _configuration["Username"],
                //     Password = _configuration["UserPassword"]
                var smtp = new SmtpClient();
                var credential = new NetworkCredential
                {
                    UserName = "kennyoluwadamilare20@gmail.com",
                    Password = "florence202"
                };
                smtp.Credentials = credential;
                smtp.Host = _configuration["Emailhost"];
                smtp.Port = 25;
                smtp.EnableSsl = true;
                await smtp.SendMailAsync(message);
                resp = "Sent";
                return resp;
            }
            catch (Exception ex)
            {
                _LogManager.SaveLog($"An error occurred while seeding the email  {ex.Message} {ex.StackTrace} {ex.InnerException} {ex.Source}");
            }

            return resp;
        }

        
        public string SendSmsAsync(string recipentPhone, string messageBody)
        {
            try
            {
                string ACCOUNT_SID = _configuration["ACCOUNT_SID"];
                string AUTH_TOKEN = _configuration["AUTH_TOKEN"];


                if(recipentPhone != null)
                {
                    string phone = "+234" + recipentPhone.Substring(1);
                    TwilioClient.Init(ACCOUNT_SID, AUTH_TOKEN);

                    var message = MessageResource.Create(
                        body: messageBody,
                        from: new Twilio.Types.PhoneNumber("+12815325287"),
                        to: new Twilio.Types.PhoneNumber(phone)

                    );

                    return message.Sid;
                }
                
                
                
                //Task.CompletedTask;
                
            }
            catch (Exception ex)
            {
                _LogManager.SaveLog($"An error occurred while seeding the database  {ex.Message} {ex.StackTrace} {ex.InnerException} {ex.Source}");
            }
            return "";
        }
        
        public async Task<TwoFactorResponseModel> ValidateTwoFactorCodeAsync(string code, int userid)
        {
            // Initially setting this to Fail
            var response = new TwoFactorResponseModel
            {
                Code = string.Empty,
                Email = string.Empty,
                IsValid = false,
                Message = "Code-InValid",
                StatusCode = HttpStatusCode.Unauthorized,
                errorCode = -5
            };

            try
            {
                if(!string.IsNullOrEmpty(code))
                {

                    var result = await _twoFactorCode.GetAsync(x => x.UserId == userid && x.CodeExpired == false &&
                                    x.ExpiryDate > DateTime.Now &&
                                    !x.CodeIsUsed && x.Attempts <= 3);

                    if(result != null)
                    {
                        if(result.OneTimeCode != code)
                        {
                            result.Attempts -= 1;
                            _twoFactorCode.Update(result);
                            await _ApplicationDbContext.SaveChangesAsync();

                            response.Attempts = (int)result.Attempts;
                            
                            if (result.Attempts == 0)
                            {
                                //user need to be lock

                            }

                            return response;
                        }
                        else if(result.OneTimeCode == code)
                        {
                            result.CodeExpired = true;
                            result.CodeIsUsed = true;
                            response.Code = result.OneTimeCode;
                            response.Email = "";
                            response.IsValid = true;
                            response.Message = "Code-Valid";
                            response.StatusCode = HttpStatusCode.OK;
                            response.errorCode = 0;  
                            
                            return response;
                        }

                    }
                }
            }
            catch(Exception ex)
            {
                _LogManager.SaveLog($"An error occurred while seeding the database  {ex.Message} {ex.StackTrace} {ex.InnerException} {ex.Source}");
            }
            return response;
        }
    
    
        public bool SendMails(string toEmail, string subject, string messageBody)
        {
                //var sql = "select * from DangoteMt940config";
                //var smtpdetails = db.Query<DangoteMt940config>(sql).FirstOrDefault();
                //string froemail = smtpdetails.FromEmail;
                var body = messageBody;
                Attachment attachment;
                //attachment = new System.Net.Mail.Attachment(path);
                MailMessage message = new MailMessage(_configuration["FromEmail1"], toEmail);
                message.Subject = subject;
                //smtpdetails.MT940MailBodyFormat = smtpdetails.MT940MailBodyFormat.Replace("{{ACCOUNTNAME}}", "DANGOTE CEMENT SIERRALEONE");
                //message.Body = smtpdetails.MT940MailBodyFormat;
                message.Body = body;
                message.IsBodyHtml = true;
                //message.Attachments.Add(attachment);
                var port = 587;
                //SmtpClient client = new SmtpClient(, port);
                SmtpClient client = new SmtpClient();
                var credential = new NetworkCredential
                {
                        // UserName = _configuration["Username"],
                        // Password = _configuration["UserPassword"]
                        UserName = "kennyoluwadamilare20@gmail.com",
                        Password = "florence202"
                };
                //smtpdetails.SmtpPassword = String.IsNullOrEmpty(smtpdetails.SmtpPassword) ? null : MT940Model.Decrypt(smtpdetails.SmtpPassword);
                //client.Credentials = new System.Net.NetworkCredential(smtpdetails.FromEmail, smtpdetails.SmtpPassword);
                client.Credentials = credential;
                client.Host = _configuration["Emailhost"];
                client.Port = port;
                //client.EnableSsl = (bool)smtpdetails.SslRequred;
                client.EnableSsl = true;
                
                //MailMessage mail = new MailMessage();
                //SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                //mail.From = new mailaddress("yourgmailaddress@gmail.com");
                //mail.to.add("thepersonyousendto@theiraddress.com");
                //mail.Subject = "Test Mail through my Gmail account";
                //mail.Body = "<p>Hello, this is an <b>HTML</b> email body<p>";
                //mail.IsBodyHtml = true;
                //SmtpServer.Port = 587;
                //SmtpServer.Credentials = new system.net.networkcredential("yourgmailaddress@gmail.com", "yourgmailpassword");
                //SmtpServer.EnableSsl = true;
                //SmtpServer.Send(mail);
                // Credentials are necessary if the server requires the client
                // to authenticate before it will send email on the client's behalf.
                //client.UseDefaultCredentials = false;
                try
                {
                    client.Send(message);
                    return true;
                }
                catch (Exception ex)
                {
                    _LogManager.SaveLog("Exception caught in ,ail sending " + ex.ToString());
                }
            return false;
        }
    
    
    }

}