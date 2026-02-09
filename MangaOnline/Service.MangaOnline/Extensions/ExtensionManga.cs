using System.ComponentModel;
using System.Net;
using System.Net.Mail;
using System.Reflection;

namespace Service.MangaOnline.Extensions;

public class ExtensionManga : IExtensionManga
{
    public string GetEnumDescription(Enum value)
    {
        FieldInfo fileInfo = value.GetType().GetField(value.ToString())!;
        
        DescriptionAttribute[]? attributes = fileInfo.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];
        
        if (attributes != null && attributes.Any())
        {
            return attributes.First().Description;
        }

        return value.ToString();
    }
    public T CoverIntToEnum<T>(int value)
    {
        return (T)Enum.ToObject(typeof(T), value);
    }
    public async Task<bool> SendEmailAsync(string recipient, string subject, string body)
    {
        var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        using (var client = new SmtpClient("smtp.gmail.com", 587))
        {
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential(config["EmailSetting:Email"], config["EmailSetting:Password"]);

            var message = new MailMessage
            {
                From = new MailAddress(config["EmailSetting:Email"]),
                To = { recipient },
                Subject = subject,
                Body = body
            };
            message.BodyEncoding = System.Text.Encoding.UTF8;
            message.SubjectEncoding = System.Text.Encoding.UTF8;
            message.IsBodyHtml = true;
            message.ReplyToList.Add(new MailAddress(config["EmailSetting:Email"]));
            message.Sender = new MailAddress(config["EmailSetting:Email"]);

            try
            {
                await client.SendMailAsync(message);
                return true;
            }
            catch (SmtpException ex)
            {
                // Lỗi xảy ra khi gửi email
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }
        }
    }
    
    public string CreateImage(IFormFile myFile)
    {
        if (myFile == null || myFile.Length == 0)
        {
            throw new Exception("File not found or empty.");
        }
        
        var newFileName = Guid.NewGuid();
        var extension = Path.GetExtension(myFile.FileName).ToLower();
        string fileName = newFileName + extension;

        // Determine folder path based on file type
        string folderPath;
        var imageExtensions = new[] { ".png", ".jpg", ".jpeg", ".gif", ".bmp", ".webp" };
        var documentExtensions = new[] { ".pdf" };
        
        if (imageExtensions.Contains(extension))
        {
            folderPath = "image/manga-image";
        }
        else if (documentExtensions.Contains(extension))
        {
            folderPath = "pdf";
        }
        else
        {
            throw new Exception($"Unsupported file type: {extension}");
        }

        string filePath = Path.Combine(GetPathService("Client.Manager"),
            "wwwroot", folderPath, fileName);
            
        // Create directory if it doesn't exist
        var directory = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        
        using (var file = new FileStream(filePath, FileMode.Create))
        {
            myFile.CopyTo(file);
        }
        return fileName;
    }
    
    public string UpdateImage(IFormFile myFile, string oldFile)
    {
        var fileName = CreateImage(myFile);
        // delete img 
        if (!string.IsNullOrEmpty(oldFile))
        {
            string imgPath = Path.Combine(GetPathService("Client.Manager"),
                "wwwroot", "image/manga-image", oldFile);
            FileInfo fileDelete = new FileInfo(imgPath);
            if (fileDelete.Length > 0)
            {
                System.IO.File.Delete(imgPath);
                fileDelete.Delete();
            }
        }
        return fileName;
    }
    
    public string UpdateImageAvatarUser(IFormFile myFile, string? oldFile)
    {
        var fileName = CreateImageAvatarUser(myFile);
        // delete img 
        if (!string.IsNullOrEmpty(oldFile))
        {
            string imgPath = Path.Combine(GetPathService("Client.Manager"),
                "wwwroot", "image/avartar-user", oldFile);
            FileInfo fileDelete = new FileInfo(imgPath);
            if (fileDelete.Length > 0)
            {
                System.IO.File.Delete(imgPath);
                fileDelete.Delete();
            }
        }
        return fileName;
    }

    public string CreateImageAvatarUser(IFormFile myFile)
    {
        if (myFile == null || myFile.Length == 0)
        {
            throw new Exception("File not found or empty.");
        }
        //add img
        var newFileName = Guid.NewGuid();
        var extension = Path.GetExtension(myFile.FileName);
        string fileName = newFileName + extension;
        
        string filePath = Path.Combine(GetPathService("Client.Manager"),
            "wwwroot", "image/avartar-user", fileName);
        using (var file = new FileStream(filePath, FileMode.Create))
        {
            myFile.CopyTo(file);
        }
        return fileName;
    }

    public string GetPathService(string nameService)
    {
        string currentPath = Directory.GetCurrentDirectory();
        string parentPath = Directory.GetParent(currentPath)!.FullName;
        string targetPath = Path.Combine(parentPath, nameService);
        return targetPath;
    }
}