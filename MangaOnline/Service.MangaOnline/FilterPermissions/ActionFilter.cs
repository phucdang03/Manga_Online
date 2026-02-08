using Service.MangaOnline.Commons;
using Service.MangaOnline.RequestModels;

namespace Service.MangaOnline.FilterPermissions;

public static class ActionFilter
{
    public static List<ActionModel> AllActionModel { get; set; } = new List<ActionModel>()
    {
        new ActionModel()
        {
            Action = ActionFilterEnum.GetWeatherForecast,
            UserRole = null
        },
        new ActionModel()
        {
            Action = ActionFilterEnum.PostWeatherForecast,
            UserRole = new[] { UserRoleEnum.Admin, UserRoleEnum.UserVip }
        },
        new ActionModel()
        {
            Action = ActionFilterEnum.ChangePassword,
            UserRole = new[] { UserRoleEnum.Admin, UserRoleEnum.UserVip, UserRoleEnum.UserNormal }
        },
        new ActionModel()
        {
            Action = ActionFilterEnum.GetUserById,
            UserRole = new[] { UserRoleEnum.Admin }
        },
        new ActionModel()
        {
            Action = ActionFilterEnum.CreateManga,
            UserRole = new[] { UserRoleEnum.Admin }
        },
        new ActionModel()
        {
            Action = ActionFilterEnum.UpdateManga,
            UserRole = new[] { UserRoleEnum.Admin }
        },
        new ActionModel()
        {
            Action = ActionFilterEnum.DeleteManga,
            UserRole = new[] { UserRoleEnum.Admin }
        },
        new ActionModel()
        {
            Action = ActionFilterEnum.FollowManga,
            UserRole = new[] { UserRoleEnum.Admin, UserRoleEnum.UserVip, UserRoleEnum.UserNormal }
        },
        new ActionModel()
        {
            Action = ActionFilterEnum.UpdateAccount,
            UserRole = new[] { UserRoleEnum.Admin, UserRoleEnum.UserVip, UserRoleEnum.UserNormal }
        },
        new ActionModel()
        {
            Action = ActionFilterEnum.GetChapter,
            UserRole = null
        }
    };

    public static ActionModel? GetAllActionModel(ActionFilterEnum action)
    {
        return AllActionModel.Find(x => x.Action == action);
    }
}