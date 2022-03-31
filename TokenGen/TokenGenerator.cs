using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using RevAssuranceApi.AppSettings;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace RevAssuranceApi.TokenGen
{
    public class TokenGenerator
    {
          IConfiguration _configuration;
           AppSettingsPath AppSettingsPath ;
           public TokenGenerator(IConfiguration IConfiguration){
                _configuration = IConfiguration;
           }
        public TokenObj GetToken()
        {
            TokenObj TokenObj = new TokenObj();
            
            try{

                AppSettingsPath = new AppSettingsPath(_configuration);
                var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AppSettingsPath.JWTSigningKey()));
                int exPiryInMinute = Convert.ToInt32(AppSettingsPath.JWTExpiryInMinutes());
                var ttt = DateTime.UtcNow.AddMinutes(exPiryInMinute);
                string month = string.Empty, day = string.Empty ;
                
                if(ttt.Month < 10)
                {
                    month = "0"+ ttt.Month;
                }
                else
                {
                     month =  ttt.Month.ToString();
                }
                 if(ttt.Day < 10){
                    day = "0"+ ttt.Day;
                }
                else
                {
                     day =  ttt.Day.ToString();
                }


                var expiryDayTime = $"{ttt.Year}-{month}-{day} {ttt:HH:mm:ss}";
                 
                var token = new JwtSecurityToken(
                    issuer: AppSettingsPath.JWTSite(),
                    audience:   AppSettingsPath.JWTSite(),
                    expires: DateTime.UtcNow.AddMinutes(exPiryInMinute),
                    signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)           
                );

                TokenObj.Token = new JwtSecurityTokenHandler().WriteToken(token);
                TokenObj.TokenExpireTime = expiryDayTime;

                return TokenObj;
            }
            catch(Exception ex)
            {
                   var exM =  ex == null ? ex.InnerException.Message : ex.Message;
                
                return null;
            }
        }
    
        public string GetLastLogin(){
                int exPiryInMinute = Convert.ToInt32(AppSettingsPath.JWTExpiryInMinutes());
                var ttt = DateTime.UtcNow.AddMinutes(exPiryInMinute);
                string month = string.Empty, day = string.Empty ;
                if(ttt.Month < 10)
                {
                    month = "0"+ ttt.Month;
                }
                else
                {
                     month =  ttt.Month.ToString();
                }
                 if(ttt.Day < 10){
                    day = "0"+ ttt.Day;
                }
                else
                {
                     day =  ttt.Day.ToString();
                }
                  var lastLoginTime = $"{ttt.Year}-{month}-{day} {ttt:HH:mm:ss}";

                  return lastLoginTime;
        }
    
    
    }
}

