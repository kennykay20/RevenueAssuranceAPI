

namespace RevAssuranceWebAPi.AnythingGood.DATA.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class AggregatedCounter
    {
         [Key]
        public string Key { get; set; }
        public long Value { get; set; }
        public DateTime? ExpireAt { get; set; }
    }
}
