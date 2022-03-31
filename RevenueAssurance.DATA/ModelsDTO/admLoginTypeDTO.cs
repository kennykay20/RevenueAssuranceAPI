

using System;

namespace RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO
{
    public class admLoginTypeDTO
    {
        public int Itbid { get; set; }
        public string LoginType { get; set; }
        public string Description { get; set; }
        public int? UserId { get; set; }
        public DateTime? DateCreated { get; set; }
    }
}