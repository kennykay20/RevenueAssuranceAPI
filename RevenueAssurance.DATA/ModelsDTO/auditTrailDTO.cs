using System;

namespace RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO
{
    public class auditTrailDTO
    {
        public int? ItbId { get; set; }
        public string UserFullName { get; set; }
        public DateTime? Eventdateutc { get; set; }
        public string Eventtype { get; set; }
        public string Tablename { get; set; }
        public string Recordid { get; set; }
        public string Originalvalue { get; set; }
        public string Newvalue { get; set; }
        public string ColumnName { get; set; }
    }

}