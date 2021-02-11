

namespace RevAssuranceWebAPi.AnythingGood.DATA.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class admRejectionReason
    {
         [Key]
        public int ItbId { get; set; }
        public string RejectionCode { get; set; }
        public string Description { get; set; }
        public int? UserId { get; set; }
        public DateTime? DateCreated { get; set; }
        public string Status { get; set; }
    }
}
