namespace RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO
{
    public class DashboardCountDTO
    {
        public int MenuId { get; set; }
        public string DashboardMenuName  { get; set; }
        public string CanView  { get; set; }
        public string ParentId  { get; set; }
        public int? DashBoardValues{ get; set; }
        public string  Icon { get; set; }
        public string  IconColor{ get; set; }
        public string Query { get; set; }
        public string RouterLink { get; set; }
        }
}