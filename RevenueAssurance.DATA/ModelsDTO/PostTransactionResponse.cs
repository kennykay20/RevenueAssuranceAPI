namespace RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO
{
    public class PostTransactionResponse
    {
        /// <summary>
        /// Below is added on April 04, 2020
        /// </summary>
        public string EquivAmt { get; set; }
        public string OrigExchRate { get; set; }
        public string ExchRate { get; set; }
        public string DrAcctChgBranch { get; set; }
        public string CrAcctChgBranch { get; set; }
        public int nErrorCode { get; set; } = -1;
        public string sErrorText { get; set; }
        public string nBalance { get; set; }
        public string nDrBalance { get; set; }
        public string nCrBalance { get; set; }
        public string sName { get; set; }
        public string sStatus { get; set; }
        public string nBranch { get; set; }
        public string nCbsTranId { get; set; }
        public string CbsTranId { get; set; }
        public string drCashAmt { get; set; }
        public string sValueDate { get; set; }
         public string TransReference { get; set; }
    }

    public class ApproveOverDraftResponse
    {
        public int nErrorCode { get; set; }
        public string sErrorText { get; set; }
    }

}