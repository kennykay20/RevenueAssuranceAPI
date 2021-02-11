using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using anythingGoodApi.AnythingGood.DATA.Models;
using RevAssuranceApi.AppSettings;
using Dapper;
using Microsoft.Extensions.Configuration;
using RevAssuranceApi.Helper;
using RevAssuranceApi.RevenueAssurance.Repository.DapperDAL;
using RevAssuranceWebAPi.AnythingGood.DATA.Models;

namespace RevAssuranceApi.EmailSettings
{
    public class EmailConfig
    {
        IConfiguration _configuration;
         IDbConnection db = null;

        public EmailConfig(IConfiguration configuration)
         {
          _configuration = configuration;
         

        }
        public async Task<bool> SendEmailOnboardingAgent(Users Postparam)
        {
            try
            {         
                db = new SqlConnection(_configuration["dbDefault"].ToString()); 
                string mailBody = string.Empty;

                 DynamicParameters param = new DynamicParameters();
                 param.Add("@remarks", "SellerSignUp");

                var rtn = new DapperDATAImplementation<EMailFormat>();
                
                var setup = await rtn.ResponseObj("sp_EmailFormat", param, db);
               
                string subject = setup.SignUpCustomerSubject;
              
                string AgentFullName = Postparam.LastName + " " + Postparam.FirstName;
               
                mailBody = setup.SignUpCustomerMailFormat;
                string emailLink = Postparam.EncryptConEmail;
                
                mailBody = mailBody
                           .Replace("<Customers name>", AgentFullName)
                           .Replace("<Till account number>", Postparam.UserName)
                           .Replace("<Agent code>", Postparam.UserName)
                           .Replace("<websiteUrlToCompleteReg>", _configuration["emailConUrl"].ToString() + emailLink);

                          
                
               
                try
                {
                 
                     MailMessage message  = new MailMessage();
                     message.To.Add(new MailAddress(Postparam.EmailAddress));
                     message.From = new MailAddress(_configuration["FromEmail"].ToString());

                      message.Subject = subject;
                      message.Body = mailBody;

                        message.IsBodyHtml = true;

                        using (var smtpclient = new SmtpClient())
                        {
                            try
                            {
                                 var credential = new NetworkCredential
                                {
                                    UserName = _configuration["FromEmail"].ToString(),
                                    Password = SecurityEncryptEmail.Decrypt(_configuration["EmailPwd"].ToString()) 
                                };


                                smtpclient.Credentials = credential;
                                smtpclient.Host = setup.SmtpServer;
                                smtpclient.Port =  Convert.ToInt32(setup.SmtpPort);
                                smtpclient.EnableSsl = true;
                                
                                smtpclient.Send(message);
                                 return true;

                            }
                            catch (Exception ex)
                            {
                                var exM = ex == null ? ex.InnerException.Message : ex.Message;
                               // _smartObjects.SaveLog($"error while sending mail:  {exM}");
                            }
                        }

                   


                 
                   
                }
                catch (Exception ex)
                {
                    var exM  = ex == null ? ex.InnerException.Message : ex.Message;
                    return false;
                }
            }
            catch (Exception ex)
            {
               
              var exM  = ex == null ? ex.InnerException.Message : ex.Message;
                    return false;
            }

             return false;
        }



        public async Task<bool> SendEmailOnboardingAgentSucess(Users Postparam)
        {
            try
            {         
                db = new SqlConnection(_configuration["dbDefault"].ToString()); 
                string mailBody = string.Empty;

                 DynamicParameters param = new DynamicParameters();
                 param.Add("@remarks", "RegistrationWelcomeMsg");

                var rtn = new DapperDATAImplementation<EMailFormat>();
                
                var setup = await rtn.ResponseObj("sp_EmailFormat", param, db);
               
                string subject = setup.SignUpCustomerSubject;
              
                string AgentFullName = Postparam.LastName + " " + Postparam.FirstName;
               
                mailBody = setup.SignUpCustomerMailFormat;
                string emailLink = Postparam.EncryptConEmail;
                
                mailBody = mailBody
                           .Replace("<Customers name>", AgentFullName)
                           .Replace("<Till account number>", Postparam.UserName)
                           .Replace("<Agent code>", Postparam.UserName)
                           .Replace("<websiteUrlToCompleteReg>", _configuration["emailConUrl"].ToString());

                          
                
               
                try
                {
                 
                     MailMessage message  = new MailMessage();
                     message.To.Add(new MailAddress(Postparam.EmailAddress));
                     message.From = new MailAddress(_configuration["FromEmail"].ToString());

                      message.Subject = subject;
                      message.Body = mailBody;

                        message.IsBodyHtml = true;

                        using (var smtpclient = new SmtpClient())
                        {
                            try
                            {
                                 var credential = new NetworkCredential
                                {
                                    UserName = _configuration["FromEmail"].ToString(),
                                    Password = SecurityEncryptEmail.Decrypt(_configuration["EmailPwd"].ToString()) 
                                };


                                smtpclient.Credentials = credential;
                                smtpclient.Host = setup.SmtpServer;
                                smtpclient.Port =  Convert.ToInt32(setup.SmtpPort);
                                smtpclient.EnableSsl = true;
                                
                                smtpclient.Send(message);
                                 return true;

                            }
                            catch (Exception ex)
                            {
                                var exM = ex == null ? ex.InnerException.Message : ex.Message;
                               // _smartObjects.SaveLog($"error while sending mail:  {exM}");
                            }
                        }

                   


                 
                    
                    
                }
                catch (Exception ex)
                {
                    var exM  = ex == null ? ex.InnerException.Message : ex.Message;
                    return false;
                }
            }
            catch (Exception ex)
            {
               
               var exM  = ex == null ? ex.InnerException.Message : ex.Message;
               return false;
            }

             return false;
        }

        public async Task<bool> ResentEmailLink(Users Postparam)
        {
            try
            {         
                db = new SqlConnection(_configuration["dbDefault"].ToString()); 
                string mailBody = string.Empty;

                 DynamicParameters param = new DynamicParameters();
                 param.Add("@remarks", "RegistrationWelcomeMsg");

                var rtn = new DapperDATAImplementation<EMailFormat>();
                
                var setup = await rtn.ResponseObj("sp_EmailFormat", param, db);
               
                string subject = setup.SignUpCustomerSubject;
              
                string AgentFullName = Postparam.LastName + " " + Postparam.FirstName;
               
                mailBody = setup.SignUpCustomerMailFormat;
                string emailLink = SecurityEncryptEmail.Encrypt(Postparam.EncryptConEmail);
                
                mailBody = mailBody
                           .Replace("<Customers name>", AgentFullName)
                           .Replace("<Till account number>", Postparam.UserName)
                           .Replace("<Agent code>", Postparam.UserName)
                           .Replace("<websiteUrlToCompleteReg>", _configuration["emailConUrl"].ToString() + emailLink);

                          
                
               
                try
                {
                 
                     MailMessage message  = new MailMessage();
                     message.To.Add(new MailAddress(Postparam.EmailAddress));
                     message.From = new MailAddress(_configuration["FromEmail"].ToString());

                      message.Subject = subject;
                      message.Body = mailBody;

                        message.IsBodyHtml = true;

                        using (var smtpclient = new SmtpClient())
                        {
                            try
                            {
                                 var credential = new NetworkCredential
                                {
                                    UserName = _configuration["FromEmail"].ToString(),
                                    Password = SecurityEncryptEmail.Decrypt(_configuration["EmailPwd"].ToString()) 
                                };


                                smtpclient.Credentials = credential;
                                smtpclient.Host = setup.SmtpServer;
                                smtpclient.Port =  Convert.ToInt32(setup.SmtpPort);
                                smtpclient.EnableSsl = true;
                                
                                smtpclient.Send(message);
                                 return true;

                            }
                            catch (Exception ex)
                            {
                                var exM = ex == null ? ex.InnerException.Message : ex.Message;
                               // _smartObjects.SaveLog($"error while sending mail:  {exM}");
                            }
                        }

                   


                 
                   
                }
                catch (Exception ex)
                {
                    var exM  = ex == null ? ex.InnerException.Message : ex.Message;
                    return false;
                }
            }
            catch (Exception ex1)
            {
               
              var exM1  = ex1 == null ? ex1.InnerException.Message : ex1.Message;
                    return false;
            }

             return false;
        }



    }
}