using System;
using System.Collections.Generic;

#nullable disable

namespace LegoCollectionManager.Models
{
    public partial class UserSet
    {
        public UserSet()
        {
            MissingPieces = new HashSet<MissingPiece>();
        }

        public int UseSetId { get; set; }
        public int? User { get; set; }
        public int? Set { get; set; }

        public virtual Set SetNavigation { get; set; }
        public virtual User UserNavigation { get; set; }
        public virtual ICollection<MissingPiece> MissingPieces { get; set; }
    }
}
