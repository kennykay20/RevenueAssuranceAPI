using System;
using Microsoft.Extensions.Configuration;
using RevAssuranceApi.Helper;
using RevAssuranceApi.OperationImplemention;

namespace RevAssuranceApi.AppSettings
{
    public class AppSettingsPath
    {
        IConfiguration _configuration;
        
        public AppSettingsPath(IConfiguration configuration) 
        {
          _configuration = configuration;
        }
        public string GetSybaseUserName()
        { 
            return _configuration["SybaseUserName:cond"].ToString();
        } 

        public bool UseEncryptedDBCon() 
        {
             LogManager _LogManager = new LogManager(_configuration);
             _LogManager.SaveLog("UseEncryptedDBCon call");
            try
            {
                Formatter formatter = new Formatter();
                return formatter.ValBool(_configuration["UseEncryptedDBCon"]);
            }
            catch(Exception ex)
            {
                var exM = ex == null ? ex.InnerException.Message : ex.Message;
                 _LogManager.SaveLog($"GetDefaultCon error {exM}");
                 return false;
            }
        }
        
        public string GetDefaultCon()
        {         
            
           LogManager _LogManager = new LogManager(_configuration);
           _LogManager.SaveLog("call GETDefault con");
            _LogManager.SaveLog($"GetDefaultCon UseEncryptedDBCon: { UseEncryptedDBCon()} ");
            try
            {
                switch(UseEncryptedDBCon())
                {
                    case true:
                            string FullconString = _configuration["ConnectionStrings:dbCon"];
                
                            var DBConSetUp = new DBConSetUp(_configuration);
                            string IpOrServerName = DBConSetUp.Decrypt(_configuration["DBSettings:IpOrServerName"]) ;
                            string DBName = DBConSetUp.Decrypt(_configuration["DBSettings:DBName"]) ;
                            string UserId = DBConSetUp.Decrypt(_configuration["DBSettings:UserId"]) ;
                            string Password = DBConSetUp.Decrypt(_configuration["DBSettings:Password"]) ;
            
                            string ConnectionString =  FullconString.Replace("{IpOrServerName}", IpOrServerName)
                                .Replace("{DBName}",DBName)
                                .Replace("{UserId}", UserId)
                                .Replace("{Password}", Password);
                            

                            
                            return  ConnectionString;
                case false:
                            _LogManager.SaveLog($"Connection with no Encryption: { _configuration.GetConnectionString("dbConNonEncrypt")} ");
                            return  _configuration.GetConnectionString("dbConNonEncrypt");

                }
                
                return string.Empty;
            }
            catch(Exception ex) 
            {
                 var exM = ex == null ? ex.InnerException.Message : ex.Message;
                 _LogManager.SaveLog($"GetDefaultCon error {exM}");
                 return String.Empty;
            }
            
        } 


        public string JWTSigningKey()
        { 
            //will later encript the below
           return  _configuration["Jwt:SigningKey"].ToString();
        
        } 
        public string JWTExpiryInMinutes()
        { 
            //will later encript the below
           return  _configuration["Jwt:ExpiryInMinutes"].ToString();
        
        } 

         public string JWTSite()
        { 
            //will later encript the below
           return  _configuration["Jwt:Site"].ToString();
        
        }

        public string FromEmailAddress()
        { 
            //will later encript the below
           return  _configuration["FromEmail"].ToString();
        
        } 

         public string Upload(string service)
        { 
            switch(service)
            {
                case "vehicle":
                return  _configuration["Uploads:Vehilce"].ToString();
            }   

            return string.Empty;

        } 

    }


}