using System;
using System.ComponentModel.DataAnnotations;

namespace RevAssuranceApi.RevenueAssurance.DATA.Models
{
    public class admPinItems
    {
        [Key]
        public int? PinId		      {get; set;}
        public string Description	  {get; set;}
        public string Status		  {get; set;}
        public int? UserId		  {get; set;}
        public DateTime? DateCreated	  {get; set;}
    }

    public class oprPinItemsTemporal {
        public int? PinId		      {get; set;}
        public string Description	  {get; set;}
        public string Status		  {get; set;}
        public int? UserId		  {get; set;}
        public DateTime? DateCreated	  {get; set;}
        public bool Select {get; set;}
    }

}