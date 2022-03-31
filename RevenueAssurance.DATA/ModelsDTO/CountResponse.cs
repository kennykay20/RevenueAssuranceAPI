namespace RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO
{
    public class CountResponse
    {
         public int Total { get; set; }
        public int UnPosted { get; set; }
        public int UnAuthorized { get; set; }
        public int Verified { get; set; }
        public int AmortVerified { get; set; }
    }
}