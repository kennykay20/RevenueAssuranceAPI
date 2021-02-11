using System;
using System.ComponentModel.DataAnnotations;

namespace anythingGoodApi.AnythingGood.DATA.Models
{
  
    public class AuditTrail
    {
        [Key]
        public int ItbId { get; set; }
        public int Userid { get; set; }
        public DateTime Eventdateutc { get; set; }
        public string Eventtype { get; set; }
        public string Tablename { get; set; }
        public string Recordid { get; set; }
        public string Originalvalue { get; set; }
        public string Newvalue { get; set; }
    }


}