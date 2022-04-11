using System;
using System.Collections.Generic;

#nullable disable

namespace LegoCollectionManager.Models
{
    public partial class User
    {
        public User()
        {
            UserSets = new HashSet<UserSet>();
        }

        public int UserId { get; set; }
        public string Username { get; set; }

        public virtual ICollection<UserSet> UserSets { get; set; }
    }
}
