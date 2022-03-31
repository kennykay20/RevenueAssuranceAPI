using System;
using System.ComponentModel.DataAnnotations;

namespace RevAssuranceApi.AnythingGood.DATA.Models
{

    public class admApplicationVersion
    {
        [Key]
        public int ItbId { get; set; }
        [Required]
        public string Version { get; set; }
        public string Password { get; set; }
        public DateTime? VersionUpdatedDate { get; set; }
    }
}