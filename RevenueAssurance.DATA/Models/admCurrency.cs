

namespace RevAssuranceWebAPi.AnythingGood.DATA.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class admCurrency
    {
         [Key]
        public string CurrencyNo { get; set; }
        public string Description { get; set; }
        public string IsoCode { get; set; }
        public bool? IsLocalCurrency { get; set; }
        public string Status { get; set; }
        public DateTime? DateCreated { get; set; }
        public int? UserId { get; set; }
        public decimal? NumberOfDecimal { get; set; }
        public int? Weight { get; set; }
        public string CountryCode2 { get; set; }
    }
}
