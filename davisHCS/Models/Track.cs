using System;
using System.Collections.Generic;

#nullable disable

namespace davisHCS.Models
{
    public partial class Track
    {
        public Track()
        {
            MemberTracks = new HashSet<MemberTrack>();
            TrackChanges = new HashSet<TrackChange>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public DateTime? CreationDt { get; set; }

        public virtual ICollection<MemberTrack> MemberTracks { get; set; }
        public virtual ICollection<TrackChange> TrackChanges { get; set; }
    }
}
