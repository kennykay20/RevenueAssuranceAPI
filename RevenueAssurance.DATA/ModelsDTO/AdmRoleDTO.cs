using System;
using RevAssuranceWebAPi.AnythingGood.DATA.Models;

namespace RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO
{
    public class AdmRoleDTO
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
        public int? UserId { get; set; }
        public string Status { get; set; }
        public DateTime? DateCreated { get; set; }
        public decimal? CreditLimit { get; set; }
        public decimal? DebitLimit { get; set; }
        public decimal? GLDebitLimit { get; set; }
        public decimal? GLCreditLimit { get; set; }

        public admRole admRole {get; set;}
        public int? LoginUserId { get; set;}
    }

}