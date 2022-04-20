using System;
using System.Collections.Generic;

#nullable disable

namespace LegoCollectionManager.Models
{
    public partial class Set
    {
        public Set()
        {
            SetPieceCategories = new HashSet<SetPieceCategory>();
            SetPieces = new HashSet<SetPiece>();
            UserSets = new HashSet<UserSet>();
        }

        public int SetId { get; set; }
        public string SetName { get; set; }
        public int? PiecesAmount { get; set; }
        public int? SetCategory { get; set; }

        public virtual SetCategory SetCategoryNavigation { get; set; }
        public virtual ICollection<SetPieceCategory> SetPieceCategories { get; set; }
        public virtual ICollection<SetPiece> SetPieces { get; set; }
        public virtual ICollection<UserSet> UserSets { get; set; }
    }
}
