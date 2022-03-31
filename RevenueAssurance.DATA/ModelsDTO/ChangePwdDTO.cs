namespace RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO
{
    public class ChangePwdDTO
    {
        public string LoginId { get; set;}
         public string OldPassword { get; set;}
        public string NewPassword { get; set;}
        public string ConfirmPassword { get; set;}
    }
}