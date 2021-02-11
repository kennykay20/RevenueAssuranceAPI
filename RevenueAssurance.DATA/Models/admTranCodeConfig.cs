

namespace RevAssuranceWebAPi.AnythingGood.DATA.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class admTranCodeConfig
    {
         [Key]
        public int ItbId { get; set; }
        public string TranCode { get; set; }
        public string Description { get; set; }
        public string DebitCredit { get; set; }
        public int? ServiceId { get; set; }
        public DateTime? DateCreated { get; set; }
        public string IsSalary { get; set; }
        public int? UserId { get; set; }
        public string Status { get; set; }
    }
}
