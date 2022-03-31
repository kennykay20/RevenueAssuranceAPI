namespace RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO
{
  

    public class CbsTransactionDTOReport
    {

        public string ItbId { get; set; }
        public string DrAcctBranchCode { get; set; }
        public string DrAcctNo { get; set; }
        public string DrAcctType { get; set; }
        public string DrAcctName { get; set; }
        public string DrAcctTC { get; set; }
        public string DrAcctChargeCode { get; set; }
        public decimal? DrAcctChargeAmt { get; set; }
        public decimal? DrAcctTaxAmt { get; set; }
        public string DrAcctChargeNarr { get; set; }
        public string DrAcctNarration { get; set; }
        public string Amount { get; set; }
        public string Status { get; set; }
        public string CrAcctBranchCode { get; set; }
        public string CrAcctNo { get; set; }
        public string CrAcctType { get; set; }
        public string CrAcctTC { get; set; }
        public string CrAcctName { get; set; }
        public string CrAcctChargeCode { get; set; }
        public string CrAcctChargeAmt { get; set; }
        public string CrAcctTaxAmt { get; set; }
        public string CrAcctChargeNarr { get; set; }
        public string CrAcctNarration { get; set; }
        public string TransReference { get; set; }
        public string BatchId { get; set; }
        public string UserId { get; set; }
        public string CcyCode { get; set; }
        public string DateCreated { get; set; }
        public string Direction { get; set; }
        public string ServiceId { get; set; }
        public string PostingDate { get; set; }
        public string TransactionDate { get; set; }
        public string ValueDate { get; set; }
        public string ChannelId { get; set; }
        public string ServiceDescription { get; set; }
        public string DeptId { get; set; }
        public string OriginatingBranchId { get; set; }
        public string BranchName { get; set; }
        public string Deptname { get; set; }
        public string FullName { get; set; }

    }

}