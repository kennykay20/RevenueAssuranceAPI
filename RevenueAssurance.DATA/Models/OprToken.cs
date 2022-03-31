

namespace RevAssuranceWebAPi.AnythingGood.DATA.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class OprToken
    {
         [Key]
        public int ItbId { get; set; }
        public int? ServiceId { get; set; }
        public int? OrigDeptId { get; set; }
        public string ReferenceNo { get; set; }
        public int? BranchNo { get; set; }
        public string AcctNo { get; set; }
        public string AcctType { get; set; }
        public string AcctName { get; set; }
        public decimal? AvailBal { get; set; }
        public string AcctSic { get; set; }
        public string AcctStatus { get; set; }
        public string CcyCode { get; set; }
        public string SerialNo { get; set; }
        public string TransId { get; set; }
        public int? WkfId { get; set; }
        public DateTime? RecordDate { get; set; }
        public string ServiceStatus { get; set; }
        public int? RsmId { get; set; }
        public string Status { get; set; }
        public string OriginatingBranchId { get; set; }
        public int? ProcessingDeptId { get; set; }
        public DateTime? TransactionDate { get; set; }
        public DateTime? ValueDate { get; set; }
        public DateTime? DateCreated { get; set; }
        public int? UserId { get; set; }
        public int? SupervisorId { get; set; }
        public string ValAcctError { get; set; }
        public int? ErrorCode { get; set; }
        public string ErrorMsg { get; set; }
        public int? DismissedBy { get; set; }
        public DateTime? DismissedDate { get; set; }
        public string Rejected { get; set; }
        public string RejectionIds { get; set; }
        public DateTime? RejectionDate { get; set; }
        public int? RejectedBy { get; set; }
        
    }
}
