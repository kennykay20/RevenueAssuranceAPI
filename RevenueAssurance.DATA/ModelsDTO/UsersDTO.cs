using RevAssuranceWebAPi.AnythingGood.DATA.Models;

namespace RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO
{
    public class UsersDTO
    {
        public admUserProfile admUserProfile {get; set;}
        public int? LoginUserId {get; set;}
        public int? CreatedBy {get; set;}
    }
}