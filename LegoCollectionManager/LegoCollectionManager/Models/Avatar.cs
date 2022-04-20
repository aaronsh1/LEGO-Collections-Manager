using System;
using System.Collections.Generic;

#nullable disable

namespace LegoCollectionManager.Models
{
    public partial class Avatar
    {
        public Avatar()
        {
            UserAvatars = new HashSet<UserAvatar>();
        }

        public int AvatarId { get; set; }
        public string Url { get; set; }

        public virtual ICollection<UserAvatar> UserAvatars { get; set; }
    }
}
