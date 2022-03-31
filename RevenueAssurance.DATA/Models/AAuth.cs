using System;
using System.ComponentModel.DataAnnotations;

namespace RevAssuranceWebAPi.AnythingGood.DATA.Models
{
    public class AAuth
    {
        [Key]
        public decimal AId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public DateTime? DateCreated { get; set; }

        public int? UserId { get; set; }
    }



}