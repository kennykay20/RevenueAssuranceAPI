using System;
using System.ComponentModel.DataAnnotations;

namespace anythingGoodApi.AnythingGood.DATA.Models
{
    public class Country
    {
        [Key]
        public decimal AId  {get; set;}  
        public string CountryName {get; set;}
        public string DateCreated {get; set;}
        public string CountryMobileCode {get; set;}
        public int CountryMobileLenght {get; set;}
        public string Status {get; set;}

    }
}