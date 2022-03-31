using System;
using System.ComponentModel.DataAnnotations;

namespace anythingGoodApi.AnythingGood.DATA.Models
{
    public class Service
    {
        [Key]
        public int AId { get; set;}
        public string ServiceName { get; set;}
        public DateTime DateCreated { get; set;}
        public string Status { get; set;}
    }
}