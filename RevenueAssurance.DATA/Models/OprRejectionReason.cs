

namespace RevAssuranceWebAPi.AnythingGood.DATA.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class OprRejectionReason
    {
        [Key]
        public decimal ItbId { get; set; }
        public int? RejectionReasonId { get; set; }
        public decimal? TransactionId { get; set; }
        public int? ServiceId { get; set; }
        public int? UserId { get; set; }
        public DateTime? DateCreated { get; set; }
        public string Status { get; set; }
    }
}
