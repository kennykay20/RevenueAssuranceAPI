using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RevAssuranceWebAPi.AnythingGood.DATA.Models
{
    public class admClientProfile
    {
         [Key]
        public string BankCode { get; set; }
        public string BankName { get; set; }
        public string BankAddress { get; set; }
        public string ChannelId { get; set; }

        public DateTime? CurrentProcessingDate { get; set; }
        public int EnforcePasswordChangeDays { get; set; }
        public bool? EnforceStrngPwd { get; set; }
        public string BankingSystem { get; set; }

        public bool? LoginIdEncryption { get; set; }
        public int? SystemIdleTimeout { get; set; }
        public string CountryCode { get; set; }
        public string CurrencyCode { get; set; }
        public int? UserId { get; set; }
        public DateTime? DateCreated { get; set; }
        public string Status { get; set; }
        public int? LoginCount { get; set; }
        public string LicenceKey { get; set; }
        public bool? UseCBSAuth { get; set; }

    }


}
