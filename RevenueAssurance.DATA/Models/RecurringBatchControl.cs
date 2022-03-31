

namespace RevAssuranceWebAPi.AnythingGood.DATA.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class RecurringBatchControl
    {
         [Key]
        public decimal BatchId { get; set; }
        public int? ServiceId { get; set; }
        public string StartDateInitialization { get; set; }
        public DateTime? StartDate { get; set; }
        public decimal ItbId { get; set; }
        public DateTime? NextRunDate { get; set; }
        public string RunPeriod { get; set; }
        public int? RunTerm { get; set; }
        public string Description { get; set; }
        public string CcyCode { get; set; }
        public decimal? TotalDr { get; set; }
        public decimal? TotalCr { get; set; }
        public decimal? Difference { get; set; }
        public string ProcessedFlag { get; set; }
        public string OriginBankBranchId { get; set; }
        public int? ProcessingDept { get; set; }
        public int? DeptId { get; set; }
        public DateTime? LastRunDate { get; set; }
        public int? NoOfTimesRun { get; set; }
        public string Status { get; set; }
        public int? UserId { get; set; }
        public int? SupervisorId { get; set; }
        public string IsOneToOne { get; set; }
        public int? ErrorCode { get; set; }
        public string ErrorText { get; set; }
        public string Rejected { get; set; }
        public int? RejectedBy { get; set; }
        public DateTime? RejectedDate { get; set; }
        public string RejectionReason { get; set; }
        public int? ClosedBy { get; set; }
        public DateTime? DateClosed { get; set; }
        public DateTime? DateCreated { get; set; }
    }
}
