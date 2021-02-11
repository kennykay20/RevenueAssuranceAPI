using System;
using System.ComponentModel.DataAnnotations;

namespace RevAssuranceApi.RevenueAssurance.DATA.Models
{
    public class admAmendReprintReason
    {
        [Key]
        public int ReasonId		{ get; set ; }
        public string ReasonType	{ get; set ; }
        public string Reason		{ get; set ; }
        public bool Chargeable	{ get; set ; }
        public int UserId		{ get; set ; }
        public string Status		{ get; set ; }
        public DateTime DateCreated	{ get; set ; }
    }
}