using System;
using System.ComponentModel.DataAnnotations;

namespace RevAssuranceApi.RevenueAssurance.DATA.Models
{
    public class OprAcctClosure
    {
            [Key]
            public int ItbId { get ; set; }
            public int ServiceId { get ; set; }
            public int BranchNo { get ; set; }
            public string ReferenceNo { get ; set; }
            public string AcctType{ get ; set; }
            public string AcctNo{ get ; set; }
            public string AcctName{ get ; set; }
            public string CcyCode{ get ; set; }
            public string AcctSic{ get ; set; }
            public string AcctStatus{ get ; set; }
            public decimal AvailBal{ get ; set; }
            public string Beneficiary { get ; set; }
            public string Reason { get ; set; }
            public DateTime? ValueDate { get ; set; }
            public DateTime? TransDate { get ; set; }
            public string ServiceStatus { get ; set; }
            public string Status { get ; set; }
            public string ErrorMsg { get ; set; }
            public int DismissedBy{ get ; set; }
            public DateTime? DismissedDate{ get ; set; }
            public DateTime DateCreated { get ; set; }
            public int UserId{ get ; set; }
            public int SupervisorId { get ; set; }
            public string ApprovedBy { get ; set; }
            public int RejectedBy { get ; set; }
            public string RejectedIds { get ; set; }
            public DateTime? RejectedDate { get ; set; }

            public int? OriginBranchNo	{ get ; set; }
            public int? Dept	{ get ; set; }
            public int? ProcessingDept	{ get ; set; }
            public decimal? Amount { get; set; }
            public int? RsmId { get; set; }
            // public string InstrumentCcy {get; set;}
    }
}