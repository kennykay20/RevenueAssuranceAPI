

namespace RevAssuranceWebAPi.AnythingGood.DATA.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class admChannel
    {
         [Key]
        public int ItbId { get; set; }
        public int channelId { get; set; }
        public string ChannelName { get; set; }
        public int UserId { get; set; }
        public DateTime? DateCreated { get; set; }
        public string Status { get; set; }
    }
}
