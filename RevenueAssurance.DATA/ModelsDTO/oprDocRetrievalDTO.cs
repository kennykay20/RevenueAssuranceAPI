using System;

namespace RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO
{
    public class oprDocRetrievalDTO
    {
        public Int64? ItbId				  {get; set; }								
        public int? ServiceId			  {get; set; }
        public int? OrigDeptId		  {get; set; }
        public string ReferenceNo		  {get; set; }
        public int? BranchNo			  {get; set; }
        public string AcctNo			  {get; set; }
        public string AcctType			  {get; set; }
        public string AcctName			  {get; set; }
        public decimal? AvailBal			  {get; set; }
        public string AcctSic			  {get; set; }
        public string AcctStatus		  {get; set; }
        public string CcyCode			  {get; set; }
        public string ProductCode		  {get; set; }
        public int? RsmId				  {get; set; }
        public string ServiceStatus		  {get; set; }
        public string Status			  {get; set; }
        public string OriginatingBranchId {get; set; }
        public int? ProcessingDeptId	  {get; set; }
        public DateTime? TransactionDate	  {get; set; }
        public DateTime? ValueDate			  {get; set; }
        public DateTime? DateCreated		  {get; set; }
        public int? UserId			  {get; set; }
        public int? SupervisorId		  {get; set; }
        public int? DismissedBy		  {get; set; }
        public DateTime? DismissedDate		  {get; set; }
         public int? RejectedBy		  {get; set; }
        public string Rejected			  {get; set; }
        public string RejectionIds		  {get; set; }
        public DateTime? RejectionDate		  {get; set; }
        public int? PrintCount		  {get; set; }
        public string  BranchName {get; set; } 
        public string  UserName {get; set; }
        public string  DeptName  {get; set; }
    }


}