using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Service.MangaOnline.Commons;
using Service.MangaOnline.Extensions;
using Service.MangaOnline.FilterPermissions;
using Service.MangaOnline.Models;
using Service.MangaOnline.RequestModels;
using Service.MangaOnline.ResponseModels;

namespace Service.MangaOnline.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : Controller
{
    private readonly MangaOnlineV1DevContext _context;
    private readonly IExtensionManga _extensionManga;
    private readonly IJwtTokenHandler _jwt;


    public AuthController(MangaOnlineV1DevContext mangaOnlineV1DevContext, IExtensionManga extensionManga,
        IJwtTokenHandler jwt)
    {

        _context = mangaOnlineV1DevContext;
        _extensionManga = extensionManga;
        _jwt = jwt;
    }

    [HttpPost("AuthLogin")]
    public async Task<IActionResult> AuthLogin([FromForm]LoginRequest request)
    {
        var user = await _context.Users
            .Include(x => x.UserToken)
            .FirstOrDefaultAsync(x => x.Email == request.Email &&  x.IsActive == true && x.EmailConfirmed == true);
        
        if (user != null && BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
        {
            var token = _jwt.WriteToken(
                user.FullName,
                user.Email,
                _extensionManga.CoverIntToEnum<UserRoleEnum>((int)user.RoleId!).ToString(),
                user.Id.ToString(),
                request.ipAddress);
            user.UserToken!.Value = token;
            await _context.SaveChangesAsync();
            return Ok(new
            {
                success = true,
                status = 200,
                data = Json(new LoginResponse
                {
                    UserData = new UserData
                    {
                        UserId = user.Id,
                        Email = user.Email,
                        UserName = user.FullName,
                        Role = _extensionManga.CoverIntToEnum<UserRoleEnum>((int)user.RoleId!).ToString(),
                        Avartar = user.Avatar
                    },
                    AccessToken = token,
                    ListFollow = _context.Mangas
                        .Include(x=>x.FollowLists)
                        .Where(x => x.FollowLists.FirstOrDefault(y => y.UserId == user.Id) != null)
                        .Select(x=>x.Id.ToString()).ToList()
                })
            });
        }
        
        return BadRequest(new
        {
            success = false,
            status = 400,
            error = _extensionManga.GetEnumDescription(NotificationEnum.LoginFailed)
        });
    }

    [HttpPost("AuthRegister")]
    public async Task<IActionResult> AuthRegister([FromForm] RegisterUserRequest request)
    {
        var dynamic = (dynamic)request.Validate();

        // //validate request
        if (!dynamic.success)
        {
            return BadRequest(request.Validate());
        }

        var checkUser = await _context.Users.FirstOrDefaultAsync(x => x.Email == request.Email);

        if (checkUser is not null)
        {
            return BadRequest(new
            {
                success = false,
                status = 400,
                error = _extensionManga.GetEnumDescription(NotificationEnum.EmailExists)
            });
        }

        // send mail
        var userId = Guid.NewGuid();
        
        // TODO: Enable email verification in production
        // Temporarily bypass email verification for development
        /*
        var config = new ConfigurationBuilder().AddJsonFile("templateEmail.json").Build();
        var sendMail = _extensionManga.SendEmailAsync(
            request.Email,
            "[MangaOnline] Xác thực email"
            , config["templateEmail:head"]
              + "http://localhost:5098/Auth/VerifyEmailRegister?userId="
              + userId
              + config["templateEmail:last"]);
        if (!sendMail.Result)
        {
            return BadRequest(new
            {
                success = false,
                status = 400,
                error = _extensionManga.GetEnumDescription(NotificationEnum.SendEmailError)
            });
        }
        */

        var createUser = new User()
        {
            Id = userId,
            SubId = 0,
            FullName = request.FullName, // length 6 -> 20
            Email = request.Email,
            EmailConfirmed = true, // Set to true to bypass email verification
            Password = BCrypt.Net.BCrypt.HashPassword(request.Password), // length 6 -> 20
            PhoneNumber = request.PhoneNumber, // length 10
            PhoneNumberConfirmed = true,
            AccessFailedCount = 0,
            CreatedAt = DateTimeOffset.Now,
            ModifiedAt = DateTimeOffset.Now,
            IsActive = true, // Set to true so user can login immediately
            Status = (int)StatusUser.Normal,
            RoleId = request.Email.Contains("admin") ? (int)UserRoleEnum.Admin : (int)UserRoleEnum.UserNormal // Auto assign Admin if email contains "admin"
        };

        var userToken = new UserToken
        {
            Id = Guid.NewGuid(),
            UserId = createUser.Id,
            Email = createUser.Email,
            Expires = DateTime.UtcNow.AddDays(30),
            Value = _jwt.WriteToken(
                createUser.FullName,
                createUser.Email,
                _extensionManga.CoverIntToEnum<UserRoleEnum>((int)createUser.RoleId).ToString(),
                createUser.Id.ToString(),"1111")
        };
        _context.Users.Add(createUser);
        _context.UserTokens.Add(userToken);
        await _context.SaveChangesAsync();
        return Ok(new
        {
            success = true,
            status = 200,
            message = _extensionManga.GetEnumDescription(NotificationEnum.RegisterAccountSuccess)
        });
    }

    [HttpGet("VerifyEmailRegister")]
    public async Task<IActionResult> VerifyEmail(Guid userId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId && x.EmailConfirmed == false);
        if (user is not null)
        {
            user.EmailConfirmed = true;
            user.IsActive = true;
            user.ModifiedAt = DateTimeOffset.Now;
            await _context.SaveChangesAsync();
            return Redirect("http://localhost:5030/Auth/AuthVerifyEmail");
        }
        else
        {
            return BadRequest(new
            {
                success = false,
                status = 400,
                error = _extensionManga.GetEnumDescription(NotificationEnum.RequestFailed)
            });
        }
    }

    [HttpPut("ChangePassword")]
    [FilterPermission(Action = ActionFilterEnum.ChangePassword)]
    public async Task<IActionResult> ChangePassword([FromForm] ChangePasswordRequest request)
    {
        var dynamic = (dynamic)request.Validate();
        // validate request
        if (!dynamic.success)
        {
            return BadRequest(request.Validate());
        }

        var user = await _context.Users
            .Include(x => x.UserToken)
            .FirstOrDefaultAsync(x => x.Email == request.Email);
        if (user != null && BCrypt.Net.BCrypt.Verify(request.OldPassword, user.Password))
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            await _context.SaveChangesAsync();
            return Ok(new
            {
                success = true,
                status = 200,
                message = _extensionManga.GetEnumDescription(NotificationEnum.ChangePasswordSuccess)
            });
        }
        else
        {
            return BadRequest(new
            {
                success = false,
                status = 400,
                error = _extensionManga.GetEnumDescription(NotificationEnum.PasswordFailed)
            });
        }
    }

    [HttpPut("UpdateRole")]
   // [FilterPermission(Action = ActionFilterEnum.UpdateAccount)]
    public async Task<IActionResult> UpdateRole(String gmail) {
        var user = _context.Users.FirstOrDefault(x => x.Email.Equals(gmail));
        user.RoleId = 3;
        _context.SaveChanges();
        return Ok(new
        {
            success = true,
            status = 200,
        });
     }

    [HttpGet("History")]
    // [FilterPermission(Action = ActionFilterEnum.UpdateAccount)]
    public async Task<IActionResult> History(String gmail)
    {
        var list = _context.Histories.Where(x => x.User == gmail).ToList();
        return Ok(new
        {
            success = true,
            status = 200,
            data = Json(list)
        });
    }

    [HttpPut("DeUpdateRole")]
  //  [FilterPermission(Action = ActionFilterEnum.UpdateAccount)]
    public async Task<IActionResult> DeUpdateRole(String gmail)
    {
        var user = _context.Users.FirstOrDefault(x => x.Email.Equals(gmail));
        user.RoleId = 2;
        _context.SaveChanges();
        return Ok(new
        {
            success = true,
            status = 200,
        });
    }

    [HttpPost("CreateTransaction")]
   // [FilterPermission(Action = ActionFilterEnum.UpdateAccount)]
    public async Task<IActionResult> CreateTransaction([FromForm] History request)
    {
        _context.Histories.Add(request);
        await _context.SaveChangesAsync();
        return Ok(new
        {
            success = true,
            status = 200,
        });
    }

    [HttpGet("GetUserById")]
    [FilterPermission(Action = ActionFilterEnum.GetUserById)]
    public async Task<IActionResult> GetUserById(Guid userId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
        if (user is not null)
        {
            return Ok(new
            {
                success = true,
                status = 200,
                data = Json(user)
            });
        }
        else
        {
            return BadRequest(new
            {
                success = false,
                status = 400,
                error = _extensionManga.GetEnumDescription(NotificationEnum.UserNotFound)
            });
        }
    }
}