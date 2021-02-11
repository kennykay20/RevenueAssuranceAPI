

namespace RevAssuranceWebAPi.AnythingGood.DATA.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class OprPrintingHistory
    {
         [Key]
        public int Itbid { get; set; }
        public int? ServiceId { get; set; }
        public int? TransactionId { get; set; }
        public int? UserId { get; set; }
        public int? DateCreated { get; set; }
        public string Status { get; set; }
    }
}
