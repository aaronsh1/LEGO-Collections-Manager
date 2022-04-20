using System;
using System.Collections.Generic;

#nullable disable

namespace LegoCollectionManager.Models
{
    public partial class CustomSetPiece
    {
        public int? CustomSet { get; set; }
        public string Piece { get; set; }
        public int? Colour { get; set; }
        public int? Amount { get; set; }

        public virtual Colour ColourNavigation { get; set; }
        public virtual CustomSet CustomSetNavigation { get; set; }
        public virtual Piece PieceNavigation { get; set; }
    }
}
