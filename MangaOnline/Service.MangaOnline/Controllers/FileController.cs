using Microsoft.AspNetCore.Mvc;
using Service.MangaOnline.Commons;
using Service.MangaOnline.Extensions;

namespace Service.MangaOnline.Controllers;

[ApiController]
[Route("[controller]")]
public class FileController : Controller
{
    private long SizeLimitImage = 50; // Increased to 50MB for both images and PDFs
    private readonly IExtensionManga _extensionManga;

    public FileController(IExtensionManga extensionManga)
    {
        _extensionManga = extensionManga;
    }

    [HttpPost("CreateImage")]
    public IActionResult CreateImage([FromForm] IFormFile imageFile)
    {
        var fileSizeMB = imageFile.Length / (1024.0 * 1024.0); // Convert to MB
        if (fileSizeMB <= SizeLimitImage) // SizeLimitImage is 5MB
        {
            var nameImage = _extensionManga.CreateImage(imageFile);
            return Ok(new
            {
                success = true,
                status = 200,
                data = nameImage
            });
        }

        return BadRequest(new
        {
            success = false,
            status = 400,
            message = $"File size {fileSizeMB:F2}MB exceeds limit of {SizeLimitImage}MB"
        });
    }
    
    // Serve image files
    [HttpGet("GetImage")]
    public IActionResult GetImage(string fileName)
    {
        try
        {
            var imagePath = Path.Combine(_extensionManga.GetPathService("Client.Manager"), 
                "wwwroot", "image", "manga-image", fileName);
                
            if (!System.IO.File.Exists(imagePath))
            {
                return NotFound($"Image {fileName} not found");
            }
            
            var imageBytes = System.IO.File.ReadAllBytes(imagePath);
            var contentType = GetContentType(fileName);
            
            return File(imageBytes, contentType);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error loading image: {ex.Message}");
        }
    }
    
    // Serve PDF files  
    [HttpGet("GetPdf")]
    public IActionResult GetPdf(string fileName)
    {
        try
        {
            Console.WriteLine($"🔍 GetPdf called for: {fileName}");
            
            var basePath = _extensionManga.GetPathService("Client.Manager");
            var pdfPath = Path.Combine(basePath, "wwwroot", "pdf", fileName);
            
            Console.WriteLine($"📁 Base path: {basePath}");
            Console.WriteLine($"📄 Full PDF path: {pdfPath}");
            Console.WriteLine($"📋 File exists: {System.IO.File.Exists(pdfPath)}");
                
            if (!System.IO.File.Exists(pdfPath))
            {
                Console.WriteLine($"❌ PDF not found: {pdfPath}");
                return NotFound($"PDF {fileName} not found at path: {pdfPath}");
            }
            
            var pdfBytes = System.IO.File.ReadAllBytes(pdfPath);
            Console.WriteLine($"✅ PDF loaded successfully, size: {pdfBytes.Length} bytes");
            
            return File(pdfBytes, "application/pdf");
        }
        catch (Exception ex)
        {
            return BadRequest($"Error loading PDF: {ex.Message}");
        }
    }
    
    private string GetContentType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension switch
        {
            ".jpg" => "image/jpeg",
            ".jpeg" => "image/jpeg", 
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".bmp" => "image/bmp",
            ".webp" => "image/webp",
            _ => "application/octet-stream"
        };
    }
}