using System;
using System.Collections.Generic;

#nullable disable

namespace LegoCollectionManager.Models
{
    public partial class Piece
    {
        public Piece()
        {
            MissingPieces = new HashSet<MissingPiece>();
            SubstitutePoolItems = new HashSet<SubstitutePoolItem>();
        }

        public int PieceId { get; set; }
        public string PieceName { get; set; }
        public int? PieceCategory { get; set; }
        public string PieceImage { get; set; }

        public virtual PieceCategory PieceCategoryNavigation { get; set; }
        public virtual SetPiece SetPiece { get; set; }
        public virtual ICollection<MissingPiece> MissingPieces { get; set; }
        public virtual ICollection<SubstitutePoolItem> SubstitutePoolItems { get; set; }
    }
}
