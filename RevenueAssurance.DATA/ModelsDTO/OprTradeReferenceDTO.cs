using System;

namespace RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO
{
    public class OprTradeReferenceDTO
    {
        public int? ItbId { get; set; }
        public string InstrumentCurrency { get; set; }
        public string SameChrgeAcct { get; set; }
        public int? ServiceId { get; set; }
        public int? BranchNo { get; set; }
        public string ReferenceNo { get; set; }
        public string AcctNo { get; set; }
        public string AcctType { get; set; }
        public string AcctName { get; set; }
        public decimal? CreditAmount { get; set; }
        public string BidNo { get; set; }
        public string ReferenceReason { get; set; }
        public string TemplateContentIds { get; set; }
        public string AddresseeName { get; set; }
        public string Addr1 { get; set; }
        public string Addr2 { get; set; }
        public string Addr3 { get; set; }
        public string Addr4 { get; set; }
        public DateTime? ValueDate { get; set; }
        public DateTime? TransDate { get; set; }
        public DateTime? TransactionDate { get; set; }
        public string IndustrySector { get; set; }
        public string CcyCode { get; set; }
        public string ServiceStatus { get; set; }
        public string Status { get; set; }
        public string ErrorMsg { get; set; }
        public int? DismissedBy { get; set; }
        public DateTime? DismissedDate { get; set; }
        public DateTime? DateCreated { get; set; }
        public int? UserId { get; set; }
        public int? SupervisorId { get; set; }
        public int? ProcessingDept { get; set; }
        public decimal? ExchangeRate { get; set; }
        public string TemplateContent { get; set; }
        public string AmmendmentReason { get; set; }
        public int? AmmendedBy { get; set; }
        public DateTime? AmmendedDate { get; set; }
        public string RePrintingReason { get; set; }
        public int? RePrintedBy { get; set; }
        public DateTime? RePrintedDate { get; set; }
        public int? PrimaryInstrumentId { get; set; }
        public int? RsmId { get; set; }
        public int? InitiatingDept { get; set; }
        public string RejectionReason { get; set; }
        public string Rejected { get; set; }
        public string Narration { get; set; }
        public int? RejectedBy { get; set; }
        public DateTime? RejectedDate { get; set; }
        public decimal? AvailBal { get; set; }
        public string AcctStatus { get; set; }
        public int? ApprovedBy { get; set; }
        public string userProcessingDept { get; set; }
        public string originBranch { get; set; }
        public string userName { get; set; }
    }
}