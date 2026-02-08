using Service.MangaOnline.ResponseModels;

namespace Service.MangaOnline.Extensions;

public interface IJwtTokenHandler
{
    string WriteToken(string fullName, string email, string role, string userId,string ipAddress); 
    UserTokenHeader ReadToken(string tokenString);
}