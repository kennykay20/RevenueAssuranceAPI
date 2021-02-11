using System;
using System.ComponentModel.DataAnnotations;

namespace RevAssuranceApi.RevenueAssurance.DATA.Models
{
    public class admTaxConfig
    {
        [Key]
        public int TaxId { get; set;}
        public string TaxName { get; set;}
        public string ShortName { get; set;}
        public decimal TaxRate { get; set;}
        public string AcctNo { get; set;}
        public string AcctType { get; set;}
        public string UserId { get; set;}
        public int Status { get; set;}
        public DateTime DateCreated { get; set;}
    }
}