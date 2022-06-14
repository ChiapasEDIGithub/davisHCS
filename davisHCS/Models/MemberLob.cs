using System;
using System.Collections.Generic;

#nullable disable

namespace davisHCS.Models
{
    public partial class MemberLob
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public int? OrganizationProviderId { get; set; }
        public string LineOfBusiness { get; set; }
        public string Source { get; set; }
        public DateTime? CreationDt { get; set; }
        public DateTime? UpdateDt { get; set; }

        public virtual Member Member { get; set; }
        public virtual Provider OrganizationProvider { get; set; }
    }
}
