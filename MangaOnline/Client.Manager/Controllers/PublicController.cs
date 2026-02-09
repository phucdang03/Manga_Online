using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Service.MangaOnline.ResponseModels;

namespace Client.Manager.Controllers;

public class PublicController : Controller
{
    private readonly HttpClient client = null;
    private string ServiceMangaUrl = "";
    private readonly ILogger<PublicController> _logger;

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

    public PublicController(ILogger<PublicController> logger)
    {
        _logger = logger;
        client = new HttpClient();
        var contentType = new MediaTypeWithQualityHeaderValue("application/json");
        client.DefaultRequestHeaders.Accept.Add(contentType);
        ServiceMangaUrl = "http://localhost:5098/";
    }
    public async Task<IActionResult> DetailChapter(Guid id)
    {
        _logger.LogInformation($"=== DETAIL CHAPTER REQUEST STARTED === ChapterID: {id}");
        
        ViewData["ChapterId"] = id;
        ViewData["MangaId"] = ""; // Default
        ViewData["MangaName"] = "";
        ViewData["ChapterName"] = "";
        ViewData["ServiceUrl"] = ServiceMangaUrl;

        try
        {
            // Check if service is reachable first
            _logger.LogInformation($"Testing service connectivity: {ServiceMangaUrl}");
            try
            {
                var healthCheck = await client.GetAsync(ServiceMangaUrl + "health", new CancellationTokenSource(TimeSpan.FromSeconds(5)).Token);
                _logger.LogInformation($"Service health check: {healthCheck.StatusCode}");
            }
            catch (Exception hEx)
            {
                _logger.LogWarning($"Health check failed: {hEx.Message}");
            }

            // Get chapter information to get the manga ID
            var apiUrl = ServiceMangaUrl + "manga/GetChapterInfo?id=" + id;
            _logger.LogInformation($"Calling API: {apiUrl}");

            HttpResponseMessage response = await client.GetAsync(apiUrl);
            _logger.LogInformation($"API Response Status: {response.StatusCode}");

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"GetChapterInfo Response: {responseBody}");
                Console.WriteLine($"GetChapterInfo Response: {responseBody}");

                var option = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                var chapterResponse = JsonSerializer.Deserialize<JsonElement>(responseBody, option);
                if (chapterResponse.TryGetProperty("success", out var successProp) && successProp.GetBoolean())
                {
                    if (chapterResponse.TryGetProperty("data", out var chapterData))
                    {
                        var mangaId = chapterData.GetProperty("mangaId").GetString();
                        ViewData["MangaId"] = mangaId;
                        ViewData["MangaName"] = chapterData.GetProperty("mangaName").GetString();
                        ViewData["ChapterName"] = chapterData.GetProperty("name").GetString();

                        _logger.LogInformation($"✓ Chapter data loaded - ID: {id}, MangaId: {ViewData["MangaId"]}, MangaName: {ViewData["MangaName"]}");
                        Console.WriteLine($"✓ Chapter loaded - ID: {id}, MangaId: {ViewData["MangaId"]}, MangaName: {ViewData["MangaName"]}");

                        // Get manga details with all chapters for navigation
                        if (!string.IsNullOrEmpty(mangaId))
                        {
                            try
                            {
                                var mangaApiUrl = ServiceMangaUrl + "manga/GetManga?id=" + mangaId;
                                _logger.LogInformation($"Fetching manga chapters: {mangaApiUrl}");

                                var mangaResponse = await client.GetAsync(mangaApiUrl);
                                if (mangaResponse.IsSuccessStatusCode)
                                {
                                    var mangaResponseBody = await mangaResponse.Content.ReadAsStringAsync();
                                    var mangaData = JsonSerializer.Deserialize<JsonElement>(mangaResponseBody, option);

                                    if (mangaData.TryGetProperty("success", out var mangaSuccess) && mangaSuccess.GetBoolean())
                                    {
                                        if (mangaData.TryGetProperty("data", out var manga) && manga.TryGetProperty("chapteres", out var chapters))
                                        {
                                            ViewData["Chapters"] = mangaResponseBody;
                                            _logger.LogInformation($"✓ Loaded {chapters.GetArrayLength()} chapters for navigation");
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogWarning($"Failed to load chapters for navigation: {ex.Message}");
                            }
                        }
                        else
                        {
                            _logger.LogWarning("Chapter API response missing data property");
                            ViewData["Error"] = "Dữ liệu chapter không hợp lệ";
                        }
                    }
                    else
                    {
                        var errorMsg = "Chapter API returned success=false";
                        _logger.LogWarning(errorMsg);
                        Console.WriteLine(errorMsg);
                        ViewData["Error"] = "Không thể tải thông tin chapter";
                    }
                }
                else
                {
                    var errorMsg = $"Failed to get chapter info: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}";
                    _logger.LogError(errorMsg);
                    Console.WriteLine(errorMsg);
                    ViewData["Error"] = $"Lỗi API: {response.StatusCode}";
                }
            }
        }
        catch (HttpRequestException ex)
        {
            var errorMsg = $"Network error loading chapter {id}: {ex.Message}";
            _logger.LogError(ex, errorMsg);
            Console.WriteLine(errorMsg);
            ViewData["Error"] = "Không thể kết nối tới server. Kiểm tra Service.MangaOnline có đang chạy không?";
        }
        catch (TaskCanceledException ex)
        {
            var errorMsg = $"Timeout loading chapter {id}: {ex.Message}";
            _logger.LogError(ex, errorMsg);
            Console.WriteLine(errorMsg);
            ViewData["Error"] = "Timeout khi tải dữ liệu. Vui lòng thử lại.";
        }
        catch (Exception ex)
        {
            var errorMsg = $"Unexpected error loading chapter {id}: {ex.Message}";
            _logger.LogError(ex, errorMsg);
            Console.WriteLine(errorMsg);
            ViewData["Error"] = $"Lỗi không xác định: {ex.Message}";
        }
        
        _logger.LogInformation($"=== DETAIL CHAPTER REQUEST COMPLETED === Returning view with Error: {ViewData["Error"]}");
        return View("DetailChapter");
    }
    
    public async Task<IActionResult> DetailManga(Guid id)
    {
        try 
        {
            _logger.LogInformation($"Loading manga details for ID: {id}");
            HttpResponseMessage response = await client.GetAsync(ServiceMangaUrl + "manga/GetManga?id="+ id);
            string responseBody = await response.Content.ReadAsStringAsync();
            _logger.LogInformation($"DetailManga API response: {response.StatusCode} - {responseBody}");
            
            var option = new JsonSerializerOptions
                { PropertyNameCaseInsensitive = true };
            if (response.IsSuccessStatusCode)
            {
                var dataMangaResponse = JsonSerializer.Deserialize<DataMangaResponse>(responseBody, option);
                ViewData["manga"] = dataMangaResponse!.data;
                return View("DetailManga");
            }
            else 
            {
                _logger.LogError($"DetailManga API failed: {response.StatusCode} - {responseBody}");
                ViewData["ErrorMessage"] = $"Không thể tải thông tin manga. Status: {response.StatusCode}";
                return View("Error");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error loading manga {id}");
            ViewData["ErrorMessage"] = $"Lỗi: {ex.Message}";
            return View("Error");
        }
    }
    
    // Image proxy to handle missing images
    [HttpGet("/image/manga-image/{fileName}")]
    public async Task<IActionResult> GetMangaImage(string fileName)
    {
        try
        {
            _logger.LogInformation($"Requesting image: {fileName}");
            
            // Try to get image from the service API
            var imageUrl = $"{ServiceMangaUrl}File/GetImage?fileName={fileName}";
            _logger.LogInformation($"Fetching from: {imageUrl}");
            
            var response = await client.GetAsync(imageUrl);
            _logger.LogInformation($"Image API response: {response.StatusCode}");
            
            if (response.IsSuccessStatusCode)
            {
                var imageBytes = await response.Content.ReadAsByteArrayAsync();
                var contentType = response.Content.Headers.ContentType?.ToString() ?? "image/jpeg";
                _logger.LogInformation($"✓ Successfully loaded image {fileName}, size: {imageBytes.Length} bytes");
                return File(imageBytes, contentType);
            }
            
            _logger.LogWarning($"Image not found: {fileName} - Status: {response.StatusCode}");
            return NotFound($"Image {fileName} not found - Status: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            var errorMsg = $"Error loading image {fileName}: {ex.Message}";
            _logger.LogError(ex, errorMsg);
            Console.WriteLine(errorMsg);
            return StatusCode(500, errorMsg);
        }
    }
    
    // Add health check endpoint
    [HttpGet("/health")]
    public IActionResult Health()
    {
        return Ok(new { status = "ok", timestamp = DateTime.Now, service = "Client.Manager" });
    }
}
