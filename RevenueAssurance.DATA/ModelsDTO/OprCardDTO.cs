using System;

namespace RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO
{
    public class OprCardDTO
    {
       public int  ItbId { get; set; }
	   public string ReferenceNo { get; set; }
	   public string ServiceStatus { get; set; }
	   public string AcctNo { get; set; }
	   public string AcctType { get; set; }
       public string AvailBal { get; set; }
	  public string AcctStatus { get; set; }
	   public string AcctName { get; set; }
       public string BranchName  { get; set; }
	   public string SerialNo  { get; set; }
       public string RecordDate   { get; set; }
	   public string CcyCode  { get; set; }
       public DateTime? TransactionDate	{ get ; set; }
        public  DateTime? ValueDate {get; set;}
        public string UserName  { get; set; }
        public string ChgAcctNo  { get; set; }
        public int UserId { get; set; }
        public string Datecreated  { get; set; } 
        public string ServiceId  { get; set; } 
        public bool Select  { get; set; } = false;
        public int RsmId { get; set; }
        //
        public string Status { get; set; }
        public string OriginatingBranchId { get; set; }
        public string ProcessingDeptId { get; set; }

        public string DateCreated { get; set; }
        public string SupervisorId { get; set; }
        public string ValAcctError { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMsg { get; set; }
        public string DismissedBy { get; set; }
        public string DismissedDate { get; set; }
        public string Rejected { get; set; }
        public string RejectionIds { get; set; }
        public string RejectionDate { get; set; }
        public string RejectedBy { get; set; }
    }
}