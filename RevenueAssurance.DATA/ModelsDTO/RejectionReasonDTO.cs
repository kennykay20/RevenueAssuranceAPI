using System;

namespace RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO
{
    public class RejectionReasonDTO
    { 
        public int? ItbId { get; set; }
        public string RejectionCode { get; set; }
        public string Description { get; set; }
        public int? UserId { get; set; }
        public DateTime? DateCreated { get; set; }
        public string Status { get; set; }
        public bool Select { get; set; }
    }
}