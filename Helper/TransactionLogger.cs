using System;
using System.IO;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace RevAssuranceApi.Helper
{
    public class TransactionLogger
    {
        private Object cvLockObject = new Object();
         IConfiguration _IConfiguration;
         private string TransLog;
        

         public TransactionLogger(IConfiguration IConfiguration)
         {
              _IConfiguration = IConfiguration;
              TransLog = _IConfiguration["LgPth:TransLog"];
         }
        

        public void SaveTransLog(string psDetails, string TransRef)
        {
            string TransLogFile = _IConfiguration["LgPth:TransLogFile"];
            try
            {
                string todaysDate = string.Format("{0:yyyy-MM-dd}", DateTime.Now); 

                TransLog = TransLog.Replace("{TodaysDate}", todaysDate);

                if (!Directory.Exists(TransLog))
                {
                    Directory.CreateDirectory(TransLog);
                }
                
                TransLogFile = TransLogFile
                                        .Replace("{TodaysDate}", todaysDate)
                                        .Replace("{TransRef}", TransRef);

                string newFileStrim = TransLogFile;
                if (newFileStrim != null)
                {
                    lock (cvLockObject)
                    {
                        string sError = $"{ DateTime.Now.ToString() } : { psDetails }";

                        StreamWriter sw = new StreamWriter(newFileStrim, true, Encoding.ASCII);

                        sw.WriteLine(sError);
                        sw.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                var reeee = ex.Message == null ? ex.InnerException.Message : ex.Message;
            }
        }
    }
}