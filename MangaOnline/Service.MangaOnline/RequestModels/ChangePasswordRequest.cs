using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Service.MangaOnline.RequestModels;

public class ChangePasswordRequest
{
    [Required] public string Email { get; set; } = null!;
    [Required] public string OldPassword { get; set; } = null!;
    [Required] public string NewPassword { get; set; } = null!;

    public Object Validate()
    {
        if (OldPassword==NewPassword || !Regex.IsMatch(NewPassword, @"^.{6,20}$"))
        {
            return new
            {
                success = false,
                status = 400,
                error = "Validate fail"
            };
        }
        else
        {
            return new { success = true };
        }
    }
}