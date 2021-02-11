

namespace RevAssuranceWebAPi.AnythingGood.DATA.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class admUserProfile
    {
         [Key]
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string LoginId { get; set; }
        public bool? IsFirstLogin { get; set; }
        public string IsSupervisor { get; set; }
        public bool? LoggedOn { get; set; }
        public int? RoleId { get; set; }
        public int? DeptId { get; set; }
        public int? BranchNo { get; set; }
        public DateTime PasswordExpiryDate { get; set; }
        public string Status { get; set; }
        public string BranchName { get; set; }
        public string RoleName { get; set; }
        public string EmailAddress { get; set; }
        public string MobileNo { get; set; }
        public string EnforcePswdChange { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? DateCreated { get; set; }
        public string Password { get; set; }
        public short? loginstatus { get; set; }
        public short? lockcount { get; set; }
        public short? logincount { get; set; }
        public bool? UseCbsAuth { get; set; }
        public bool? CanPrintStatement { get; set; }
        public bool? CanPrintBidSecurity { get; set; }
        public bool? CanPrintRefLetter { get; set; }
        public bool? CanPrintBond { get; set; }
        public bool? CanPrintTradeRef { get; set; }
        public bool? CanPrintOD { get; set; }
        public bool? CanReverse { get; set; }
        public string DataSources { get; set; }
        public bool? AuthUser { get; set; }
        public bool? CanApprove { get; set; }
        public int? RsmId { get; set; }



    



    }


}
