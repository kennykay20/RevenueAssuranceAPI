using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace anythingGoodApi.AnythingGood.DATA.Models
{
    public class Price
    {
         [Key]
        public  string AId { get; set; }
        public  decimal Price1 { get; set; }
        public  int CreatedBy { get; set; }
        public  DateTime DateCreated { get; set; }
        public  string PriceRange { get; set; }
        public int service_id {get; set;}
        public  int Priority{ get; set; }
    }
}