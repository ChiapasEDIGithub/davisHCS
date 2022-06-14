using System;
using System.Collections.Generic;

#nullable disable

namespace davisHCS.Models
{
    public partial class Location
    {
        public Location()
        {
            MemberMailingLocations = new HashSet<Member>();
            MemberResidentialLocations = new HashSet<Member>();
            Providers = new HashSet<Provider>();
        }

        public int Id { get; set; }
        public string LocationType { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string StateCd { get; set; }
        public string ZipCd { get; set; }

        public virtual ICollection<Member> MemberMailingLocations { get; set; }
        public virtual ICollection<Member> MemberResidentialLocations { get; set; }
        public virtual ICollection<Provider> Providers { get; set; }
    }
}
