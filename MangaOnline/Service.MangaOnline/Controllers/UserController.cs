using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.MangaOnline.DTO;
using Service.MangaOnline.Models;

namespace Service.MangaOnline.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly MangaOnlineV1DevContext _context;

        public UserController(MangaOnlineV1DevContext mangaOnlineV1DevContext)
        {
            _context = mangaOnlineV1DevContext;
        }

        [HttpGet]
        public IActionResult GetUserProfile(Guid id)
        {
            User? user = _context.Users.Where(x => x.Id == id).FirstOrDefault();
            return Ok(user);
        }

        [HttpGet("GetUserToken/{id}")]
        public IActionResult GetUserToken(Guid id)
        {
            UserToken? token = _context.UserTokens
                   .Where(x => x.UserId == id).FirstOrDefault();
            return Ok(token);
        }

        [HttpPost("UpdateUser")]
        public IActionResult UpdateUser(ProfileUser user)
        {
            User? current = _context.Users.Where(x => x.Id == user.Id).FirstOrDefault();
            if (current == null)
            {
                return NotFound();
            }

            current.FullName = user.FullName;
            current.PhoneNumber = user.PhoneNumber;
            if (user.Avatar != null)
                current.Avatar = user.Avatar;

            _context.SaveChanges();

            return Ok(current);
        }
    }
}
