

namespace RevAssuranceWebAPi.AnythingGood.DATA.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class CollateralStatus
    {
         [Key]
        public int ItbId { get; set; }
        public string Status { get; set; }
    }
}
