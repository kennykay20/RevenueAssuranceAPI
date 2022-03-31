using System;

namespace RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO
{
    public class admAccountTypeDTO
    {
        
        public int? ItbId { get; set; }
        public string AccountTypeCode { get; set; }
        public string Description { get; set; }
        public string AccountFormat { get; set; }
        public int? UserId { get; set; }
        public DateTime? DateCreated { get; set; }
        public string Status { get; set; }
        public int? AcctLenght { get; set; }
        public string Delimeter { get; set; }
        public bool Hide { get; set;}
    }
}