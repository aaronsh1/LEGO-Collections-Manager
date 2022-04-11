using System;
using System.Collections.Generic;

#nullable disable

namespace LegoCollectionManager.Models
{
    public partial class PieceCategory
    {
        public PieceCategory()
        {
            Pieces = new HashSet<Piece>();
            SetPieceCategories = new HashSet<SetPieceCategory>();
        }

        public int PieceCategoryId { get; set; }
        public string PieceCategoryName { get; set; }

        public virtual ICollection<Piece> Pieces { get; set; }
        public virtual ICollection<SetPieceCategory> SetPieceCategories { get; set; }
    }
}
