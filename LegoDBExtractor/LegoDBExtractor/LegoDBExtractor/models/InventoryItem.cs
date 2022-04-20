using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegoDBExtractor
{
    class InventoryItem
    {
        public int Id { get; set; }
        public int SetId { get; set; }
        public int SubVersion { get; set; }

        public InventoryItem(int Id, int SetId, int SubVersion)
        {
            this.Id = Id;
            this.SetId = SetId;
            this.SubVersion = SubVersion;
        }
    }
}
