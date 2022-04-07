using System;
using System.Collections.Generic;

#nullable disable

namespace LegoCollectionManager.Models
{
    public partial class SubstitutePool
    {
        public SubstitutePool()
        {
            SubstitutePoolItems = new HashSet<SubstitutePoolItem>();
        }

        public int SubstitutePool1 { get; set; }
        public string Name { get; set; }

        public virtual ICollection<SubstitutePoolItem> SubstitutePoolItems { get; set; }
    }
}
