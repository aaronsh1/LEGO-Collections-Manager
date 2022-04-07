using System;
using System.Collections.Generic;

#nullable disable

namespace LegoCollectionManager.Models
{
    public partial class UserSubsititutePool
    {
        public int? User { get; set; }
        public int? SubstitutePool { get; set; }

        public virtual SubstitutePool SubstitutePoolNavigation { get; set; }
        public virtual User UserNavigation { get; set; }
    }
}
