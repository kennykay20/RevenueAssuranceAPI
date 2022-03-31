namespace RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO
{
    public class UserReturnDetails
    {
        public string UserName { get; set; }
        public int? RoleId { get; set; }
        public string RoleName { get; set; }
        public long? UserId { get; set; }
        public string FullName { get; set; }
        public string LoginId { get; set; }
        public string staffId { get; set; }
        public int? DeptId { get; set; }
        public int? BranchNo { get; set; }
        public string BranchName{ get; set; }
        public string lastLoginTime { get; set; } 
        public string BankingDate { get; set; }
        public string CBSStatus { get; set; }
        public string loginType { get; set; }
        public string userLoginType { get; set; }
    }
}