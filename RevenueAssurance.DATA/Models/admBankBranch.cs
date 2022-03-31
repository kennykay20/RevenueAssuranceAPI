

namespace RevAssuranceWebAPi.AnythingGood.DATA.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class admBankBranch
    {
         [Key]
        public int Itbid { get; set; }
        public int BranchNo { get; set; }
        public string BranchCode { get; set; }
        public string BranchAddress { get; set; }
        public int UserId { get; set; }
        public DateTime DateCreated { get; set; }
        public string BranchName { get; set; }
        public string Status { get; set; }
        public Boolean? IsHeadOffice { get; set; }
        public string BranchPreffix { get; set; }
        public string BranchSuffix { get; set; }
        public string TelePhone { get; set; }
        public string AltTelephone { get; set; }
    }
}
