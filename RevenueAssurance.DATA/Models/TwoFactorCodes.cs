using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RevAssuranceWebAPi.AnythingGood.DATA.Models
{
    public class TwoFactorCodes
    {
        [Key]
        public int ItbId { get; set; }
        public string OneTimeCode { get; set; }
        public bool RememberDevice { get; set; }
        public string SelectedProvider { get; set; }
        public int UserId { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string EncryptionKey2Fa { get; set; }
        public string Token { get; set; }
        public int? Attempts { get; set; }
        public string IpAddress { get; set; }
        public bool CodeExpired { get; set; }
        public bool CodeIsUsed { get; set; }
        public string EncryptionKeyForDeviceId { get; set; }

    }


}