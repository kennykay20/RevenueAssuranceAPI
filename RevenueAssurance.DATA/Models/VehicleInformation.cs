using System;
using System.ComponentModel.DataAnnotations;

namespace anythingGoodApi.AnythingGood.DATA.Models
{
    public class VehicleInformation
    {
        [Key]
        public int AId {get; set;}
        public int VehicleManufacturer_AId  {get; set;}
        public int Vehicle_Model_Id  {get; set;}
        public string EngineType  {get; set;}
        public string FuelType  {get; set;}
        public string Transmission  {get; set;}
        public string Mileage  {get; set;}
        public string EngineCapacity  {get; set;}
        public string CustomPaper  {get; set;}
        public string Colour  {get; set;}
        public string Colour_Interior {get; set;}
        public string DriveType {get; set;}
        public string DiagnosticReport  {get; set;}
        public string Airbag  {get; set;}
        public decimal Price {get; set;}
        public long CreatedBy  {get; set;}
        public string Car_Type {get; set;}
        public DateTime  DateCreated {get; set;}
        public int Service_Id {get; set;}
        public string Year {get; set;}
        public string Product_Url  {get; set;}
        public long UserId  {get; set;}
    }
}