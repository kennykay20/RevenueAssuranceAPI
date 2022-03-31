using System;

namespace RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO
{

    public class admBroadCastDTO
    {
       public int? ItbId { get; set; }
        public string TargetAudience { get; set; }
        public int? DeptId { get; set; }
        public string Subject {get; set;}
        public string Message { get; set; }
        public string BroadcastType { get; set; }
        public int? UserId { get; set; }
        public DateTime? DateCreated { get; set; }
        public string Status { get; set; }

        public DateTime?  StartDate { get; set; }
         public DateTime? EndDate { get; set; }

        public string NotifyTimeInterval { get; set; }
        public string DeptName { get; set;}

      
    }


}