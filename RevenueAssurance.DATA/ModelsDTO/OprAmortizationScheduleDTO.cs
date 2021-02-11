using System;

namespace RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO
{   
    public class OprAmortizationScheduleDTO
    {
       
        public int? ScheduleId { get; set; }
        public string TransTracer { get; set; }
        public int? PrimaryId { get; set; }
        public string NewAmrt { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string CurrencyCode { get; set; }
        public decimal? TotalAmount { get; set; }
        public int? NoInstalmt { get; set; }
        public decimal? InstlmtAmount { get; set; }
        public int? InstlmtProcessed { get; set; }
        public int? Term { get; set; }
        public string TermPeriod { get; set; }
        public DateTime? FirstInstlmtDate { get; set; }
        public DateTime? NextInstlmtDate { get; set; }
        public DateTime? InstlmtDueDate { get; set; }
        public DateTime? FinalInstlmtDate { get; set; }
        public string MlyProcessed { get; set; }
        public decimal? LastTransId { get; set; }
        public decimal? InstlmtAmtRem { get; set; }
        public int? InstlmtRem { get; set; }
        public string DrAcctNo { get; set; }
        public string DrAcctType { get; set; }
        public string DrAcctTC { get; set; }
        public string DrAcctNarration { get; set; }
        public string CrAcctNo { get; set; }
        public string CrAcctType { get; set; }
        public string CrAcctTC { get; set; }
        public string CrAcctNarration { get; set; }
        public string Status { get; set; }
        public int? DeptId { get; set; }
        public int? UserId { get; set; }
        public int? SupervisorId { get; set; }
        public int? ProcessingDept { get; set; }
        public DateTime? DateCreated { get; set; }
        public string OriginBranch { get; set; }
        public string Rejected { get; set; }
        public int? RejectedBy { get; set; }
        public DateTime? RejectedDate { get; set; }
        public string RejectionReason { get; set; }
        public int? ClosedBy { get; set; }
        public DateTime? DateClosed { get; set; }
        public string InstrumentRef { get; set; }
        public int? InstrumentType { get; set; }
        public int? ServiceId { get; set; }
        public string Channel { get; set; }
        public DateTime? LastPostingDate { get; set; }
        public string UserName { get; set; }
    }

}