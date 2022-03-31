namespace RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO
{
    public class CollateralCoreBankingDTOParam
    {
         public string TransactionRef { get; set; }
        public string AccountType { get; set; }
        public string AccountNo { get; set; }
        public long? HoldID { get; set; }
        public decimal? HoldAmt { get; set; }
        public string UserName { get; set; }
        public string HoldReason { get; set; }
        public string Modify { get; set; }
         public int? ConnectionStringId { get; set; } = 1;
    }

       public class CollateralHoldRes
        {
            public int? ErrorCode { get; set; }
            public string ErrorText { get; set; }
            public long HoldId { get; set; }

        }
}