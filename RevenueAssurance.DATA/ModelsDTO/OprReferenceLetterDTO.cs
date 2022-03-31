using System;

namespace RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO
{
    public class OprReferenceLetterDTO
    {
        public Int64? ItbId { get; set; }	
        public int? ServiceId	{ get; set; }	
        public int? OrigDeptId	{ get; set; }	
        public string ReferenceNo	{ get; set; }	
        public string AcctType	{ get; set; }	
        public string AcctNo	{ get; set; }	
        public string AcctName	{ get; set; }	
        public string CcyCode	{ get; set; }	
        public decimal? AvailBal	{ get; set; }	
        public string AcctSic	{ get; set; }	
        public string AcctStatus	{ get; set; }	
        public int? BranchNo	{ get; set; }	
        public string AcctProductCode	{ get; set; }	
        public string AcctCustNo	{ get; set; }	
        public string BidNo	{ get; set; }	
        public string ReferenceReason	{ get; set; }	
        public string TemplateContentIds	{ get; set; }	
        public string AddresseeName	{ get; set; }	
        public string Addr1	{ get; set; }	
        public string Addr2	{ get; set; }	
        public string Addr3	{ get; set; }	
        public string Addr4	{ get; set; }	
        public DateTime? BalanceDate	{ get; set; }	
        public int? RsmId	{ get; set; }	
        public int? IncomeBranch	{ get; set; }	
        public string InstrumentStatus	{ get; set; }	
        public  DateTime? TransactionDate	{ get; set; }	
        public  DateTime? ValueDate	{ get; set; }	
        public  DateTime DateCreated	{ get; set; }	
        public string Status	{ get; set; }	
        public string ServiceStatus	{ get; set; }	
        public int? UserId	{ get; set; }	
        public int? SupervisorId	{ get; set; }	
        public int? ApprovedBy	{ get; set; }	
        public  DateTime? ApprovalDate	{ get; set; }	
        public int? OriginatingBranchId	{ get; set; }	
        public int? ProcessingDeptId	{ get; set; }	
        public int? ValAcctError	{ get; set; }	
        public int? ErrorCode	{ get; set; }	
        public string ErrorMsg	{ get; set; }	
        public string AmmendntReasonIds	{ get; set; }	
        public string AmmendedBy	{ get; set; }	
        public  DateTime? AmmendedDate	{ get; set; }	
        public string RePrintReasonIds	{ get; set; }	
        public  int? RePrintedBy	{ get; set; }	
        public  DateTime? RePrintedDate	{ get; set; }	
        public int? DismissedBy	{ get; set; }	
        public  DateTime? DismissedDate	{ get; set; }	
        public string Rejected	{ get; set; }
        public int? RejectedBy{get; set;}	
        public string RejectionIds	{ get; set; }	
        public  DateTime? RejectionDate	{ get; set; }	
        public int? PrintCount  { get; set; }	
        public string InstrumentCcy	{ get; set; }	
        public decimal? CreditAmount  { get; set; }	
        public DateTime? AcctOpeningDate { get; set; }
        public string UserName	{ get; set; }	
    }
}