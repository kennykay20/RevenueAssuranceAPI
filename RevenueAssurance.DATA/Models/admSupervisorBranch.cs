

namespace RevAssuranceWebAPi.AnythingGood.DATA.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class admSupervisorBranch
    {
         [Key]
        public int ItbId { get; set; }
        public int SupervisorId { get; set; }
        public string BranchCode { get; set; }
        public int? UserId { get; set; }
        public DateTime? DateCreated { get; set; }
        public string Status { get; set; }
    }
}
