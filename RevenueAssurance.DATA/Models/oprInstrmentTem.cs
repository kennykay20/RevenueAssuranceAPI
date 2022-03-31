

namespace RevAssuranceWebAPi.AnythingGood.DATA.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class oprInstrmentTemp
    {
         [Key]
        public Int64 ItbId { get; set; }
        public int? ServiceId { get; set; }
        public Int64? ServiceItbId { get; set; }
        public int UserId { get; set; }
        public DateTime? DateCreated { get; set; }
        public string TemplateContent { get; set; }
    }
}
