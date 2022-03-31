

namespace RevAssuranceWebAPi.AnythingGood.DATA.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class OprInstrument
    {
         [Key]
        public Int64? ItbId { get; set; }
        public int? ServiceId { get; set; }
        public int? OrigDeptId { get; set; }
        public string ReferenceNo { get; set; }
        public string AcctType { get; set; }
        public string AcctNo { get; set; }
        public string AcctName { get; set; }
        public string CcyCode { get; set; }
        public decimal? AvailBal { get; set; }
        public string AcctSic { get; set; }
        public string AcctStatus { get; set; }
        public int? BranchNo { get; set; }
        public string AcctProductCode { get; set; }
        public string AcctCustNo { get; set; }
        public int? ProcessingDeptId { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public int? Tenor { get; set; }
        public string TenorPeriod { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public decimal? Amount { get; set; }
        public string InstrumentCcy { get; set; }
        public int? RsmId { get; set; }
        public string ContractNo { get; set; }
        public DateTime? ContractDate { get; set; }
        public string Beneficiary { get; set; }
        public string Purpose { get; set; }
        public string AdresseeName { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string AddressLine4 { get; set; }
        public string TemplateContentIds { get; set; }
        public string Insured { get; set; }
        public string InsuranceCoyName { get; set; }
        public int? InsuranceCoverTypeId { get; set; }
        public DateTime? InsuranceEffectiveDate { get; set; }
        public DateTime? InsuranceExpiryDate { get; set; }
        public string InsurancePolicyNo { get; set; }
        public decimal? InsuranceSumAssured { get; set; }
        public string InsurancePremiumPayable { get; set; }
        public string InsuranceCurrency { get; set; }
        public string InsuranceLocationOfProperty { get; set; }
        public string ContDrAcctType { get; set; }
        public string ContDrAcctNo { get; set; }
        public string ContDrAcctName { get; set; }
        public string ContDrCcyCode { get; set; }
        public decimal? ContDrAvailBal { get; set; }
        public string ContDrAcctStatus { get; set; }
        public string ContCrAcctType { get; set; }
        public string ContCrAcctNo { get; set; }
        public string ContCrAcctName { get; set; }
        public string ContCrCcyCode { get; set; }
        public decimal? ContCrAvailBal { get; set; }
        public string ContCrAcctStatus { get; set; }
        public string AmmendmentReason { get; set; }
        public DateTime? AmmendmentDate { get; set; }
        public int? AmmendedBy { get; set; }
        public DateTime? ReturnDate { get; set; }
        public int? ReturnUserId { get; set; }
        public string RePrintingReason { get; set; }
        public DateTime? RePrintdate { get; set; }
        public int? RePrintBy { get; set; }
        public string Rejected { get; set; }
        public string RejectionReason { get; set; }
        public int? RejectedBy { get; set; }
        public DateTime? RejectedDate { get; set; }
        public int? ParentId { get; set; }
        public string InstrumentStatus { get; set; }
        public DateTime? TransactionDate { get; set; }
        public DateTime? ValueDate { get; set; }
        public string Status { get; set; }
        public DateTime? DateCreated { get; set; }
        public int? UserId { get; set; }
        public int? SupervisorId { get; set; }
        public string ContDrAcctNarration { get; set; }
        public string ContCrAcctNarration { get; set; }
        public int? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public int? OriginatingBranchId { get; set; }
        public DateTime? AmortDate { get; set; }
        public int? RePrintCount { get; set; }
        public int? AmmendCount { get; set; }
        public string RejectedIds { get; set; }
        public string AmmendmentReasonIds { get; set; }
        public string RePrintReasonIds { get; set; }
        public int? DismissedBy { get; set; }
        public DateTime? DismissedDate { get; set;}
    }
}
