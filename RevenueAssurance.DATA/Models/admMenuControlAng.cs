

namespace RevAssuranceWebAPi.AnythingGood.DATA.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class admMenuControl
    {
         [Key]
        public int MenuId { get; set; }
        public string MenuName { get; set; }
        public string RouterLink { get; set; }
        public string RouteAddress { get; set; }
        public string Icon { get; set; }
        public string IconAddress { get; set; }
        public bool IsParent { get; set; }
        public int ParentId { get; set; }
        public string status { get; set; }
    }
}
