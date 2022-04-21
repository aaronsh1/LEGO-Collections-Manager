using System;
using System.Collections.Generic;

#nullable disable

namespace LegoCollectionManager.Models
{
    public partial class Piece
    {
        public Piece()
        {
            CustomSetPieces = new HashSet<CustomSetPiece>();
            MissingPieces = new HashSet<MissingPiece>();
            SetPieces = new HashSet<SetPiece>();
        }

        public string PieceId { get; set; }
        public string PieceName { get; set; }
        public int? PieceCategory { get; set; }

        public virtual PieceCategory PieceCategoryNavigation { get; set; }
        public virtual ICollection<CustomSetPiece> CustomSetPieces { get; set; }
        public virtual ICollection<MissingPiece> MissingPieces { get; set; }
        public virtual ICollection<SetPiece> SetPieces { get; set; }
    }
}
