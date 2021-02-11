using System;

namespace RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO
{
    public class admTemplateDTO
    {
        public int? ItbId { get; set; }
        public string TemplateName { get; set; }
        public int? ServiceId { get; set; }
        public string TemplateCode { get; set; }
        public string TemplateContent { get; set; }
        public DateTime? DateCreated { get; set; }
        public int? UserId { get; set; }
        public string Status { get; set; }

        public string ServiceDescription {get; set;}
    }
}