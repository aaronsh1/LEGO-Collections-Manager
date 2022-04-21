using LegoCollectionManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LegoCollectionManager.DTO
{
    public class SearchResultDTO
    {
        public int setId;
        public SetInformation.SetInformationDTO setDTO;
        public double matchPercentage;
        public IEnumerable<SetPiece> missingPieces;
    }

    public class SearchRequestDTO
    {
        public string Item1;
        public string Item2;
        public string Item3;
    }
}
