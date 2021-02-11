

namespace RevAssuranceWebAPi.AnythingGood.DATA.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class OprApproval
    {
         [Key]
        public int ItbId { get; set; }
        public int? ServiceId { get; set; }
        public int? BranchNo { get; set; }
        public string ReferenceNo { get; set; }
        public string AcctNo { get; set; }
        public string AcctType { get; set; }
        public string AcctName { get; set; }
        public DateTime? ValueDate { get; set; }
        public DateTime? TransDate { get; set; }
        public string IndustrySector { get; set; }
        public string CcyCode { get; set; }
        public string ChargeCode { get; set; }
        public decimal? ChargeRate { get; set; }
        public decimal? ChargeAmount { get; set; }
        public decimal? TaxAmount { get; set; }
        public string ChargeNarration { get; set; }
        public string Status { get; set; }
        public string ErrorMsg { get; set; }
        public int? DismissedBy { get; set; }
        public DateTime? DismissedDate { get; set; }
        public DateTime? DateCreated { get; set; }
        public int? UserId { get; set; }
        public int? SupervisorId { get; set; }
        public int? ProcessingDept { get; set; }
        public decimal? ExchangeRate { get; set; }
        public int? PrimaryInstrumentId { get; set; }
        public int? IncomeBranch { get; set; }
        public string TransTracer { get; set; }
        public int? OriginDept { get; set; }
        public int? ProcDept { get; set; }
        public int? ApprovedBy { get; set; }
        public int? RejectedBy { get; set; }
    }
}
