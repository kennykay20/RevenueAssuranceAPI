

namespace RevAssuranceWebAPi.AnythingGood.DATA.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class admCountry
    {
         [Key]
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public string CurrencyCode { get; set; }
        public string CountryNumber { get; set; }
        public int? UserId { get; set; }
        public int? AuthorizeId { get; set; }
        public DateTime? DateCreated { get; set; }
        public string Status { get; set; }
        public string ServiceIdentifier { get; set; }
        public string timezone { get; set; }
        public decimal? CountryDirectLimit { get; set; }
        public decimal? CountryTranferLimit { get; set; }
        public string RtgsFlag { get; set; }
        public int? IBANLength { get; set; }
    }
}
