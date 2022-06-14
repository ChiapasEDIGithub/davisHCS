using System;
using System.Collections.Generic;

#nullable disable

namespace davisHCS.Models
{
    public partial class Ethnicity
    {
        public Ethnicity()
        {
            Members = new HashSet<Member>();
        }

        public int Id { get; set; }
        public string EthnicityName { get; set; }
        public DateTime? CreationDt { get; set; }

        public virtual ICollection<Member> Members { get; set; }
    }
}
