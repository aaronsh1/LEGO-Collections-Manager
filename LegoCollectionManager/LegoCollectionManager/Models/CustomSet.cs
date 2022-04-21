using System;
using System.Collections.Generic;

#nullable disable

namespace LegoCollectionManager.Models
{
    public partial class CustomSet
    {
        public CustomSet()
        {
            CustomSetPieces = new HashSet<CustomSetPiece>();
        }

        public int CustomSetId { get; set; }
        public string CustomSetName { get; set; }
        public int? PiecesAmount { get; set; }
        public byte[] CustomSetImage { get; set; }
        public int? User { get; set; }

        public virtual User UserNavigation { get; set; }
        public virtual ICollection<CustomSetPiece> CustomSetPieces { get; set; }
    }
}
