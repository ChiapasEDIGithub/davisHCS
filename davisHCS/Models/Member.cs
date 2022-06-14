using System;
using System.Collections.Generic;

#nullable disable

namespace davisHCS.Models
{
    public partial class Member
    {
        public Member()
        {
            InverseRelationMember = new HashSet<Member>();
            MemberLobs = new HashSet<MemberLob>();
            MemberTracks = new HashSet<MemberTrack>();
            TrackChanges = new HashSet<TrackChange>();
        }

        public int Id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string GenderCd { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int? EthnicityId { get; set; }
        public int? LanguageId { get; set; }
        public int? RelationMemberId { get; set; }
        public string RelationCd { get; set; }
        public string Ssn { get; set; }
        public int? ResidentialLocationId { get; set; }
        public int? MailingLocationId { get; set; }
        public int? IntegrationActivityId { get; set; }
        public DateTime? CreationDt { get; set; }
        public DateTime? UpdateDt { get; set; }

        public virtual Ethnicity Ethnicity { get; set; }
        public virtual IntegrationActivity IntegrationActivity { get; set; }
        public virtual Language Language { get; set; }
        public virtual Location MailingLocation { get; set; }
        public virtual Member RelationMember { get; set; }
        public virtual Location ResidentialLocation { get; set; }
        public virtual ICollection<Member> InverseRelationMember { get; set; }
        public virtual ICollection<MemberLob> MemberLobs { get; set; }
        public virtual ICollection<MemberTrack> MemberTracks { get; set; }
        public virtual ICollection<TrackChange> TrackChanges { get; set; }
    }
}
