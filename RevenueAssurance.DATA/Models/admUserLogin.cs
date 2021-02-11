

namespace RevAssuranceWebAPi.AnythingGood.DATA.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class admUserLogin
    {
         [Key]
        public System.Guid RowGuid { get; set; }
        public int? UserId { get; set; }
        public DateTime? loginDate { get; set; }
        public DateTime? logoutDate { get; set; }
        public string ipAddress { get; set; }
        public string macAddress { get; set; }
        public string status { get; set; }
    }
}
