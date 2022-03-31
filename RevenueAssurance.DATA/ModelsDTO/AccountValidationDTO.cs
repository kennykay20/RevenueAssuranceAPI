namespace RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO
{
    public class AccountValidationDTO
    {
         public string AcctType { get; set; }
        public string AcctNo { get; set; }
        public string CrncyCode { get; set; }
        public string Username { get; set; }


         public int? sRsmId { get; set; }
         public string sRsmIdString { get; set; }
            //
            public int nErrorCode { get; set; }
            public string sErrorText { get; set; }
            public string nBalance { get; set; }
            public decimal nBalanceDec { get; set; }
            public string sName { get; set; }
            public string sStatus { get; set; }
            public string nBranch { get; set; }
            public string sCrncyIso { get; set; }
            public string sAddress { get; set; }
            public string sTransNature { get; set; }
            public string sChequeStatus { get; set; }
            public string sAccountType { get; set; }
            public string sProductCode { get; set; }
            public string ProductCode { get; set; }
            public string sBirthDate { get; set; }
            public string sIdentityType { get; set; }
            public string sIdNo { get; set; }
            public string sNationality { get; set; }
            public string sCustomerId { get; set; }
            public string sCustomerType { get; set; }
            public string sSector { get; set; }
            public string sEmail { get; set; }
            public string sRsmEmail { get; set; }
            public string nCashBalance { get; set; }
            public string sStaffAcct { get; set; }
            public string sRsmName { get; set; }
            public string sAcctOpenDate { get; set; }
            public string nLastChqNo { get; set; }
            public string sCity { get; set; }
            public string drNarr { get; set; }
            public string crNarr { get; set; }
            public string sBranchName { get; set; }
            public string BranchName { get; set; }
            public string sSectorName { get; set; }
            public string LoginId { get; set; }

            public string AvailBal { get; set; }	
            public string AcctName { get; set; }	
            public string AcctStatus { get; set; }
            public string AcctCCy	 { get; set; }
            public string CcyCode  { get; set; }	
            public string nDrIntRate  { get; set; }	 
            public string sOdOption  { get; set; }	 
            public string sOdOptionValue  { get; set; }	 
            public string sOdExpiryDate  { get; set; }	 
            public string nUnapprovedOdRate  { get; set; }	 
            public string nOdLimit  { get; set; }	 
            public string CustId  { get; set; }	  
    }
}


