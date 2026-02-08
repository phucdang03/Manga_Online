using Service.MangaOnline.Commons;

namespace Service.MangaOnline.RequestModels;

public class ActionModel
{
    public ActionFilterEnum Action { get; set; }
    public UserRoleEnum[]? UserRole { get; set; }
    public bool CheckAllRole { get; set; } = false; // false chi can thoa man 1 userRole
}