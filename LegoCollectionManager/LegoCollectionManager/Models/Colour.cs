using System;
using System.Collections.Generic;

#nullable disable

namespace LegoCollectionManager.Models
{
    public partial class Colour
    {
        public Colour()
        {
            CustomSetPieces = new HashSet<CustomSetPiece>();
            MissingPieces = new HashSet<MissingPiece>();
            SetPieces = new HashSet<SetPiece>();
        }

        public int ColourId { get; set; }
        public string ColourName { get; set; }

        public virtual ICollection<CustomSetPiece> CustomSetPieces { get; set; }
        public virtual ICollection<MissingPiece> MissingPieces { get; set; }
        public virtual ICollection<SetPiece> SetPieces { get; set; }
    }
}
