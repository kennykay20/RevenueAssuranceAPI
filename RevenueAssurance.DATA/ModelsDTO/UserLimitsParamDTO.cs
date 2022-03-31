using RevAssuranceWebAPi.AnythingGood.DATA.Models;

namespace RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO
{
    public class UserLimitsParamDTO
    {
        public admUserProfileDTO admUserProfileDTO { get; set;}
        public UserLimitDTO  UserLimitDTO { get; set;}

        public int?  LoginUserId { get; set;}
    }
}