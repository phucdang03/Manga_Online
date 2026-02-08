using System.ComponentModel;

namespace Service.MangaOnline.Commons;

public enum StatusUser
{
    [Description("Bình thường")]
    Normal,
    [Description("Bị khóa")]
    Lock,
}
