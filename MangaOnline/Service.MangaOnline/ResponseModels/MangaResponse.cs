using Service.MangaOnline.Models;

namespace Service.MangaOnline.ResponseModels;

public class MangaResponse
{
    public Guid Id { get; set; }
    public Guid AuthorId { get; set; }
    public int Status { get; set; }
    public int ViewCount { get; set; }
    public int RateCount { get; set; }
    public int Star { get; set; }
    public int FollowCount { get; set; }
    public string Description { get; set; } = null!;
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? ModifiedAt { get; set; }
    public bool IsActive { get; set; }
    public string? Image { get; set; }
    public string? Name { get; set; }
    public List<Category>? CategoryMangas { get; set; }
    public List<ChapterResponse>? Chapteres { get; set; }
    public int CountChapter { get; set; }
    public string? AuthorName { get; set; }
} 

public class DataMangaResponse
{
    public int status { get; set; }
    public bool success { get; set; }
    public MangaResponse data { get; set; }
}

public class DataListMangaResponse
{
    public int status { get; set; }
    public bool success { get; set; }
    public List<MangaResponse> data { get; set; }
    public int lastPage { get; set; }
}