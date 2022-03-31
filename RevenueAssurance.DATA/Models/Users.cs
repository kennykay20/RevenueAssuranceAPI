using System;
using System.ComponentModel.DataAnnotations;

namespace RevAssuranceWebAPi.AnythingGood.DATA.Models
{
    public class Users
    {
        [Key]
        public int AId {get; set;}
        public string UserName {get; set;}
        public string EmailAddress {get; set;}
        public string Password {get; set;}
        public string MobileNo {get; set;}
        public int Country_Id {get; set;}
        public string Address {get; set;}
        public string FirstName {get; set;}
        public string LastName {get; set;}
        public DateTime DateCreated {get; set;}
        public string IpRegion {get; set;}
        public int State_Id {get; set;}
        public string Service_Id  {get; set;}
        public string Status {get; set;}
        public string RetryPwdCount {get; set;}
        public string EncryptConEmail { get; set; }
        public DateTime TimeConExpires {get; set;}
        public string CompanyName {get; set;}
        public string MobileOrWeb{ get; set; }
        public bool EmailRegisSent {get; set;}
    }
}