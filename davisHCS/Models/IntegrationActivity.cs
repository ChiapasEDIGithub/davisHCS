using System;
using System.Collections.Generic;

#nullable disable

namespace davisHCS.Models
{
    public partial class IntegrationActivity
    {
        public IntegrationActivity()
        {
            IntegrationMemberTracks = new HashSet<IntegrationMemberTrack>();
            Members = new HashSet<Member>();
            TrackChanges = new HashSet<TrackChange>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string FileSource { get; set; }
        public string ProcessSource { get; set; }
        public DateTime? ScheduleDt { get; set; }
        public DateTime? CreationDt { get; set; }

        public virtual ICollection<IntegrationMemberTrack> IntegrationMemberTracks { get; set; }
        public virtual ICollection<Member> Members { get; set; }
        public virtual ICollection<TrackChange> TrackChanges { get; set; }
    }
}
