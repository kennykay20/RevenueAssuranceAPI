

namespace RevAssuranceWebAPi.AnythingGood.DATA.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class BatchControl
    {
         [Key]
        public long? BatchNo				{ get; set; }	
        public string Description { get; set; }
        public int? ServiceId				{ get; set; }
        public string CcyCode				{ get; set; }
        public int? PostedTransCount		{ get; set; } //
        public int? RecordCount			{ get; set; } //
        public int? TotalDrCount			{ get; set; } //
        public int? TotalCrCount			{ get; set; } //
        public decimal? TotalDr				{ get; set; }//
        public decimal? TotalCr				{ get; set; }//
        public decimal? TDifference			{ get; set; }//
        public int? LoadedBy				{ get; set; }//
        public int? Dept					{ get; set; }
        public int? OriginBranchNo		{ get; set; }
        public string IsBalanced			{ get; set; }
        public string VerifiedBy			{ get; set; }
        public int? ApprovedBy			{ get; set; }
        public int? PostedDrCount			{ get; set; }
        public int? PostedCrCount			{ get; set; }
        public string IsManual				{ get; set; }
        public string Status				{ get; set; }
        public string Filename				{ get; set; }
        public DateTime? DateCreated			{ get; set; }
        public DateTime? DateVerified			{ get; set; }
        public DateTime? DateApproved			{ get; set; }
        public int? ProcessingDept		{ get; set; }
        public string Rejected				{ get; set; }
        public int? RejectedBy			{ get; set; }
        public string RejectionReason		{ get; set; }
        public DateTime? RejectionDate			{ get; set; }
        public int? ClosedBy				{ get; set; }
        public DateTime? DateClosed			{ get; set; }
        public DateTime? PostingDate			{ get; set; }
        public string DefaultNar { get; set;}


        ////////


    }



}
