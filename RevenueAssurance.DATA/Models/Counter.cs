

namespace RevAssuranceWebAPi.AnythingGood.DATA.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Counter
    {
         [Key]
        public string Key { get; set; }
        public int Value { get; set; }
        public DateTime? ExpireAt { get; set; }
    }
}
