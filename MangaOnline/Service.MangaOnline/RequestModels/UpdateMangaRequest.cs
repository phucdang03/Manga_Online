using System.ComponentModel.DataAnnotations;

namespace Service.MangaOnline.RequestModels;

public class UpdateMangaRequest
{
    [Required]
    public Guid MangaId { get; set; }
    [MaxLength(200)]
    public string? Name { get; set; }
    [MaxLength(200)]
    public string? AuthorName { get; set; }
    public string? Description { get; set; }
    public int? CreatedAt { get; set; }
    public bool? IsActive { get; set; }
    public Guid[]? CategoriesId { get; set; }
    public string? Image { get; set; }
    public int? Status { get; set; }
}