using System;
using System.Collections.Generic;

#nullable disable

namespace LegoCollectionManager.Models
{
    public partial class MissingPiece
    {
        public int MissingPieceId { get; set; }
        public int? Piece { get; set; }
        public int? UserSet { get; set; }
        public int? Amount { get; set; }
        public int? Colour { get; set; }

        public virtual Colour ColourNavigation { get; set; }
        public virtual Piece PieceNavigation { get; set; }
        public virtual UserSet UserSetNavigation { get; set; }
    }
}
