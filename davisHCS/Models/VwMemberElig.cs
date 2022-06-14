using System;
using System.Collections.Generic;

#nullable disable

namespace davisHCS.Models
{
    public partial class VwMemberElig
    {
        public int MemId { get; set; }
        public DateTime? Effectivedt { get; set; }
        public string EligStatus { get; set; }
    }
}
