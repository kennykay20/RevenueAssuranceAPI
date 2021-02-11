namespace RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO
{
    public class TranEnqParam
    {
        public string pnUserId { get; set; } 
        public string psBranchCode { get; set; }
        public string psCurrencyIso{ get; set; }
        public string psDrAcctNo{ get; set; }
        public string psCrAcctNo{ get; set; }
        public string pdtStartDate{ get; set; }
        public string pdtEndDate { get; set; }
        public string psTranRef { get; set; }
        public string psStatus{ get; set; }
        public string pnAmnt { get; set; }
        public string pnServiceId { get; set; }
        public string pnDeptId	{ get; set; }
        public string pnbatchId	{ get; set; }
    }
}