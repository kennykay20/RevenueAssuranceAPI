using System;

namespace RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO
{
    public class oprStatementReqDTO
    {
        public int? ItbId { get; set; }
        public int? ServiceId { get; set; }
        public int? BranchNo { get; set; }
        public string AcctNo { get; set; }
        public string AcctType { get; set; }
        public string AcctName { get; set; }
        public string CcyCode { get; set; }
        public decimal? AvailBal { get; set; }
        public string AcctStatus { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? DataSource { get; set; }
        public string AdrrsName { get; set; }
        public string AdrrsAddress1 { get; set; }
        public string AdrrsAddress2 { get; set; }
        public string AdrrsAddress3 { get; set; }
        public int? ServiceReason { get; set; }
        public int? NoOfPages { get; set; }
        public int? CheqNo { get; set; }
        public string ProductCode { get; set; }
        public string ChgAcctNo { get; set; }
        public string ChgAcctType { get; set; }
        public string ChgAcctName { get; set; }
        public string ChgAcctCcy { get; set; }
        public decimal? ChgAcctAvailbal { get; set; }
        public string ChgAcctStatus { get; set; }
        public string ReferenceNo { get; set; }
        public int? templateType { get; set; }
        public DateTime? ValueDate { get; set; }
        public DateTime? TransDate { get; set; }
        public string IndustrySector { get; set; }
        public string InstrumentStatus { get; set; }
        public string Status { get; set; }
        public string ErrorMsg { get; set; }
        public int? DismissedBy { get; set; }
        public DateTime? DismissedDate { get; set; }
        public DateTime? DateCreated { get; set; }
        public int? UserId { get; set; }
        public int? SupervisorId { get; set; }
        public int? ProcessingDept { get; set; }
        public string TemplateContent { get; set; }
        public int? IncomeBranch { get; set; }
        public int? RsmId { get; set; }
        public int? InitiatingDept { get; set; }
         public string RejectionIds { get; set; }
        public string Rejected { get; set; }
        public int? RejectedBy { get; set; }
        public DateTime? RejectedDate { get; set; }
        public int? ApprovedBy { get; set; }
        public DateTime? ApprovalDate { get; set; }
    }
}