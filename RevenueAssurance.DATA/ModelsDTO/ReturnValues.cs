using System;

namespace RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO
{
     public class ReturnValues
    {
        //
        public string refreshPage { set; get; }
        public string StringbuilderMessage { set; get; }
        //

        public string FirstInstlmtDate { set; get; }
        public string NextInstlmtDate { set; get; }
        public string FinalInstlmtDate { set; get; }
        public string expiryDate { set; get; }

        public int? nErrorCode { set; get; }
        public string ReferenceNo { set; get; }
        public string sErrorText { set; get; }
        public string InstlmtAmount { set; get; }
        public int? NoInstalmt { set; get; }
        public int? succesful { set; get; }
        public decimal? MfeeItbid { set; get; }
        public decimal? FaciFeeItbid { set; get; }
        public int? Userid { set; get; }
        public string dealId { set; get; }
        public int Itbid { set; get; }
        public decimal? CbsItbid { set; get; }
        public string Url { set; get; }
        public string Email { get; set; }
        public string Fullname { get; set; }
        public string LastRunDate { set; get; }
        public string NextRunDate { set; get; }
        public int? companycode { set; get; }
        public int? classcode { set; get; }
        public int? RoleID { set; get; }
        public string enforcepwdchnge { set; get; }
        public bool? forcePasswordChange { get; set; }
        public string fullname { set; get; }
        public string balance { set; get; }
        public string difference { set; get; }
        public string Filename { get; set; }
         public string EndChqNo { get; set; }
        public string ccyCode { set; get; }
        public string chargeCode { set; get; }
        public string chrgAmt { get; set; }
        public string taxAmt { get; set; }
        public string template { get; set; }
        public string acctStatus { set; get; }
        public string startDate { set; get; }
        public string iRate { set; get; }
        public string loginId { get; set; }
        public string LastLoginDate { get; set; }
        public DateTime? logInTime { get; set; }
        public string imagepath { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string UserKey { get; set; }
        public string PasswordKey { get; set; }
        public int? ServiceId { get; set; }
        public int? BatchNo { get; set; }
        public string tranCode { get; set; }
        public string RecordCount { get; set; }
        public int? count { get; set; }
        public string reference { get; set; }

        public string RecordCountDr { get; set; }
        public string RecordCountCr { get; set; }
        public string TotalDr { get; set; }
        public string TotalCr { get; set; }
        public string LoadedBy { get; set; }
        public string IsBalanced { get; set; }
        public string VerifiedBy { get; set; }
        public string ApprovedBy { get; set; }
        public string Department { get; set; }
        public string BranchName { get; set; }
        public string DateCreated { get; set; }
        public string BatchStatus { get; set; }
        public string ServiceName { get; set; }
    }

}