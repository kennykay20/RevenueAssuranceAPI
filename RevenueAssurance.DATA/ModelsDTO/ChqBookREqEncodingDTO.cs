using System;

namespace RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO
{
    public class ChqBookREqEncodingDTO
    {
        public string TransTracer { get; set; }
        public string BookletType { get; set; }
        public int Quantity { get; set; }
        public string StartNo { get; set; }
        public string EndNo { get; set; }
        public string BranchNo { get; set; }
        public int InitiatingDept { get; set; }
        public decimal? Chg1Amount { get; set; }
        public decimal? Chg2Amount { get; set; }
        public int ItbId { get; set; }
        public int? InitiatedBy { get; set; }
        public string DrAcctBranchCode { get; set; }
        public string DrAcctNo { get; set; }
        public string DrAcctType { get; set; }
        public string DrAcctName { get; set; }
        public string DrAcctTC { get; set; }
        public string DrAcctChargeCode { get; set; }
        public decimal? DrAcctChargeAmt { get; set; }
        public decimal? DrAcctTaxAmt { get; set; }
        public string DrAcctChargeNarr { get; set; }
        public string CrAcctBranchCode { get; set; }
        public string CrAcctNo { get; set; }
        public string CrAcctTC { get; set; }
        public string CrAcctType { get; set; }
        public string CrAcctName { get; set; }
        public string Filename { get; set; }
        public string CrAcctChargeCode { get; set; }
        public decimal? CrAcctChargeAmt { get; set; }
        public decimal? CrAcctTaxAmt { get; set; }
        public string CrAcctChargeNarr { get; set; }
        public string Rejected { get; set; }
        public decimal? Amount { get; set; }
        public string Status { get; set; }
        public string TransReference { get; set; }
        public int BatchId { get; set; }
        public int UserId { get; set; }
        public decimal? DrAmount { get; set; }
        public decimal? CrAmount { get; set; }
        public int DeptId { get; set; }
        public string CcyCode { get; set; }
        public DateTime DateCreated { get; set; }
        public string Direction { get; set; }
        public string OriginatingBranchId { get; set; }
        public string Servicename { get; set; }
        public int ServiceId { get; set; }
        public DateTime PostingDate { get; set; }
        public DateTime TransactionDate { get; set; }
        public DateTime TransDate { get; set; }
        public string ChannelId { get; set; }
        public string branchName { get; set; }
        public string CreatedBy { get; set; }
        public string OrginDept { get; set; }
        public decimal? TaxAmount { get; set; }
        public decimal? ChargeAmount { get; set; }

    }

}