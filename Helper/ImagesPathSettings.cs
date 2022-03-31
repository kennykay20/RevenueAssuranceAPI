using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using RevAssuranceApi.Response;

namespace RevAssuranceApi.Helper
{
     public class ImagesPathSettings
    {   
        
        IConfiguration _IConfiguration;
       
        public ImagesPathSettings(IConfiguration IConfiguration){

            _IConfiguration = IConfiguration;
           
        }

       
        public  string GetImagePath(int? ServiceId)
        {
            string Path = string.Empty;
            string Images = "Images";
            try
            {
                if(ServiceId == 13)
                {
                    return Path = _IConfiguration[$"{Images}:OverDrafMemo"];
                }
            }
            catch(Exception ex)
            {
                var exM = ex == null ? ex.InnerException.Message : ex.Message;
            }
            
            return Path;
        }
    
    }
}