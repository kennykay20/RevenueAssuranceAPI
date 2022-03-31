namespace RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO
{
    public class AcctStateCoreBankParam
    { 
        public string AccountNo {get; set; } 
        public string AcctType { get; set; } 
        public string DateFrom { get; set; } 
        public string DateTo { get; set;} 
        public string FromAmount { get; set;} 
        public string ToAmount { get; set;} 
        public string CoreBankingState {get; set;} 
         public int? ConnectionStringId { get; set; } = 1;
    }
}