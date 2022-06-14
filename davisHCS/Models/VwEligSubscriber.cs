using System;
using System.Collections.Generic;

#nullable disable

namespace davisHCS.Models
{
    public partial class VwEligSubscriber
    {
        public int MemId { get; set; }
        public DateTime? Fromdt { get; set; }
        public DateTime? Throughdt { get; set; }
        public string EligStatus { get; set; }
    }
}
