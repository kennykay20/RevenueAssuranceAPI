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
     public class LogManager
    {   
        
        IConfiguration _IConfiguration;
        private static Object cvLockObject = new Object();
        private string cvsLogFile;
        private  string filePath ;
        private string LogSize;
        
        public LogManager(IConfiguration IConfiguration){

            _IConfiguration = IConfiguration;
            cvsLogFile = _IConfiguration["LgPth:LogFile"];
            filePath = _IConfiguration["LgPth:LogFilePath"];
            LogSize = _IConfiguration["LgPth:LogSize"];
        }

       
        private static string GetUniqueFilePath11(string filepath)
        {
            if (File.Exists(filepath))
            {
                string folder = Path.GetDirectoryName(filepath);
                string filename = Path.GetFileNameWithoutExtension(filepath);
                string extension = Path.GetExtension(filepath);
                int number = 1;

                Match regex = Regex.Match(filepath, @"(.+) \((\d+)\)\.\w+");

                if (regex.Success)
                {
                    filename = regex.Groups[1].Value;
                    number = int.Parse(regex.Groups[2].Value);
                }

                do
                {
                    number++;
                    filepath = Path.Combine(folder, string.Format("{0} ({1}){2}", filename, number, extension));
                }
                while (File.Exists(filepath));
            }

            return filepath;
        }
        public  void SaveLog11(string psDetails)
        {

            FileInfo f = new FileInfo(Path.Combine(filePath, cvsLogFile));
            string new_file_name = string.Empty;

            if (filePath != null && cvsLogFile != null)
            {
                if (File.Exists(Path.Combine(filePath, cvsLogFile)))
                {
                    long s1 = f.Length;
                    if (s1 > Convert.ToInt32(LogSize))
                    {
                       
                        new_file_name = "";// GetUniqueFilePath(Path.Combine(filePath, cvsLogFile));

                        string filename = new_file_name;


                        File.Move(Path.Combine(filePath, cvsLogFile), filename);
                    }
                }
            }
            lock (cvLockObject)
            {
                if (filePath != null && cvsLogFile != null)
                {
                    File.AppendAllText(Path.Combine(filePath, cvsLogFile), DateTime.Now.ToString() + ": " + psDetails + Environment.NewLine);
                }
            }
        }
    
       private static string GetUniqueFilePath(string filepath, string lastModifyDate)
        {
            if (File.Exists(filepath))
            {
                string folder = Path.GetDirectoryName(filepath);
                string filename = Path.GetFileNameWithoutExtension(filepath);
                string extension = Path.GetExtension(filepath);
                int number = 1;

                Match regex = Regex.Match(filepath, @"(.+) \((\d+)\)\.\w+");

                if (regex.Success)
                {
                    filename = regex.Groups[1].Value;
                    number = int.Parse(regex.Groups[2].Value);
                }
                do
                {
                    number++;

                    filepath = Path.Combine(folder, string.Format("{0} ({1}){2}", filename, lastModifyDate, extension));
                }
                while (File.Exists(filepath));
            }

            return filepath;
        }

        public void SaveLog(string psDetails)
        {
            try
            {
                string todaysDate =  string.Format("{0:yyyy-MM-dd}", DateTime.Now); 

                filePath = filePath.Replace("{TodaysDate}", todaysDate);

                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                cvsLogFile = cvsLogFile.Replace("{TodaysDate}", todaysDate);

                //if (!Directory.Exists(cvsLogFile))
                //{
                //    Directory.CreateDirectory(cvsLogFile);
                //}

                if (cvsLogFile != null)
                {
                    FileInfo f = new FileInfo(cvsLogFile);

                    string new_file_name = string.Empty;

                    if (File.Exists(cvsLogFile))
                    {
                        DateTime LastmodificationDate = File.GetLastWriteTime(cvsLogFile).Date;
                        var TodaysDate = DateTime.Now.Date;

                        string ConvertLastModiydate = string.Format("{0: yyyy-MM-dd}", LastmodificationDate);

                        if (TodaysDate > LastmodificationDate)
                        {
                            new_file_name = GetUniqueFilePath(Path.Combine(filePath, cvsLogFile), ConvertLastModiydate);
                            string filename = new_file_name;

                            File.Move(Path.Combine(filePath, cvsLogFile), filename);
                        }
                    }
                    lock (cvLockObject)
                    {
                        string sError = DateTime.Now.ToString() + ": " + psDetails;

                        StreamWriter sw = new StreamWriter(cvsLogFile, true, Encoding.ASCII);

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