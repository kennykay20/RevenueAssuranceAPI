

namespace RevAssuranceWebAPi.AnythingGood.DATA.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class admCollateralType
    {
         [Key]
        public int CollTypeId { get; set; }
        public string CollateralName { get; set; }
        public int UserId { get; set; }
        public string Status { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
