using System;
using System.ComponentModel.DataAnnotations;

namespace RevAssuranceApi.RevenueAssurance.DATA.Models
{
    public class OprAmmendAndReprint
    {
         [Key]
        public long ItbId { get; set; }
        public long? ServiceItbId { get; set; }
        public int? ServiceId { get; set; }
        public string Action { get; set; }
        public int ReasonId { get; set; }
        public bool RequireCharge { get; set; }
        public int? UserId { get; set; }
        public DateTime DateCreated { get; set; }
    }
}