namespace RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO
{
      public class RevCalChargeModel
    {
        public string sTaxNarration { get; set; }
        public string sChannel { get; set; }
        public string nReference { get; set; }
        public string sChargeType { get; set; }
        public string pnServiceId { get; set; }
        public string pnDirection { get; set; }
        public string pnTranAmount { get; set; }
        public string pnProductCode { get; set; }
        public string PsAcctNo { get; set; }
        public string psCustomerNo { get; set; }
        public string psCurrency { get; set; }
        public string pnTemplateId { get; set; }
        public string pnTransCount { get; set; }
        public string psDrCr { get; set; }
        public string psAmmendment { get; set; }
        public string psReprint { get; set; }
        public string pnIChgAmount { get; set; }

        public int nReturnCode { get; set; }
        public string sReturnMsg { get; set; }
        public string sErrorText { get; set; }
        public string nChargeCode { get; set; }
        
        public decimal nChargeRate { get; set; }
        
        public decimal nOrigChargeAmount { get; set; }
        
        public decimal nExchRate { get; set; }
            
        public decimal nActualChgAmt { get; set; }
        
        public decimal nTaxAmt { get; set; }
        
        public decimal nTaxRate { get; set; }
        
        public string  sCurrency { get; set; }
        public string sChgCurrency { get; set; }
        public string sNarration { get; set; }
        public string sTransCurrency { get; set; }
        public string sDRCR { get; set; }
        public string sChargeIncAcctType { get; set; }

        public string sChargeIncAcctNo { get; set; }
        public int nErrorCode { get; set; }

        //////////  Chg Acct Details ----------------

        public string chgAcctNo	{ get; set; }
        public string chgAcctType	{ get; set; }
        public string chgAcctName	{ get; set; }
        public string chgAvailBal	{ get; set; }
        public string chgAcctCcy  { get; set; }
        public string chgAcctStatus	{ get; set; }
        public string chargeCode { get; set; }

        /////Income

        public string incBranch { get; set; }
        public string incBranchString { get; set; }
        public string incAcctNo	{ get; set; }
        public string incAcctType { get; set; }
        public string incAcctName	{ get; set; }
        public decimal incAcctBalance	{ get; set; }
         public string incAcctBalanceString	{ get; set; }
        public string incAcctStatus	{ get; set; }
        public string incAcctNarr	{ get; set; }


        //
        public string chargeRate { get; set; }
        public string origChgAmount { get; set; }
        public string exchangeRate { get; set; }
        public string equivChgAmount { get; set; }
        public string taxAmount { get; set; }
        public string sTaxAcctNo { get; set; }
        public string sTaxAcctType { get; set; }

        public int? TemplateId {get; set;}

    }
}