namespace RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO
{
    public class ReportParam
    {
        public int? pnUserId { get; set; } 
        public string psBranchCode { get; set; }
        public string psCurrencyIso{ get; set; }
        public string psDrAcctNo{ get; set; }
        public string psCrAcctNo{ get; set; }
        public string pdtStartDate{ get; set; }
        public string pdtEndDate { get; set; }
        public string psTranRef { get; set; }
        public string psStatus{ get; set; }
        public string TotalAmt { get; set; }
        public string pnServiceId { get; set; }
        public string pnDeptId	{ get; set; }
        public string pnbatchId	{ get; set; }
         public bool psIsGlobalSupervisor	{ get; set; }
         public string Tenure { get; set; }
         public string StartDate { get; set; }
         public string TimeBasis { get; set; }
         public string TotalAmount { get; set; }
         public string originBranch {get; set;}
         public int? MenuId { get; set;}
         public int? RoleId { get; set;}
         public string psAcctNo { get; set;}
         public string pnAmnt { get; set;}
         public string EffectiveDate { get; set;}
         public string ExpiryDate { get; set;}
         public string NextRunDate { get; set;}
        public string Branch {get; set;}
        
        

    }

    public class auditTables
    {
        public string tableName { get; set; }
    }
}