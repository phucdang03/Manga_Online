using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Service.MangaOnline.ResponseModels;

namespace Client.Manager.Controllers;

public class PublicController : Controller
{
    private readonly HttpClient client = null;
    private string ServiceMangaUrl = "";

    public IActionResult Search(string? query = null, string? categoryName = null)
    {
        // Pass search parameters to view if provided
        if (!string.IsNullOrEmpty(query))
        {
            ViewData["SearchQuery"] = query;
        }
        
        if (!string.IsNullOrEmpty(categoryName))
        {
            ViewData["CategoryName"] = categoryName;
        }
        
        // Set the API URL for the view to use
        ViewData["ServiceMangaUrl"] = ServiceMangaUrl.TrimEnd('/');
        
        return View();
    }

    public PublicController()
    {
        client = new HttpClient();
        var contentType = new MediaTypeWithQualityHeaderValue("application/json");
        client.DefaultRequestHeaders.Accept.Add(contentType);
        ServiceMangaUrl = "http://localhost:5098/";
    }
    public async Task<IActionResult> DetailChapter(Guid id)
    {
        ViewData["ChapterId"] = id;
        ViewData["MangaId"] = ""; // Default
        ViewData["MangaName"] = "";
        ViewData["ChapterName"] = "";
        
        try 
        {
            // Get chapter information to get the manga ID
            HttpResponseMessage response = await client.GetAsync(ServiceMangaUrl + "manga/GetChapterInfo?id=" + id);
            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"GetChapterInfo Response: {responseBody}");
                
                var option = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                
                var chapterResponse = JsonSerializer.Deserialize<JsonElement>(responseBody, option);
                if (chapterResponse.TryGetProperty("success", out var successProp) && successProp.GetBoolean())
                {
                    if (chapterResponse.TryGetProperty("data", out var chapterData))
                    {
                        ViewData["MangaId"] = chapterData.GetProperty("mangaId").GetString();
                        ViewData["MangaName"] = chapterData.GetProperty("mangaName").GetString();
                        ViewData["ChapterName"] = chapterData.GetProperty("name").GetString();
                        
                        // Debug logging
                        Console.WriteLine($"Chapter loaded - ID: {id}, MangaId: {ViewData["MangaId"]}, MangaName: {ViewData["MangaName"]}");
                    }
                }
                else 
                {
                    Console.WriteLine($"Chapter API returned success=false");
                }
            }
            else 
            {
                Console.WriteLine($"Failed to get chapter info: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            // Log error but continue to show the view
            Console.WriteLine($"Error loading chapter: {ex.Message}");
            ViewData["Error"] = ex.Message;
        }
        
        return View("DetailChapter");
    }
    
    public async Task<IActionResult> DetailManga(Guid id)
    {
        HttpResponseMessage response = await client.GetAsync(ServiceMangaUrl + "manga/GetManga?id="+ id);
        string responseBody = await response.Content.ReadAsStringAsync();
        var option = new JsonSerializerOptions
            { PropertyNameCaseInsensitive = true };
        if (response.IsSuccessStatusCode)
        {
            var dataMangaResponse = JsonSerializer.Deserialize<DataMangaResponse>(responseBody, option);
            ViewData["manga"] = dataMangaResponse!.data;
            return View("DetailManga");
        }

        return View("Error");
    }
}
