namespace RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO
{
    public class ChqBookParamDTO
    {
        public string psAcctNo { get; set; }
        public string psAcctType  { get; set; }
        public int? pnFromChqNo  { get; set; }
        public int? pnToChqNo { get; set; }
        public string psUserName { get; set; }     
        public int? pnVendorId  { get; set; }    
        public string pnChqProductId { get; set; }  
        public int? pnCountOrdered { get; set; }   
        public string pnChargeAssessed { get; set; }  
        public int? pnrimno { get; set; }
         public int? ConnectionStringId { get; set; } = 1;
         public string accntNo { get; set; }
         public string acctType { get; set; }
         public string FromChqNo { get; set; }
         public string ToChqNo { get; set; }
         public string Username { get; set; }
         public string VendorId { get; set; }
         public string ChqProductId { get; set; }
         public string CountOrdered { get; set; }
         public string ChargeAssessed { get; set; }
         public string rimno { get; set; }
    }


}