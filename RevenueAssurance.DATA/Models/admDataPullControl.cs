

namespace RevAssuranceWebAPi.AnythingGood.DATA.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class admDataPullControl
    {
         [Key]
        public int ItbId { get; set; }
        public int? ServiceId { get; set; }
        public int? RecordCount { get; set; }
        public DateTime? LastTransDate { get; set; }
        public string LastTransId { get; set; }
        public DateTime? DateCreated { get; set; }
        public int? UserId { get; set; }
        public string Status { get; set; }
    }
}
