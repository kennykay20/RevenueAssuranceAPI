

namespace RevAssuranceWebAPi.AnythingGood.DATA.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class FXRate
    {
         [Key]
        public int ItbId { get; set; }
        public string MajorCcy { get; set; }
        public string MinorCcy { get; set; }
        public decimal? BuyRate { get; set; }
        public decimal? CBuyRate { get; set; }
        public decimal? SellRate { get; set; }
        public decimal? CSellRate { get; set; }
        public decimal? MidRate { get; set; }
        public decimal? CrossRate { get; set; }
        public decimal? CMidRate { get; set; }
        public decimal? LclRate { get; set; }
        public decimal? SellLowerLimit { get; set; }
        public decimal? SellUpperLimit { get; set; }
        public decimal? BuyLowerLimit { get; set; }
        public decimal? BuyUpperLimit { get; set; }
        public decimal? CrossUpperLimit { get; set; }
        public decimal? CrossLowerLimit { get; set; }
        public DateTime DateCreated { get; set; }
        public int UserId { get; set; }
        public string Status { get; set; }
    }
}
