using System;
using System.Collections.Generic;

namespace Service.MangaOnline.Models
{
    public partial class IpUserVote
    {
        public Guid MangaId { get; set; }
        public Guid? UserId { get; set; }
        public int? Rate { get; set; }
    }
}
