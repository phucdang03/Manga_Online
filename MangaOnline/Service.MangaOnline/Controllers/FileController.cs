using Microsoft.AspNetCore.Mvc;
using Service.MangaOnline.Commons;
using Service.MangaOnline.Extensions;

namespace Service.MangaOnline.Controllers;

[ApiController]
[Route("[controller]")]
public class FileController : Controller
{
    private long SizeLimitImage = 5;
    private readonly IExtensionManga _extensionManga;

    public FileController(IExtensionManga extensionManga)
    {
        _extensionManga = extensionManga;
    }

    [HttpPost("CreateImage")]
    public IActionResult CreateImage([FromForm] IFormFile imageFile)
    {
        var fileSize = imageFile.Length / (1024 * 1024 * 50);
        if (fileSize < SizeLimitImage)
        {
            var nameImage = _extensionManga.CreateImage(imageFile);
            return Ok(new
            {
                success = true,
                status = 200,
                data = nameImage
            });
        }

        return NotFound(new
        {
            success = false,
            status = 400,
            message = _extensionManga.GetEnumDescription(NotificationEnum.SizeImageLimit)
        });
    }
}