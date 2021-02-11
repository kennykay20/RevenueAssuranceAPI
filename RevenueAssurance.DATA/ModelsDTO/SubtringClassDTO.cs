namespace RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO
{
    public class SubtringClassDTO
    {
        public string TransTracer { get; set; }
        public long? BatchNo { get; set; }
        public int? PostErrorCode { get; set; }
        public string PostErrorText { get; set; }
        public int TransTracerCount { get; set; }

    }
}