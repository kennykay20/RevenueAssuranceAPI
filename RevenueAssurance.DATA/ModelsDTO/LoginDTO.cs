using System;
using System.ComponentModel.DataAnnotations;

namespace RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO
{
    public class LoginDTO
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Code { get; set; }
        public string loginType { get; set; }
        public string staffId { get; set; }
    }

    public class License
    {
        public string LicenseKey { get; set; }

    }

    public class VersionApplicationDTO
    {
        [Key]
        public int ItbId { get; set; }
        public string Version { get; set; }
        public string Password { get; set; }
        public DateTime? VersionUpdatedDate { get; set; }
    }
}