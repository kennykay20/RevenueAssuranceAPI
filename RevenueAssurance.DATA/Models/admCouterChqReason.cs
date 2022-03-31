

namespace RevAssuranceWebAPi.AnythingGood.DATA.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class admCouterChqReason
    {
         [Key]
        public int ItbId { get; set; }
        public string ReqReason { get; set; }
        public int? UserId { get; set; }
        public string Status { get; set; }
        public DateTime? DateCreated { get; set; }
        public string ChargeAble { get; set; }
    }
}
