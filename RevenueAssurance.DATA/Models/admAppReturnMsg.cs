

namespace RevAssuranceWebAPi.AnythingGood.DATA.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class admErrorMsg
    {
         [Key]
        public int ItbId { get; set; }
        public int ErrorId { get; set; }
        public string ErrorText { get; set; }
        public int? ModuleId { get; set; }

       // public virtual admAppReturnMsgSetUp admAppReturnMsgSetUp { get; set; }
    }
}
