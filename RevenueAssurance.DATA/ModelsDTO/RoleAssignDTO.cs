namespace RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO
{
    public class RoleAssignDTO
    {
  
        public int MenuId {get; set; }
        public string MenuName {get; set; }
        public string DashboardMenuName {get; set; }
        public bool? CanAdd { get; set; }
        public bool? CanAuth{get; set; }
        public bool? CanDelete{get; set; }
        public bool? CanEdit{get; set; }
        public bool? CanView {get; set; }
        public bool? IsGlobalSupervisor {get; set; }
        public int? RoleId {get; set; }
         public int? UserId {get; set; }
         public int? ParentId {get; set;}
        public string DashBaordTable { get; set; } 
        public string DashBoardValues { get; set; }
         public string Icon { get; set; } 
         public string IconColor { get; set; }  
         public string Query { get; set; }   
    }
}