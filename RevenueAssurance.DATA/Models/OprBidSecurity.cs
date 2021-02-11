

namespace RevAssuranceWebAPi.AnythingGood.DATA.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class OprBidSecurity
    {
         [Key]
        public int ItbId { get; set; }
        public string InstrumentCurrency { get; set; }
        public string SameChrgeAcct { get; set; }
        public int? ServiceId { get; set; }
        public int? BranchNo { get; set; }
        public string ChgAcctNo { get; set; }
        public string ChgAcctType { get; set; }
        public string ChgAcctName { get; set; }
        public string ChgAcctCcy { get; set; }
        public string ChgAcctSic { get; set; }
        public string ReferenceNo { get; set; }
        public string AcctNo { get; set; }
        public string AcctType { get; set; }
        public string AcctName { get; set; }
        public int? templateType { get; set; }
        public decimal? SecurityAmt { get; set; }
        public DateTime? SecurityDate { get; set; }
        public int? Duration { get; set; }
        public string Tenor { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string Purpose { get; set; }
        public string AddresseeName { get; set; }
        public string Addr1 { get; set; }
        public string Addr2 { get; set; }
        public string Addr3 { get; set; }
        public string Addr4 { get; set; }
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
        public string TemplateContent { get; set; }
        public string AmmendmentReason { get; set; }
        public string RePrintingReason { get; set; }
        public int? PrimaryInstrumentId { get; set; }
        public int? IncomeBranch { get; set; }
        public int? RsmId { get; set; }
        public string InstrumentStatus { get; set; }
        public int? InitiatingDept { get; set; }
        public string RejectionReason { get; set; }
        public string Rejected { get; set; }
        public string Narration { get; set; }
        public int? RejectedBy { get; set; }
        public DateTime? RejectedDate { get; set; }
        public decimal? AvailBal { get; set; }
        public string AcctStatus { get; set; }
        public decimal? ChgAcctAvailbal { get; set; }
        public string ChgAcctStatus { get; set; }
        public int? ApprovedBy { get; set; }
    }
}
