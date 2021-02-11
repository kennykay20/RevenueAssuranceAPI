

namespace RevAssuranceWebAPi.AnythingGood.DATA.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class admAmmendAndRepriReason
    {
         [Key]
        public int ItbId { get; set; }
        public string AmdReason { get; set; }
        public string RePrintReason { get; set; }
        public int UserId { get; set; }
        public string Status { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
