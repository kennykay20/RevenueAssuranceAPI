

namespace RevAssuranceWebAPi.AnythingGood.DATA.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class admStatusItem
    {
         [Key]
        public int ItbId { get; set; }
        public string StatusValue { get; set; }
        public string Status { get; set; }
        public string PaymentProcessing { get; set; }
        public string AdminRecord { get; set; }
        public string ClientRecord { get; set; }
    }
}
