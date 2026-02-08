namespace Service.MangaOnline.ResponseModels;

public class ChapterResponse
{
    public Guid Id { get; set; }
    public long SubId { get; set; }
    public Guid MangaId { get; set; }
    public int ChapterNumber { get; set; }
    public string Name { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; }
    public int Status { get; set; }
    public bool IsActive { get; set; }
    public string? FilePdf { get; set; }
}