using System.Collections.Generic;

namespace RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO
{
    public class GenRoleAssignDTO
    {
        public List<RoleAssignDTO> ListRoleAssignDTO{ get; set;}
        public int? UserId {get; set; }
        public int? RoleId { get; set; }
    }
}