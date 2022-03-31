

namespace RevAssuranceWebAPi.AnythingGood.DATA.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class OprBondsAndGuarantee
    {
         [Key]
        public int ItbId { get; set; }
        public string SameChrgeAcct { get; set; }
        public int? BranchNo { get; set; }
        public string ReferenceNo { get; set; }
        public string AcctNo { get; set; }
        public string AcctType { get; set; }
        public string AcctName { get; set; }
        public string AcctCcy { get; set; }
        public string AcctSic { get; set; }
        public string ChargeAcctNo { get; set; }
        public string ChargeAcctType { get; set; }
        public string ChargeAcctName { get; set; }
        public string ChargeAcctCcy { get; set; }
        public string ChgAcctSic { get; set; }
        public string InstrumentCcy { get; set; }
        public string Beneficiary { get; set; }
        public string Purpose { get; set; }
        public string ContractNo { get; set; }
        public DateTime? ContractDate { get; set; }
        public string CollateralType { get; set; }
        public string CollateralAcctNo { get; set; }
        public string CollateralAcctName { get; set; }
        public string CollateralAcctType { get; set; }
        public string CollateralCcyCode { get; set; }
        public decimal? CollateralAmount { get; set; }
        public string CollaOthers { get; set; }
        public string IndustrySector { get; set; }
        public decimal? Amount { get; set; }
        public int? Type { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public int? Tenor { get; set; }
        public string TenorPeriod { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string ChargeCode { get; set; }
        public decimal? ChargeRate { get; set; }
        public decimal? ChargeAmount { get; set; }
        public decimal? TaxAmount { get; set; }
        public decimal? FcyChargeAmount { get; set; }
        public decimal? ExchangeRate { get; set; }
        public decimal? EquivChargeAmount { get; set; }
        public string ContAssetAcctNo { get; set; }
        public string ContAssetAcctType { get; set; }
        public string ContLiabAcctNo { get; set; }
        public string ContLiabAcctType { get; set; }
        public string Insurer { get; set; }
        public string Insured { get; set; }
        public string PolicyNo { get; set; }
        public DateTime? PeriodFrom { get; set; }
        public DateTime? PeriodTo { get; set; }
        public string AdresseeName { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string AddressLine4 { get; set; }
        public DateTime? TransactionDate { get; set; }
        public string InstrumentStatus { get; set; }
        public DateTime? ReturnDate { get; set; }
        public int? ReturnUserId { get; set; }
        public string Status { get; set; }
        public DateTime? DateCreated { get; set; }
        public int? UserId { get; set; }
        public int? ProcessingDept { get; set; }
        public int? SupervisorId { get; set; }
        public string TemplateContent { get; set; }
        public string AmmendmentReason { get; set; }
        public string RePrintingReason { get; set; }
        public int? PrimaryInstrumentId { get; set; }
        public int? IncomeBranch { get; set; }
        public int? RsmId { get; set; }
        public int? ServiceId { get; set; }
        public int? InitiatingDept { get; set; }
        public string Rejected { get; set; }
        public string RejectionReason { get; set; }
        public string Narration { get; set; }
        public int? RejectedBy { get; set; }
        public DateTime? RejectedDate { get; set; }
        public decimal? AvailBal { get; set; }
        public string AcctStatus { get; set; }
        public decimal? ChargeAcctAvailbal { get; set; }
        public string ChargeAcctStatus { get; set; }
        public string ChargeAcctIndusSector { get; set; }
        public string ContAssetCcyCode { get; set; }
        public string ContLiabAcctCcyCode { get; set; }
        public string ContAssetAcctStatus { get; set; }
        public string ContLiabAcctStatus { get; set; }
        public string ContAssetAcctName { get; set; }
        public string ContLiabAcctSName { get; set; }
        public string CollateralAcctStatus { get; set; }
    }
}
