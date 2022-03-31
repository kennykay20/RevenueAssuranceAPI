

namespace RevAssuranceWebAPi.AnythingGood.DATA.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class admLicenseSetUpHistory
    {
         [Key]
        public int Itbid { get; set; }
        public string LincenseKey { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? UserId { get; set; }
        public string Status { get; set; }
        public DateTime? DateCreated { get; set; }
    }
}
