
using System;
using System.ComponentModel.DataAnnotations;

namespace RevAssuranceApi.RevenueAssurance.DATA.Models
{

    public class admLoginType
    {
        [Key]
        public int Itbid { get; set; }
        public string LoginType { get; set; }
        public string Description { get; set; }
        public int? UserId { get; set; }
        public DateTime? DateCreated { get; set; }
        public string Status { get; set; }
    }

}