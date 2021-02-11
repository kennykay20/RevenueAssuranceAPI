using Microsoft.Extensions.Configuration;

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
            return _configuration["SybaseUserName"].ToString();
        } 
        
        public string GetDefaultCon()
        { 
            //will later encript the below
         //  return  _configuration["dbDefault"].ToString();
           //Configuration.GetConnectionString("dbCon"))

            return  _configuration.GetConnectionString("dbCon");
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