using System;
using System.Collections.Generic;

#nullable disable

namespace LegoCollectionManager.Models
{
    public partial class User
    {
        public User()
        {
            CustomSets = new HashSet<CustomSet>();
            UserAvatars = new HashSet<UserAvatar>();
            UserSets = new HashSet<UserSet>();
        }

        public int UserId { get; set; }
        public string Username { get; set; }
        public string Salt { get; set; }
        public string Password { get; set; }

        public virtual ICollection<CustomSet> CustomSets { get; set; }
        public virtual ICollection<UserAvatar> UserAvatars { get; set; }
        public virtual ICollection<UserSet> UserSets { get; set; }
    }
}
