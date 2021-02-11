

namespace RevAssuranceWebAPi.AnythingGood.DATA.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class admIndustrySecMapg
    {
         [Key]
        public int ItbId { get; set; }
        public string SicCode { get; set; }
        public string SectorName { get; set; }
        public string ParentSectorCode { get; set; }
        public string Status { get; set; }
        public int UserId { get; set; }
        public DateTime? DateCreated { get; set; }
        public string InwardParentSectorCode { get; set; }
    }
}
