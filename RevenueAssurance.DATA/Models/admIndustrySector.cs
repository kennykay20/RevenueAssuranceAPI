

namespace RevAssuranceWebAPi.AnythingGood.DATA.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class admIndustrySector
    {
         [Key]
        public int ItbId { get; set; }
        public string SectorCode { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public int UserId { get; set; }
        public DateTime? DateCreated { get; set; }
    }
}
