

namespace RevAssuranceWebAPi.AnythingGood.DATA.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    public class admBroadCastDetail
    {
        [Key]
        public int ItbId { get; set; }
        public int UserId { get; set; }
        public int BroadCastId { get; set; }
        public DateTime? DateCreated { get; set; }
        public string Status { get; set; }
    }
}
