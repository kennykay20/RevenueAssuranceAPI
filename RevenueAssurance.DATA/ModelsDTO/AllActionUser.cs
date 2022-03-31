namespace RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO
{
    public class AllActionUser
    {
        public string CreatedBy { get; set;}
        public string RejectedBy { get; set;}
        public string RejectedDate  { get; set;}
        public string DismissedBy { get; set;}
        public string DismissedDate { get; set;}
        public string RejectionReasons  { get; set;}
        public string Supervisor  { get; set;}
        public string OriginBranch  { get; set;}
    }
}