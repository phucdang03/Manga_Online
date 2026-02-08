namespace Service.MangaOnline.Extensions;

public interface IExtensionManga
{
    string GetEnumDescription(Enum value);
    T CoverIntToEnum<T>(int value);
    
    Task<bool> SendEmailAsync(string recipient, string subject, string body);
    
    string CreateImage(IFormFile myFile);
    
    string UpdateImage(IFormFile myFile,string oldFile);
    
    string UpdateImageAvatarUser(IFormFile myFile,string? oldFile);
}