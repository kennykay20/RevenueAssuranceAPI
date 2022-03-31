using System;
using System.Net;
using Microsoft.Extensions.Configuration;

namespace RevAssuranceApi.Helper
{
    /* This class is use to get decrypt and Encrypt Database Connection */
    public class DBConSetUp
    {
          IConfiguration _IConfiguration;
           //LogManager _LogManager;
          public DBConSetUp(IConfiguration IConfiguration)
          {
               _IConfiguration = IConfiguration;
               //_LogManager = LogManager;
          }
        public string  Encrypt(string value)
        {
            var _LogManager = new LogManager(_IConfiguration);
            try
            {
                string EncyValue = Cryptors.EncryptNoKey(value);
                return EncyValue;
            }
            catch(Exception ex)
            {
                var exM = ex == null ? ex.InnerException.Message : ex.Message;
                _LogManager.SaveLog($"Internal Error When getting Ip to Post Transaction Error: {exM}");
                return string.Empty;
            }
        }   
    
        public string  Decrypt(string value)
        {
            var _LogManager = new LogManager(_IConfiguration);
            try
            {
                string DecryptValue =  Cryptors.DecryptNoKey(value);
                return DecryptValue;
            }
            catch(Exception ex)
            {
                var exM = ex == null ? ex.InnerException.Message : ex.Message;
                _LogManager.SaveLog($"Internal Error When getting Ip to Post Transaction Error: {exM}");
                return string.Empty;
            }
        }   
    
    }
}