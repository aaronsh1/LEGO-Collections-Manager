using System;
using System.Collections.Generic;

#nullable disable

namespace LegoCollectionManager.Models
{
    public partial class SubstitutePoolItem
    {
        public int SubstitutePoolItemId { get; set; }
        public int? Piece { get; set; }
        public int? SubstitutePool { get; set; }

        public virtual Piece PieceNavigation { get; set; }
        public virtual SubstitutePool SubstitutePoolNavigation { get; set; }
    }
}
