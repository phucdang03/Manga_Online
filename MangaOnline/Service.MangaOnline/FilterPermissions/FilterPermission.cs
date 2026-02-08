using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using Service.MangaOnline.Commons;
using Service.MangaOnline.Models;
using Service.MangaOnline.ResponseModels;

namespace Service.MangaOnline.FilterPermissions;

public class FilterPermission : ActionFilterAttribute
{
    public ActionFilterEnum Action { get; set; }
    private UserRoleEnum[]? UserRole { get; set; }

    public override void OnActionExecuting(ActionExecutingContext actionContext)
    {
        try
        {
            StringValues values;
            var checkAction = false;
            var userToken = new UserTokenHeader();

            var action = ActionFilter.GetAllActionModel(Action);
            if (action != null && action.UserRole != null)
            {
                UserRole = action.UserRole;
            }

            // var newToken = WriteToken("john_doe", "123@gmail.com", "userVip", "12345");

            // action public
            if (UserRole == null)
            {
                checkAction = true;
            }
            else
            {
                //check role
                // USER_DATA 
                // ACCESS_TOKEN
                if (actionContext.HttpContext.Request.Headers.TryGetValue("ACCESS_TOKEN", out values))
                {
                    var headerValue = values.FirstOrDefault();
                    if (!String.IsNullOrEmpty(headerValue))
                    {
                        userToken = ReadToken(headerValue);
                        using (var context = new MangaOnlineV1DevContext())
                        {
                            UserRoleEnum userRole;
                            Enum.TryParse(userToken.Role, out userRole);
                            if (context.Users
                                    .Include(x=>x.UserToken)
                                    .FirstOrDefault(x =>
                                        x.RoleId == (Int32)userRole && x.Email == userToken.Email && x.UserToken!.Value == headerValue) is not null)
                            {
                                if (UserRole is null)
                                {
                                    checkAction = true;
                                }
                                else
                                {
                                    var t = UserRole.FirstOrDefault(x => x.ToString().ToLower() == userToken.Role!.ToLower())
                                        .ToString();
                                    if (t != "0")
                                    {
                                        checkAction = true;
                                    }
                                } 
                            }
                        }
                        
                    }
                }
            }

            if (!checkAction)
            {
                actionContext.Result = new JsonResult(new
                {
                    success = false,
                    status = 401,
                    error = "401 Unauthorized"
                });
            }
            else
            {
                actionContext.HttpContext.Request.Headers.TryGetValue("ACCESS_TOKEN", out values);
                if (values.FirstOrDefault()!="null")
                {
                    var headerValue = values.FirstOrDefault();
                    if (!String.IsNullOrEmpty(headerValue))
                    {
                        userToken = ReadToken(headerValue);
                    }
                }

                if (userToken.Role!=null)
                {
                    actionContext.HttpContext.Session.SetString("RoleUser",userToken.Role);
                    actionContext.HttpContext.Session.SetString("IpAddress",userToken.IpAddress??"1111");
                }
            }
        }
        catch
        {
            actionContext.Result = new JsonResult(new
            {
                success = false,
                status = 401,
                error = "401 Unauthorized"
            });
        }

        base.OnActionExecuting(actionContext);
    }

    public UserTokenHeader ReadToken(string tokenString)
    {
        var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(config["jwt:key"])),
            ValidIssuer = config["jwt:Issuer"],
            ValidAudience = config["jwt:Issuer"],
            ValidateIssuer = true,
            ValidateAudience = true,
            ClockSkew = TimeSpan.Zero
        };
        var handler = new JwtSecurityTokenHandler();
        var claimsPrincipal = handler.ValidateToken(tokenString, tokenValidationParameters, out var validatedToken);

        var jwtToken = validatedToken as JwtSecurityToken;
        if (jwtToken == null)
        {
            throw new SecurityTokenException("Invalid token");
        }

        var userToken = new UserTokenHeader();

        userToken.UserId = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
        userToken.UserName = jwtToken.Claims.FirstOrDefault(c => c.Type == "username")?.Value;
        userToken.Email = jwtToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
        userToken.Role = jwtToken.Claims.FirstOrDefault(c => c.Type == "role")?.Value;
        userToken.IpAddress = jwtToken.Claims.FirstOrDefault(c => c.Type == "ipAddress")?.Value;

        return userToken;
    }
}

// Request with custom header
// $.ajax({
//     type: "POST",
//     contentType: "application/json",
//     url: "http://localhost:5098/WeatherForecast/PostWeatherForecast",
//     dataType: 'json',
//     headers: {"ACCESS_TOKEN" : localStorage.getItem('ACCESS_TOKEN')},
//     success: function(success) {
//     }
// });

// Lưu giá trị vào local storage
// localStorage.setItem('ACCESS_TOKEN', '123');
// get
// localStorage.getItem('ACCESS_TOKEN');


// public override void OnResultExecuting(ResultExecutingContext context)
// {
//     var newToken = WriteToken("john_doe", "123@gmail.com", "admin", "12345");
//     context.HttpContext.Request.Headers["Authorization"] = newToken;
//     base.OnResultExecuting(context);
// }