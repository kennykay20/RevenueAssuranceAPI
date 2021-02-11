namespace RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO
{
    public class CbsTransInsertBatchDTO
    {
        public long ItbId {get; set; }	
        public string DrAcctNo { get; set; }	
        public string DrAcctType { get; set; }
        public string CcyCode { get; set; }
        public string CrAcctNo	{ get; set; }
    }
}