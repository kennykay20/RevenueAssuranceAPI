namespace RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO
{
    public class ValidateUserResponse
    {
           public int nErrorCode { get; set; }
            public string sErrorText { get; set; }
            public string EmployeeId { get; set; }
            public string UserName { get; set; }
            public string Email { get; set; }
            public string FullName { get; set; }
            public string UserStatus { get; set; }
            public string BranchNo { get; set; }
            public string LoginId { get; set; }
    }
}