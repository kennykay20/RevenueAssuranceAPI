using System;
using System.ComponentModel.DataAnnotations;

namespace anythingGoodApi.AnythingGood.DATA.Models
{
    public class UploadVehicle
    {
        [Key]
        public string AId	{ get; set; }
        public string VehicleInfoId	{ get; set; }
        public string ImagePath	{ get; set; }
        public DateTime DateCreated	{ get; set; }
        public string Status	{ get; set; }
    }
}