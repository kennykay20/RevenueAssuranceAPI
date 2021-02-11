

namespace RevAssuranceWebAPi.AnythingGood.DATA.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class admInstrumentCode
    {
         [Key]
        public int ItbId { get; set; }
        public string InstrumentCode { get; set; }
        public string Description { get; set; }
        public int? UserId { get; set; }
        public DateTime? DateCreated { get; set; }
        public int SettlementPeriod { get; set; }
        public string Status { get; set; }
        public int? ValidityPeriod { get; set; }
        public string Module { get; set; }
        public int? AuthorizeId { get; set; }
    }
}
