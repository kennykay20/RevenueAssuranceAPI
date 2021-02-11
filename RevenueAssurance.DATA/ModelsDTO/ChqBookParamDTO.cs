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
    }


}