using System;
using System.ComponentModel.DataAnnotations;

namespace RevAssuranceApi.RevenueAssurance.DATA.Models
{
    public class admDocumentChg
    {
        [Key]
        public int? DocumentId { get; set; }
        public int? ServiceId { get; set; }
        public string Description { get; set; }
        public string ChgMetrix { get; set; }
        public string ChgBasis { get; set; }
        public int? PeriodStart { get; set; }
        public int? PeriodEnd { get; set; }
        public decimal? ChgAmount { get; set; }
        public string CcyCode { get; set; }
        public string Status { get; set; }
        public int? UserId { get; set; }
        public DateTime DateCreated { get; set; }
    }


  public class admDocumentChgTemp
    {
        public long? ItbId { get; set; } 
        public int? DocumentId { get; set; }
        public int? ServiceId { get; set; }
        public string Description { get; set; }
        public string ChgMetrix { get; set; }
        public string ChgBasis { get; set; }
        public int? PeriodStart { get; set; }
        public int? PeriodEnd { get; set; }
        public decimal? ChgAmount { get; set; }
        public string CcyCode { get; set; }
        public string Status { get; set; }
        public int? UserId { get; set; }
        public DateTime DateCreated { get; set; }
        public decimal? Total { get; set; }
         public int? Qty { get; set; }
    }
   
 
}