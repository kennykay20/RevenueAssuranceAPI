

namespace RevAssuranceWebAPi.AnythingGood.DATA.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class admAppReturnMsgSetUp
    {

        [Key]
        public int ItbId { get; set; }
        public string ModuleName { get; set; }
        public string MsgCodeRange { get; set; }

        // [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
      //   public virtual ICollection<admAppReturnMsg> admAppReturnMsgs { get; set; }
    }
}
