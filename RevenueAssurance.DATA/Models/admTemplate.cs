

namespace RevAssuranceWebAPi.AnythingGood.DATA.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class admTemplate
    {
         [Key]
        public int ItbId { get; set; }
        public string TemplateName { get; set; }
        public int? ServiceId { get; set; }
        public string TemplateCode { get; set; }
        public string TemplateContent { get; set; }
        public DateTime? DateCreated { get; set; }
        public int? UserId { get; set; }
        public string Status { get; set; }
        public int? RecordPerPage { get; set; } 
        public int? ParentId { get; set; } 
        public string ImageUrl { get; set; } 
    }
}
