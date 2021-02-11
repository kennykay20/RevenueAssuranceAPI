namespace RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO
{
    public class AccountValParam
    {
         public string AcctType { get; set; }
        public string AcctNo { get; set; }
        public string CrncyCode { get; set; }
        public string Username { get; set; }
        public int? ConnectionStringId { get; set; } = 1;
    }
}