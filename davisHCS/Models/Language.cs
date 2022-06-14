using System;
using System.Collections.Generic;

#nullable disable

namespace davisHCS.Models
{
    public partial class Language
    {
        public Language()
        {
            Members = new HashSet<Member>();
            Providers = new HashSet<Provider>();
        }

        public int Id { get; set; }
        public string LanguageName { get; set; }
        public DateTime? CreationDt { get; set; }

        public virtual ICollection<Member> Members { get; set; }
        public virtual ICollection<Provider> Providers { get; set; }
    }
}
