namespace Service.MangaOnline.Models
{
    public class ProfileUser
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string? Avatar { get; set; }
        public IFormFile? fileUp;
    }
}
