using Service.MangaOnline.Models;

namespace Service.MangaOnline.Commons;

public static class AllCategory
{
    public static List<Category> GetAll { get; set; } = new List<Category>()
    {
        new Category()
        {
            Id = Guid.Parse("D8C7163C-1E11-48C0-98D4-16B22C812607"),
            SubId = 3,
            Name = "Ngôn tình"
        },
        new Category()
        {
            Id = Guid.Parse("6ACCC550-D475-4357-BCB9-17302ED409C4"),
            SubId = 4,
            Name = "Drama"
        },new Category()
        {
            Id = Guid.Parse("FD761C84-897C-4529-AEC7-7A8731EF1B90"),
            SubId = 5,
            Name = "Trinh thám"
        },new Category()
        {
            Id = Guid.Parse("385871E9-75B7-4CF9-9EB3-87F31D6E7BAE"),
            SubId = 6,
            Name = "Cổ đại"
        },new Category()
        {
            Id = Guid.Parse("2660558E-65C0-4EFD-86F8-8B84DE4B9CE4"),
            SubId = 7,
            Name = "Tình cảm"
        },new Category()
        {
            Id = Guid.Parse("766A75B4-6942-4B9B-AECB-BBEEAA36A4B0"),
            SubId = 8,
            Name = "Anime"
        },new Category()
        {
            Id = Guid.Parse("83458358-35E2-4845-BDDC-C00314040CB0"),
            SubId = 9,
            Name = "Đam mỹ"
        }
    };

    public static Category? CategoryByID(Guid id)
    {
        return GetAll.Find(x=>x.Id == id);
    }
}