using System;
using System.Collections.Generic;

#nullable disable

namespace LegoCollectionManager.Models
{
    public partial class SetPieceCategory
    {
        public int SetPieceCategoryId { get; set; }
        public int? PieceCategory { get; set; }
        public int? Set { get; set; }

        public virtual PieceCategory PieceCategoryNavigation { get; set; }
        public virtual Set SetNavigation { get; set; }
    }
}
