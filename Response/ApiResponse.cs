
using System;
namespace RevAssuranceApi.Response
{
    public class ApiResponse
    {
        public int ResponseCode { get; set;} = -99;
        public string ResponseMessage {get; set;}
        public string UserId {get; set;}
        public long xxxx {get; set;}

        public string sErrorText { get; set;}

        public Int64 CbsItbid {get; set; }
        public string StringbuilderMessage { get; set;}
        public string FinalInstlmtDate {get; set;}
         public string expiryDate {get; set;}
        public string NextInstlmtDate {get; set;}
        public string InstlmtAmount {get; set;}
        public string FirstInstlmtDate {get; set;}
        public int? NoInstalmt {get; set;}
        public int InstlmtProcessed {get; set;}
        public string AccountFormat { get; set; }
        public string AccountType { get; set; }     
        public string AccountDescription { get; set; } 
        public string acctLength { get; set; }   

    }


}