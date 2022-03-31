

namespace RevAssuranceWebAPi.AnythingGood.DATA.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class admDashBoardAssignment {
         [Key]
        public int Itbid { get; set; }
        public int? RoleId { get; set; }
        public int? MenuId { get; set; }
        public bool? CanView { get; set; }
        public bool? CanAdd { get; set; }
        public bool? CanEdit { get; set; }
        public bool? CanAuth { get; set; }
        public bool? CanDelete { get; set; }
        public bool? IsGlobalSupervisor { get; set; }
    }
}
