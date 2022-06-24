using System;
using System.Collections.Generic;

#nullable disable

namespace davisHCS.Models
{
    public partial class MemberTrack
    {
        public int Id { get; set; }
        public int TrackId { get; set; }
        public int MemberId { get; set; }
        public int Seq { get; set; }
        public int? NextSeq { get; set; }
        public DateTime? EffectiveDt { get; set; }
        public int? Valid { get; set; }
        public int? TrackDataInt { get; set; }
        public string TrackDataChar { get; set; }
        public DateTime CreationDt { get; set; }

        public virtual Member Member { get; set; }
        public virtual Track Track { get; set; }
    }
}
