namespace Service.MangaOnline.ResponseModels;

public class LoginResponse
{
    public UserData UserData { get; set; }
    public string AccessToken { get; set; }
    public List<string>? ListFollow { get; set; }
}

public class UserData
{
    public Guid UserId { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public string? Avartar { get; set; } 
}