using Service.MangaOnline.Models;
using Service.MangaOnline.ResponseModels;

namespace Service.MangaOnline.Extensions;

public interface IMapObject
{ 
    MangaResponse MapMangaResponse(Manga manga);
    
    ChapterResponse MapChapterResponse(Chaptere manga);
}