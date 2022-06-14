using System;
using System.Collections.Generic;

#nullable disable

namespace davisHCS.Models
{
    public partial class VwEligMember
    {
        public int SubId { get; set; }
        public int MemId { get; set; }
        public DateTime? Fromdt { get; set; }
        public DateTime? Throughdt { get; set; }
    }
}
