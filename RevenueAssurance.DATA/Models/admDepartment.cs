

namespace RevAssuranceWebAPi.AnythingGood.DATA.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class admDepartment
    {
        [Key]
        public int DeptId { get; set; }
        public string Deptname { get; set; }
        public int UserId { get; set; }
        public DateTime? DateCreated { get; set; }
        public string Status { get; set; }

    }
}
