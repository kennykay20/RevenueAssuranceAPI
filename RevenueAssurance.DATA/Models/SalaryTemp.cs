

namespace RevAssuranceWebAPi.AnythingGood.DATA.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class SalaryTemp
    {
         [Key]
        public decimal ItbId { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? PostingDate { get; set; }
        public DateTime? TransactionDate { get; set; }
        public DateTime? ValueDate { get; set; }
        public string DrAcctBranchCode { get; set; }
        public string DrAcctNo { get; set; }
        public string DrAcctType { get; set; }
        public string DrAcctName { get; set; }
        public decimal? DrAcctBalance { get; set; }
        public string DrAcctStatus { get; set; }
        public string DrAcctTC { get; set; }
        public string DrAcctNarration { get; set; }
        public string DrAcctAddress { get; set; }
        public string DrAcctClassCode { get; set; }
        public decimal? DrAcctChequeNo { get; set; }
        public string DrAcctChargeCode { get; set; }
        public decimal? DrAcctChargeAmt { get; set; }
        public decimal? DrAcctTaxAmt { get; set; }
        public string DrAcctChargeNarr { get; set; }
        public DateTime? DrDateOfAcctOpening { get; set; }
        public string DrIndustryCode { get; set; }
        public int? DrAcctCustNo { get; set; }
        public string DrAcctCustType { get; set; }
        public string CcyCode { get; set; }
        public string DrCr { get; set; }
        public decimal? Amount { get; set; }
        public string Status { get; set; }
        public string TransTracer { get; set; }
        public string TransReference { get; set; }
        public int? ServiceId { get; set; }
        public string CrAcctBranchCode { get; set; }
        public string CrAcctNo { get; set; }
        public string CrAcctType { get; set; }
        public string CrAcctName { get; set; }
        public decimal? CrAcctBalance { get; set; }
        public string CrAcctStatus { get; set; }
        public string CrAcctTC { get; set; }
        public string CrAcctNarration { get; set; }
        public string CrAcctAddress { get; set; }
        public string CrAcctClassCode { get; set; }
        public decimal? CrAcctChequeNo { get; set; }
        public string CrAcctChargeCode { get; set; }
        public decimal? CrAcctChargeAmt { get; set; }
        public decimal? CrAcctTaxAmt { get; set; }
        public string CrAcctChargeNarr { get; set; }
        public DateTime? CrDateOfAcctOpening { get; set; }
        public string CrIndustryCode { get; set; }
        public int? CrAcctCustNo { get; set; }
        public string CrAcctCustType { get; set; }
        public string Channel { get; set; }
        public int? DeptId { get; set; }
        public int? ProcessingDept { get; set; }
        public decimal? BalAfterPosting { get; set; }
        public decimal? BatchId { get; set; }
        public int? BatchSeqNo { get; set; }
        public string IsSalary { get; set; }
        public int? UserId { get; set; }
        public string CbsUserId { get; set; }
        public int? Direction { get; set; }
        public int? SupervisorId { get; set; }
        public string CbsUSupervisorId { get; set; }
        public string CbsTranId { get; set; }
        public string OriginatingBranchId { get; set; }
        public int? PostingErrorCode { get; set; }
        public string PostingText { get; set; }
        public int? ErrorCode { get; set; }
        public string ErrorMsg { get; set; }
        public int? Reversal { get; set; }
        public string ParentTransactionId { get; set; }
        public string Rejected { get; set; }
        public string RejectionReason { get; set; }
        public int? RejectedBy { get; set; }
        public DateTime? RejectedDate { get; set; }
        public int? ReversedBy { get; set; }
        public int? ClosedBy { get; set; }
        public DateTime? ClosedDate { get; set; }
        public string Filename { get; set; }
        public string ProcessingType { get; set; }
        public string BatchType { get; set; }
        public int? ValidateErrorCode { get; set; }
        public string ValidateErrorText { get; set; }
        public int? ValidateUserId { get; set; }
        public string PostType { get; set; }
    }
}
