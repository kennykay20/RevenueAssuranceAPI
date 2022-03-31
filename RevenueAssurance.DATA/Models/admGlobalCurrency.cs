using System;
using System.ComponentModel.DataAnnotations;

namespace RevAssuranceApi.RevenueAssurance.DATA.Models
{

    public class admGlobalCurrency
    {
        [Key]
        public int ItbId { get; set; }
        public string CountryCode2 { get; set; }
        public string CountryCode3 { get; set; }
        public string CountryName { get; set; }
        public string CurrencyName { get; set; }
        public string CcyCode { get; set; }
        public int CcyNo { get; set; }
        public string Status { get; set; }
    }
}