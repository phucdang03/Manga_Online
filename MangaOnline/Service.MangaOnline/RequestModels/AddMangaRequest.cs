using System.ComponentModel.DataAnnotations;

namespace Service.MangaOnline.RequestModels;

public class AddMangaRequest
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; }
    [Required]
    [MaxLength(200)]
    public string AuthorName { get; set; }
    [Required]
    public string Description { get; set; }
    [Required]
    public int CreatedAt { get; set; }
    [Required]
    public bool IsActive { get; set; }
    [Required]
    public Guid[] CategoriesId { get; set; }
    [Required]
    public string Image { get; set; }
    [Required]
    public int Status { get; set; }
}