using System.ComponentModel;

namespace Service.MangaOnline.Commons;

public enum UserRoleEnum
{
    [Description("admin")] 
    Admin = 1,
    [Description("người dùng BT")] 
    UserNormal,
    [Description("người dùng Vip")] 
    UserVip,
}