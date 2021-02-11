namespace RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO
{
    public class EndDateDTO
    {
        public string EffectiveDate {get; set;}   
        public string  TenorPeriod  {get; set;}
        public string  TimeBasis  {get; set;}
        public int? ServiceId {get; set;}
        public int? TemplateId {get; set;}
        public long ServiceItbId {get; set;}
        
    }
}