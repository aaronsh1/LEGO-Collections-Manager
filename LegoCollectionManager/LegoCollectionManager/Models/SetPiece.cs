using System;
using System.Collections.Generic;

#nullable disable

namespace LegoCollectionManager.Models
{
    public partial class SetPiece
    {
        public int SetPieceId { get; set; }
        public string Piece { get; set; }
        public int? SetId { get; set; }
        public int? Amount { get; set; }
        public int? Colour { get; set; }

        public virtual Colour ColourNavigation { get; set; }
        public virtual Piece PieceNavigation { get; set; }
        public virtual Set Set { get; set; }
    }
}
