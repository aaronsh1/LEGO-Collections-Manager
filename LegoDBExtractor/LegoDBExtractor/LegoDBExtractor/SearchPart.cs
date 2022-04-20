using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegoDBExtractor
{
    class SearchPart
    {
        public string Id;
        public string Color;
        public int Count;

        public override string ToString()
        {
            return $"{Id} {Color} {Count}";
        }
    }
}
