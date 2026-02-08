using Microsoft.AspNetCore.Mvc;
using Service.MangaOnline.ResponseModels;
using System.Net.Http.Headers;
using System.Text.Json;
using Service.MangaOnline.Commons;
using Service.MangaOnline.Models;

namespace Client.Manager.Controllers;

public class ManagerController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly HttpClient client = null;
    private string ServiceMangaUrl = "";

    public ManagerController(ILogger<HomeController> logger)
    {
        client = new HttpClient();
        var contentType = new MediaTypeWithQualityHeaderValue("application/json");
        client.DefaultRequestHeaders.Accept.Add(contentType);
        ServiceMangaUrl = "http://localhost:5098/";
        _logger = logger;
    }
    // GET
    public IActionResult AddManga()
    {
        return View();
    }

    public IActionResult UpdateManga(Guid id)
    {
        return View("AddManga");
    }
    public async Task<IActionResult> ReadingHistory(Guid userId)
    {
        HttpResponseMessage response = await client.GetAsync(ServiceMangaUrl + "manga/ReadingHistory?userId=" + userId);
        string responseBody = await response.Content.ReadAsStringAsync();
        var option = new JsonSerializerOptions()
        { PropertyNameCaseInsensitive = true };
        var list = JsonSerializer.Deserialize<ReadingHistoryRespone>(responseBody, option);
        ViewBag.list = list;
        return View("ReadingHistory");
    }
    public async Task<IActionResult> AddReadingHistory(Guid userId, Guid mangaId)
    {
        try 
        {
            // Simply add to reading history - API will handle duplicates
            var postResponse = await client.PostAsync(ServiceMangaUrl + "manga/ReadingHistory?userId=" + userId + "&mangaId=" + mangaId, null);
            var responseContent = await postResponse.Content.ReadAsStringAsync();
            
            if (postResponse.IsSuccessStatusCode)
            {
                return Json(new { success = true, message = "Added to reading history", data = responseContent });
            }
            else 
            {
                return Json(new { success = false, message = "Failed to add to history", error = responseContent });
            }
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "Error: " + ex.Message });
        }
    }
    public async Task<IActionResult> Follow(Guid userId)
    {
        HttpResponseMessage response = await client.GetAsync(ServiceMangaUrl + "manga/FollowManga?userId="+ userId);
            string responseBody = await response.Content.ReadAsStringAsync();
            var option = new JsonSerializerOptions()
            { PropertyNameCaseInsensitive = true };
            var list = JsonSerializer.Deserialize<FollowResponse>(responseBody, option);
            ViewBag.list = list;
        return View("FollowManga");
    }

    public async Task<IActionResult> UnFollow(Guid userId,Guid mangaId)
    {
        HttpResponseMessage response = await client.DeleteAsync(ServiceMangaUrl + "manga/FollowManga?userId="+ userId+ "&&mangaId="+mangaId);
        return Redirect("follow?userId="+userId);
    }

    public async Task<IActionResult> ViewAddChapter(Guid id)
    {
        var response = await client.GetAsync(ServiceMangaUrl + "manga/GetManga?id="+ id);
        string responseBody = await response.Content.ReadAsStringAsync();
        var option = new JsonSerializerOptions
            { PropertyNameCaseInsensitive = true };
        var data = JsonSerializer.Deserialize<DataMangaResponse>(responseBody, option);
        ViewData["manga"] = data!.data;
        return View("AddChapter");
    }

    public async Task<IActionResult> AddChapter()
    {
        IFormFile file = Request.Form.Files.GetFile("fileUp")!;

        Chaptere chaptere = new Chaptere();
        chaptere.ChapterNumber = int.Parse(Request.Form["ChapNumber"]);
        chaptere.Id = Guid.NewGuid();
        chaptere.SubId = 0;
        chaptere.MangaId = Guid.Parse(Request.Form["mangaId"]);
        chaptere.Name = " Chapter "+ Request.Form["ChapNumber"];
        chaptere.CreatedAt = DateTimeOffset.Now;
        chaptere.Status = Int32.Parse(Request.Form["Status"]);
        chaptere.IsActive = true;
        //chaptere.FilePDF = _logicHandler.CreatePDF(file);
        
        string apiEndpoint = "http://localhost:5098/File/CreateImage";
        var formData = new MultipartFormDataContent();
        formData.Add(new StreamContent(file.OpenReadStream()), "imageFile", file.FileName);
        var response = await client.PostAsync(apiEndpoint, formData);
        if (response.IsSuccessStatusCode)
        {
            string responseBody = await response.Content.ReadAsStringAsync();
            var option = new JsonSerializerOptions
                { PropertyNameCaseInsensitive = true };
            var data = JsonSerializer.Deserialize<DataResponse>(responseBody, option);
            chaptere.FilePdf = data!.data;
            
            string apiC = "http://localhost:5098/Manga/AddChapter";
            var formContent = new FormUrlEncodedContent(new[] {
                new KeyValuePair<string,string>("ChapterNumber", chaptere.ChapterNumber.ToString()),
                new KeyValuePair<string,string>("SubId", chaptere.SubId.ToString()),
                new KeyValuePair<string,string>("MangaId", chaptere.MangaId.ToString()),
                new KeyValuePair<string,string>("Name", chaptere.Name),
                new KeyValuePair<string,string>("Status", chaptere.Status.ToString()),
                new KeyValuePair<string,string>("IsActive", chaptere.IsActive.ToString()),
                new KeyValuePair<string,string>("FilePDF", chaptere.FilePdf)
            });
            var responseC = await client.PostAsync(apiC, formContent);
            if (responseC.IsSuccessStatusCode)
            {
                return Redirect("/Public/DetailManga?id="+Request.Form["mangaId"]); 
            } 
        }
        return View("Error");
    }

    public async Task<IActionResult> ListManga(string? genre, 
        string? status, string? statusOff, string? sort,int index)
    {
        genre = string.IsNullOrEmpty(genre) ? "Tất cả":genre;
        status = string.IsNullOrEmpty(status) ? "Tất cả" : status;
        statusOff = string.IsNullOrEmpty(statusOff) ? "Tất cả" : statusOff;
        sort = string.IsNullOrEmpty(sort) ? "Tất cả" : sort;
        HttpResponseMessage response = 
            await client.GetAsync(
                ServiceMangaUrl 
                + $"Manga/listManga?genre={genre}&status={status}&statusOff={statusOff}&sort={sort}&index={index}");
        string responseBody = await response.Content.ReadAsStringAsync();
        var option = new JsonSerializerOptions()
            { PropertyNameCaseInsensitive = true };
        var responseData = JsonSerializer.Deserialize<DataListMangaResponse>(responseBody, option);
        ViewData["ListManga"] = responseData!.data;
        ViewData["genre"] = genre;
        ViewData["status"] = status;
        ViewData["statusOff"] = statusOff;
        ViewData["sort"] = sort;
        ViewData["index"] = index==0?1:index;
        ViewData["LastPage"] = responseData.lastPage;
        return View("ListMangaManager");
    }
}