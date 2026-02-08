using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Service.MangaOnline.Commons;

namespace Service.MangaOnline.RequestModels;

public class RegisterUserRequest
{
    [Required] public string FullName { get; set; } = null!;
    [Required] public string Email { get; set; } = null!;
    [Required] public string Password { get; set; } = null!;
    public string? PhoneNumber { get; set; }

    public Object Validate()
    {
        var result = new
        {
            success = false,
            status = 400,
            error = "Validate fail"
        };

        var fullName = Regex.Replace(FullName.Trim(), @"\s+", " ");

        if ( !Regex.IsMatch(fullName, @"^.{6,20}$")
                                 || !Regex.IsMatch(Password, @"^.{6,20}$")
                                 || (PhoneNumber != null && !Regex.IsMatch(PhoneNumber, @"^\d{10}$"))
                                 || !Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$")
           )
        {
            return result;
        }

        return new
        {
            success = true
        };
    }
}