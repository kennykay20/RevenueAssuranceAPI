

namespace RevAssuranceWebAPi.AnythingGood.DATA.Models
{
    using System;
    using System.Collections.Generic;

    public class User
    {
        public System.Guid ApplicationId { get; set; }
        public System.Guid UserId { get; set; }
        public string UserName { get; set; }
        public bool IsAnonymous { get; set; }
        public DateTime LastActivityDate { get; set; }

        // public virtual Membership Membership { get; set; }
    }
}
