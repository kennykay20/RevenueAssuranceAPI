

namespace RevAssuranceWebAPi.AnythingGood.DATA.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class admStatementSetUp
    {
         [Key]
        public int Id { get; set; }
        public string AcStmtHeader { get; set; }
        public string AcStmtBody { get; set; }
        public string AcStmtFooter { get; set; }
        public int? RowPage { get; set; }
        public int? MaxCharacterBreak { get; set; }
    }
}
