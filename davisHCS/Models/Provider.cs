using System;
using System.Collections.Generic;

#nullable disable

namespace davisHCS.Models
{
    public partial class Provider
    {
        public Provider()
        {
            MemberLobs = new HashSet<MemberLob>();
        }

        public int Id { get; set; }
        public string ProviderType { get; set; }
        public string OrgName { get; set; }
        public string PhysicianLastName { get; set; }
        public string PhysicianFirstName { get; set; }
        public string PhysicianMiddleName { get; set; }
        public string Npi { get; set; }
        public int? LocationId { get; set; }
        public int? LanguageId { get; set; }
        public DateTime? PhysicianDateOfBirth { get; set; }
        public string PhysicianGender { get; set; }
        public string Ein { get; set; }
        public string Ssn { get; set; }
        public DateTime CreationDt { get; set; }

        public virtual Language Language { get; set; }
        public virtual Location Location { get; set; }
        public virtual ICollection<MemberLob> MemberLobs { get; set; }
    }
}
