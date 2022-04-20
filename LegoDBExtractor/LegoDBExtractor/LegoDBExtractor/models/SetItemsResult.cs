using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegoDBExtractor
{
    class SetItemsResult
    {
        public List<string> SetsFormatted { get; set; }
        public HashSet<string> UniquePartNums { get; set; }

        public List<SetPart> SetParts { get; set; }

        public SetItemsResult(List<string> setsFormatted, HashSet<string> uniquePartNums, List<SetPart> setParts)
        {
            SetsFormatted = setsFormatted;
            UniquePartNums = uniquePartNums;
            SetParts = setParts;
        }
    }
}
