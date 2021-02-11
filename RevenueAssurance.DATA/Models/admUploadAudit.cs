

namespace RevAssuranceWebAPi.AnythingGood.DATA.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class admUploadAudit
    {
         [Key]
        public int ItbId { get; set; }
        public int? ServiceId { get; set; }
        public string FileName { get; set; }
        public int? UserId { get; set; }
        public DateTime? DateCreated { get; set; }
        public string Status { get; set; }
    }
}
