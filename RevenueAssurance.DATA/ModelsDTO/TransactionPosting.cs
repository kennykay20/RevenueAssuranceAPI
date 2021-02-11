namespace RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO
{
       public class TransactionPosting
        {
            public string TransRef { get; set; }
            public string DrAccountNo { get; set; }
            public string DrAcctType { get; set; }
            public string DrAcctTC { get; set; }
            public string DrAcctNarration { get; set; }
            public string TranAmount { get; set; }
            public string CrAcctNo { get; set; }
            public string CrAcctType { get; set; }
            public string CrAcctTC { get; set; }
            public string CrAcctNarration { get; set; }
            public string CurrencyISO { get; set; }
            public string PostDate { get; set; }
            public string ValueDate { get; set; }
            public string UserName { get; set; }
            public string TranBatchID { get; set; }
            public string ChargeCode { get; set; }
            public string ChargeAmt { get; set; }
            public string SupervisorName { get; set; }
            public string ChannelId { get; set; }
            public string ForcePostFlag { get; set; }
            public string Reversal { get; set; }
            public string TaxAmt { get; set; }
            public string DrAcctChargeCode { get; set; }
            public string DrAcctChargeAmt { get; set; }
            public string DrAcctTaxAmt { get; set; }
            public string DrAcctChequeNo { get; set; }
            public string DrAcctChgDescr { get; set; }
            public string CrAcctChargeCode { get; set; }
            public string CrAcctChargeAmt { get; set; }
            public string CrAcctTaxAmt { get; set; }
            public string CrAcctChequeNo { get; set; }
            public string CrAcctChgDescr { get; set; }
            public string TransTracer { get; set; }
            public string ParentTransRef { get; set; }
            public string RoutingNo { get; set; }
            public string FloatDays { get; set; }
            public string RimNo { get; set; }
            public string Direction { get; set; }
            public string ChargeAcct { get; set; }
            public string DrAcctCashAmt { get; set; }
            public string CrAcctCashAmt { get; set; }
            public string DrAcctChargeBranch { get; set; }
            public string CrAcctChargeBranch { get; set; }
             public int? ConnectionStringId { get; set; } = 1;

        }
}