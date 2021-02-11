

namespace RevAssuranceWebAPi.AnythingGood.DATA.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class admRole
    {
         [Key]
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
        public int? UserId { get; set; }
        public string Status { get; set; }
        public DateTime? DateCreated { get; set; }
       
    }
}
