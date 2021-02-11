namespace RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO
{
    public  class LoginReturnProperty
    {
        public string EnforcePassChange { get; set; }
        public string FullName { get; set; }
        public long UserId { get; set; }
        public string Username { set; get; }
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
        public short LicenseErrorCode { get; set; }  = -99;
         public int LicenseNumberOfDay { get; set; }  
        public string ErrorMessage { get; set; }
       // public ErrorDisplay oErrorDisplay { get; set; }
        public int? ClassCode { get; set; }
        public string Url { get; set; }
    }
}