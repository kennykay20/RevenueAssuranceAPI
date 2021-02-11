using System;
using System.ComponentModel.DataAnnotations;

namespace anythingGoodApi.AnythingGood.DATA.Models
{
    public class VehicleModel
    {
        [Key]
        public int? AId { get; set;}
        public string VehicleModelName { get; set; }
        public int VehicleManufacturer_AId { get; set; }
        public DateTime DateCreated { get; set;}
        public int service_id {get; set;}
    }
}