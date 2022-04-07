using System;
using System.Collections.Generic;

#nullable disable

namespace LegoCollectionManager.Models
{
    public partial class SetCategory
    {
        public SetCategory()
        {
            Sets = new HashSet<Set>();
        }

        public int SetCategoryId { get; set; }
        public string SetCategoryName { get; set; }

        public virtual ICollection<Set> Sets { get; set; }
    }
}
