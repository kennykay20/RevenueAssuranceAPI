using System.Collections.Generic;

namespace RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO
{
    public class ResetUserDTO
    {
        public List<admUserProfileDTO> admUserProfileDTO { get; set; }
        public int? LoginUserId { get; set; }
    }
}