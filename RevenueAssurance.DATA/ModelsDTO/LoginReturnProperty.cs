using System;
using System.Net;

namespace RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO
{
    public  class LoginReturnProperty
    {
        public string EnforcePassChange { get; set; }
        public string FullName { get; set; }
        public long UserId { get; set; }
        public string Username { set; get; }
        public string staffId { get; set; }
        public int? branchNo { set; get; }
        public string BranchName { get; set; }
        public string branchCode { set; get; }
        public int? deptId { set; get; }
        public string BankingDate { set; get; }
        public string Status { set; get; }
        public int? RoleId { get; set; }
        public string RoleName { get; set; }
        public string LastLoginDate { get; set; }
        public short ErrorCode { get; set; }  = -99;
        public string OneTimePinCode {get; set;}
        public DateTime? ExpiryDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public short LicenseErrorCode { get; set; }  = -99;
         public int LicenseNumberOfDay { get; set; }  
        public string ErrorMessage { get; set; }

       // public ErrorDisplay oErrorDisplay { get; set; }
        public int? ClassCode { get; set; }
        public string Url { get; set; }
        public string loginType { get; set; }
        public string userLoginType { get; set; }
        
    }

    public class TwoFactorResponseModel
    {
        public bool IsValid { get; set; }
        public string selectedType { get; set; }
        public string Code { get; set; }
        public string Email { get; set; }
        public Nullable<DateTime> ExpiryTime { get; set; }
        public string Message { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public int Attempts { get; set; }
        public int errorCode { get; set; }
    }
}