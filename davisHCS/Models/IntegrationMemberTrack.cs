using System;
using System.Collections.Generic;

#nullable disable

namespace davisHCS.Models
{
    public partial class IntegrationMemberTrack
    {
        public int Id { get; set; }
        public int IntegrationActivityId { get; set; }
        public int MemberTrackId { get; set; }
        public DateTime? CreationDt { get; set; }

        public virtual IntegrationActivity IntegrationActivity { get; set; }
        public virtual MemberTrack MemberTrack { get; set; }
    }
}
