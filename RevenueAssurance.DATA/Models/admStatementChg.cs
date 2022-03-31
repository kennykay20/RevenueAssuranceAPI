using System;
using System.ComponentModel.DataAnnotations;

namespace RevAssuranceApi.RevenueAssurance.DATA.Models
{
    public class admStatementChg
    { 
        [Key]
        public int? DocumentId		{ get; set; }
        public int? ServiceId			{ get; set; }
        public string Description		{ get; set; }
        public string ChgMetrix			{ get; set; }
        public string ChgBasis			{ get; set; }
        public int? StartingNo		{ get; set; }
        public int? EndingNo			{ get; set; }
        public decimal? CghAmount			{ get; set; }
        public string CcyCode			{ get; set; }
        public int? TemplateId		{ get; set; }
        public string Status			{ get; set; }
        public int? UserId			{ get; set; }
        public DateTime? DateCreated		{ get; set; }
    }
}