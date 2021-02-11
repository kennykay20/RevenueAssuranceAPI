

namespace RevAssuranceWebAPi.AnythingGood.DATA.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class admIssuranceCoverType
    {
         [Key]
        public int ItbId { get; set; }
        public string CoverName { get; set; }
        public string Status { get; set; }
        public DateTime? DateCreated { get; set; }
        public int? UserId { get; set; }
    }
}
