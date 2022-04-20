using System;
using System.Collections.Generic;

#nullable disable

namespace LegoCollectionManager.Models
{
    public partial class UserAvatar
    {
        public int UserAvatarId { get; set; }
        public int User { get; set; }
        public int Avatar { get; set; }

        public virtual Avatar AvatarNavigation { get; set; }
        public virtual User UserNavigation { get; set; }
    }
}
