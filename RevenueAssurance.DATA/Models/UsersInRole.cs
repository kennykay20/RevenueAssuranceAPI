

namespace RevAssuranceWebAPi.AnythingGood.DATA.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class UsersInRole
    {
        [Key]
        public System.Guid UserId { get; set; }
        public System.Guid RoleId { get; set; }
    }
}
