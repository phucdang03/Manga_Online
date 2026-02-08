using Microsoft.EntityFrameworkCore;
using Service.MangaOnline.Commons;
using Service.MangaOnline.Models;

namespace Service.MangaOnline.Extensions;

public static class Job
{
    public static async void CheckUserVip()
    {
        using (var context = new MangaOnlineV1DevContext())
        {
            var listUserExpired = context.UserTokens
                .Include(x => x.User)
                .Where(x => x.User.RoleId == (int)UserRoleEnum.UserVip && x.Expires < DateTime.Now);
            foreach (var user in listUserExpired)
            {
                user.Expires = DateTime.MaxValue;
                user.User.RoleId = (int)UserRoleEnum.UserNormal;
            }
            await context.SaveChangesAsync();
        }
    }
}