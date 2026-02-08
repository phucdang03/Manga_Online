namespace Service.MangaOnline.ResponseModels;

public class HomeResponse
{
    public List<MangaResponse> TopMonthManga { get; set; } = new List<MangaResponse>();
    public List<MangaResponse> NewUpdateMangas { get; set; } = new List<MangaResponse>();
    public List<MangaResponse> NewDoneMangas { get; set; } = new List<MangaResponse>();
    public List<MangaResponse> TopViewMangas { get; set; } = new List<MangaResponse>();
}