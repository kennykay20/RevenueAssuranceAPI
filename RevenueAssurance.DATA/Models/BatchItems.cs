using System;
using System.ComponentModel.DataAnnotations;

namespace RevAssuranceApi.RevenueAssurance.DATA.Models
{
    public class BatchItems
    {
		[Key] 
		public long? ItbId	{ get; set; }		
		public int? ServiceId	{ get; set; }		
		public long BatchNo	{ get; set; }		
		public string BranchNo	{ get; set; }		
		public string AcctNo {  get; set; }		
		public string AcctType { get; set; }		
		public string AcctName	{ get; set; }		
		public decimal? AcctBalance	{ get; set; }		
		public string AcctStatus { get; set; }		
		public string CbsTC	{ get; set; }		
		public long? ChequeNo { get; set; }		
		public string CcyCode	{ get; set; }		
		public decimal? Amount	{ get; set; }		
		public string DrCr	{ get; set; }		
		public string Narration	{ get; set; }		
		public string ChargeCode	{ get; set; }		
		public decimal? ChargeAmount	{ get; set; }		
		public string ChgNarration	{ get; set; }		
		public string TaxAcctNo	{ get; set; }		
		public string TaxAcctType	{ get; set; }		
		public decimal? TaxRate	{ get; set; }		
		public decimal? TaxAmount	{ get; set; }		
		public string TaxNarration	{ get; set; }		
		public int? IncBranch	{ get; set; }		
		public string IncAcctNo	{ get; set; }		
		public string IncAcctType	{ get; set; }		
		public string IncAcctName	{ get; set; }		
		public string IncAcctBalance	{ get; set; }		
		public string IncAcctStatus	{ get; set; }		
		public string IncAcctNarr	{ get; set; }		
		public string ClassCode		{ get; set; }		
		public DateTime? OpeningDate { get; set; }		
		public string IndusSector	{ get; set; }		
		public string CustType	{ get; set; }		
		public int? CustNo		{ get; set; }		
		public int? RsmId	{ get; set; }		
		public decimal? CashBalance	{ get; set; }		
		public decimal? CashAmt	{ get; set; }		
		public string City { get; set; }		
		public int? ValUserId { get; set; }		
		public int? ValErrorCode	{ get; set; }		
		public string ValErrorMsg	{ get; set; }		
		public DateTime? TransactionDate		{ get; set; }		
		public DateTime? ValueDate	{ get; set; }			
		public DateTime? DateCreated { get; set; }					
		public string ServiceStatus	{ get; set; }		
		public string Status { get; set; }			
		public int? DeptId { get; set; }						
		public int? ProcessingDept	{ get; set; }							
		public int? BatchSeqNo	{ get; set; }							
		public int? UserId { get; set; }						
		public int? SupervisorId { get; set; }				
		public int? Direction	{ get; set; }								
		public string OriginatingBranchId { get; set; }		 		
		public int? DismissedBy	{ get; set; }								
		public DateTime? DismissedDate	{ get; set; }				
		public string Rejected	{ get; set; }				
		public string RejectionIds { get; set; }				
		public DateTime? RejectionDate { get; set; }	
		public string ReferenceNo { get; set; }		
    }
}