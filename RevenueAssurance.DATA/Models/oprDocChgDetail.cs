using System;
using System.ComponentModel.DataAnnotations;

namespace RevAssuranceApi.RevenueAssurance.DATA.Models
{
    public class oprDocChgDetail
    {
        [Key]

        public Int64? ItbId { get; set; }
        public int? DocumentId { get; set; }
        public int? ServiceId { get; set; }
        public Int64? ServiceItbId { get; set; }
        public string Description { get; set; }
        public decimal? ChargeRate { get; set; }
        public int? Qty { get; set; }
        public decimal? TotalCharge { get; set; }
        public DateTime? DateCreated { get; set; }
    }
}