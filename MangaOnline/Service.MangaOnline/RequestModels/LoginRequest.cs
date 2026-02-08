using System.ComponentModel.DataAnnotations;

namespace Service.MangaOnline.RequestModels;

public class LoginRequest
{
    [Required] public string Email { get; set; } = null!;
    [Required] public string Password { get; set; } = null!;
    [Required] public string ipAddress { get; set; } = null!;
}