using System;
using System.ComponentModel.DataAnnotations;

namespace anythingGoodApi.AnythingGood.DATA.Models
{
    public class VehicleManufacturer
    {
        [Key]
        public int? AId { get; set;}
        public string ManaufacturerName { get; set; }
        public DateTime DateCreated { get; set;}
        public int service_id { get; set; }
    }
}