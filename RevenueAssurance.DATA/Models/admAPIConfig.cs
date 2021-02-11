using System;
using System.ComponentModel.DataAnnotations;

namespace RevAssuranceApi.RevenueAssurance.DATA.Models
{
    public class admAPIConfig
    {
        [Key]
        public int? ItbId			{get; set; }
        public string FunctionName	{get; set; }
        public int? ConnectionId	{get; set; }
        public string Status		{get; set; }
        public int? UserId		{get; set; }
        public DateTime DateCreated	{get; set; }
    }
}