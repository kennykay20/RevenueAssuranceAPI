

namespace RevAssuranceWebAPi.AnythingGood.DATA.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class TransactionLog
    {
         [Key]
        public decimal ItbId { get; set; }
        public string TransReference { get; set; }
        public DateTime? TransactionDate { get; set; }
        public DateTime? DateCreated { get; set; }
    }
}
