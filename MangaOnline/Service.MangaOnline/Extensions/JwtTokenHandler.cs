using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Service.MangaOnline.ResponseModels;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Service.MangaOnline.Extensions;

public class JwtTokenHandler : IJwtTokenHandler
{
    public string WriteToken(string fullName, string email, string role, string userId,string ipAddress)
    {
        var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim("username", fullName),
            new Claim("email", email),
            new Claim("role", role),
            new Claim("ipAddress", ipAddress)
        };

        var token = new JwtSecurityToken(
            claims: claims,
            issuer: config["jwt:Issuer"],
            audience: config["jwt:Issuer"],
            expires: DateTime.UtcNow.AddDays(30),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.ASCII.GetBytes(config["jwt:key"])),
                SecurityAlgorithms.HmacSha256Signature)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
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