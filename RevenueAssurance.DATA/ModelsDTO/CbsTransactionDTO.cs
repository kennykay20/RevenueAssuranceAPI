using System;
namespace RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO
{
    public class CbsTransactionDTO
    {
        public Int64? ItbId { get; set; }
        public int? ServiceId { get; set; }
        public string ServiceName { get; set; }
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
        public string DrAcctChargeAmt { get; set; }
        public string DrAcctTaxAmt { get; set; }
        public decimal? DrAcctChargeRate { get; set; }
        public string DrAcctChargeNarr { get; set; }
        public decimal? DrAcctBalAfterPost { get; set; }
        public DateTime? DrAcctOpeningDate { get; set; }
        public string DrAcctIndusSector { get; set; }
        public decimal? DrAcctCbsTranId { get; set; }
        public string DrAcctCustType { get; set; }
        public int? DrAcctCustNo { get; set; }
        public decimal? DrAcctCashBalance { get; set; }
        public decimal? DrAcctCashAmt { get; set; }
        public string DrAcctCity { get; set; }
        public string DrAcctIncBranch { get; set; }
        public int? DrAcctValUserId { get; set; }
        public int? DrAcctValErrorCode { get; set; }
        public string DrAcctValErrorMsg { get; set; }
        public string CcyCode { get; set; }
        public decimal? TaxRate { get; set; }
        public decimal? Amount { get; set; }
        public string CrAcctBranchCode { get; set; }
        public string CrAcctNo { get; set; }
        public string CrAcctType { get; set; }
        public string CrAcctName { get; set; }
        public decimal? CrAcctBalance { get; set; }
        public string CrAcctStatus { get; set; }
        public string CrAcctTC { get; set; }
        public string CrAcctNarration { get; set; }
        public string CrAcctAddress { get; set; }
        public string CrAcctProdCode { get; set; }
        public decimal? CrAcctChequeNo { get; set; }
        public string CrAcctChargeCode { get; set; }
        public decimal? CrAcctChargeAmt { get; set; }
        public decimal? CrAcctTaxAmt { get; set; }
        public decimal? CrAcctChargeRate { get; set; }
        public string CrAcctChargeNarr { get; set; }
        public DateTime? CrAcctOpeningDate { get; set; }
        public string CrAcctIndusSector { get; set; }
        public decimal? CrAcctCbsTranId { get; set; }
        public string CrAcctCustType { get; set; }
        public int? CrAcctCustNo { get; set; }
        public decimal? CrAcctCashBalance { get; set; }
        public decimal? CrAcctCashAmt { get; set; }
        public string CrAcctCity { get; set; }
        public string CrAcctIncBranch { get; set; }
        public int? CrAcctValUserId { get; set; }
        public int? CrAcctValErrorCode { get; set; }
        public string CrAcctValErrorMsg { get; set; }
        public DateTime? TransactionDate { get; set; }
        public DateTime? ValueDate { get; set; }
        public DateTime? DateCreated { get; set; }
        public string Status { get; set; }
        public string TransReference { get; set; }
        public string TransTracer { get; set; }
        public string ChannelId { get; set; }
        public int? DeptId { get; set; }
        public int? ProcessingDept { get; set; }
        public decimal? BatchId { get; set; }
        public int? BatchSeqNo { get; set; }
        public DateTime? PostingDate { get; set; }
        public int? UserId { get; set; }
        public string CbsUserId { get; set; }
        public int? SupervisorId { get; set; }
        public string CbsSupervisorId { get; set; }
        public int? Direction { get; set; }
        public decimal? PrimaryId { get; set; }
        public int? PostingErrorCode { get; set; }
        public string PostingErrorDescr { get; set; }
        public string Rejected { get; set; }
        public int? Reversal { get; set; }
        public int? RejectedBy { get; set; }
        public DateTime? RejectionDate { get; set; }
        public int? ReversedBy { get; set; }
        public DateTime? ReversalDate { get; set; }
        public string OriginatingBranchId { get; set; }
        public string ParentTransactionId { get; set; }
        public string CbsChannelId { get; set; }
        public decimal? LcyEquivAmt { get; set; }
        public decimal? LclEquivExchRate { get; set; }
        public string AuthRejection { get; set; }
        public string RejectedIds { get; set; }
        public bool? Select { get; set; }
        public string DeptName { get; set; }
        public string OriginatingBranchName { get; set; }
        public string Limit { get; set; }
        public string FullName { get; set; }

        public string BranchName { get; set; }

        public string ServiceDescription { get; set; }
    }





}