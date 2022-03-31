namespace RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO
{
    public class UserCoreBannkingDetailsDTO
    {
        public int?  nErrorCode {get; set;} = -1;
        public string sErrorText {get; set;}
        public long? EmployeeId {get; set;}
        public string UserName {get; set;}
        public string FullName {get; set;}
        public string UserStatus {get; set;}
        public string BranchNo {get; set;}
        public string Email {get; set;}
         public string  LoginId {get; set;}
         public string StaffId { get; set; }

    }
}