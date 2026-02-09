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

    public async Task<IActionResult> DeleteChapter(Guid chapterId, Guid? mangaId = null)
    {
        try
        {
            _logger.LogInformation($"=== DELETE CHAPTER REQUEST === ChapterID: {chapterId}, MangaID: {mangaId}");
            
            // Call the delete API
            var deleteUrl = ServiceMangaUrl + $"Manga/DeleteChapter?chapterId={chapterId}";
            _logger.LogInformation($"Calling delete API: {deleteUrl}");
            
            HttpResponseMessage response = await client.DeleteAsync(deleteUrl);
            string responseBody = await response.Content.ReadAsStringAsync();
            _logger.LogInformation($"Delete API response: {response.StatusCode} - {responseBody}");
            
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            
            if (response.IsSuccessStatusCode)
            {
                var deleteResult = JsonSerializer.Deserialize<JsonElement>(responseBody, options);
                
                if (deleteResult.TryGetProperty("success", out var successProp) && successProp.GetBoolean())
                {
                    string successMessage = "Xóa chapter thành công";
                    if (deleteResult.TryGetProperty("message", out var msgProp))
                    {
                        successMessage = msgProp.GetString() ?? successMessage;
                    }
                    
                    _logger.LogInformation($"✓ Chapter {chapterId} deleted successfully");
                    
                    // Set success message for next page
                    TempData["SuccessMessage"] = successMessage;
                    
                    return Json(new { 
                        success = true, 
                        message = successMessage,
                        chapterId = chapterId,
                        data = deleteResult.GetProperty("deletedChapter")
                    });
                }
                else 
                {
                    string errorMessage = "Xóa chapter thất bại";
                    if (deleteResult.TryGetProperty("message", out var msgProp))
                    {
                        errorMessage = msgProp.GetString() ?? errorMessage;
                    }
                    
                    _logger.LogError($"Delete API returned success=false: {errorMessage}");
                    TempData["ErrorMessage"] = errorMessage;
                    
                    return Json(new { 
                        success = false, 
                        message = errorMessage 
                    });
                }
            }
            else
            {
                string errorMessage = $"Lỗi server khi xóa chapter: {response.StatusCode}";
                
                // Try to parse error message from API
                try
                {
                    var errorResult = JsonSerializer.Deserialize<JsonElement>(responseBody, options);
                    if (errorResult.TryGetProperty("message", out var msgProp))
                    {
                        errorMessage = msgProp.GetString() ?? errorMessage;
                    }
                }
                catch
                {
                    // Use default error message if parsing fails
                }
                
                _logger.LogError($"Delete API failed: {response.StatusCode} - {responseBody}");
                TempData["ErrorMessage"] = errorMessage;
                
                return Json(new { 
                    success = false, 
                    message = errorMessage,
                    statusCode = (int)response.StatusCode
                });
            }
        }
        catch (HttpRequestException ex)
        {
            var errorMsg = "Không thể kết nối tới server. Kiểm tra Service.MangaOnline có đang chạy không?";
            _logger.LogError(ex, $"Network error deleting chapter {chapterId}");
            TempData["ErrorMessage"] = errorMsg;
            
            return Json(new { 
                success = false, 
                message = errorMsg,
                error = "NetworkError"
            });
        }
        catch (TaskCanceledException ex)
        {
            var errorMsg = "Timeout khi xóa chapter. Server phản hồi quá chậm.";
            _logger.LogError(ex, $"Timeout deleting chapter {chapterId}");
            TempData["ErrorMessage"] = errorMsg;
            
            return Json(new { 
                success = false, 
                message = errorMsg,
                error = "Timeout"  
            });
        }
        catch (Exception ex)
        {
            var errorMsg = $"Lỗi không xác định khi xóa chapter: {ex.Message}";
            _logger.LogError(ex, $"Unexpected error deleting chapter {chapterId}");
            TempData["ErrorMessage"] = errorMsg;
            
            return Json(new { 
                success = false, 
                message = errorMsg,
                error = "UnexpectedError"
            });
        }
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

    [HttpPost]
    public async Task<IActionResult> AddChapter()
    {
        try
        {
            // Validate Chapter Number
            if (!int.TryParse(Request.Form["ChapNumber"], out int chapterNumber) || chapterNumber <= 0)
            {
                TempData["ErrorMessage"] = "Số chapter phải là số nguyên dương.";
                return await ReturnToAddChapterView();
            }

            // Validate Status
            if (!int.TryParse(Request.Form["Status"], out int status))
            {
                TempData["ErrorMessage"] = "Trạng thái chapter không hợp lệ.";
                return await ReturnToAddChapterView();
            }

            // Validate Manga ID
            if (!Guid.TryParse(Request.Form["mangaId"], out Guid mangaId))
            {
                TempData["ErrorMessage"] = "ID manga không hợp lệ.";
                return await ReturnToAddChapterView();
            }

            // Check if chapter number already exists for this manga
            _logger.LogInformation($"Checking if chapter {chapterNumber} already exists for manga {mangaId}");
            try
            {
                var checkChapterUrl = ServiceMangaUrl + $"manga/CheckChapterExists?mangaId={mangaId}&chapterNumber={chapterNumber}";
                var checkResponse = await client.GetAsync(checkChapterUrl);
                
                if (checkResponse.IsSuccessStatusCode)
                {
                    string checkResponseBody = await checkResponse.Content.ReadAsStringAsync();
                    var checkOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var checkResult = JsonSerializer.Deserialize<JsonElement>(checkResponseBody, checkOptions);
                    
                    if (checkResult.TryGetProperty("exists", out var existsProperty) && existsProperty.GetBoolean())
                    {
                        _logger.LogWarning($"Chapter {chapterNumber} already exists for manga {mangaId}");
                        TempData["ErrorMessage"] = $"Chapter {chapterNumber} đã tồn tại cho manga này. Vui lòng chọn số chapter khác.";
                        return await ReturnToAddChapterView();
                    }
                    
                    _logger.LogInformation($"✓ Chapter {chapterNumber} is available for manga {mangaId}");
                }
                else
                {
                    // If the API doesn't exist yet, try alternative method - get all chapters and check manually
                    _logger.LogInformation("CheckChapterExists API not available, using fallback method");
                    var getMangaUrl = ServiceMangaUrl + $"manga/GetManga?id={mangaId}";
                    var getMangaResponse = await client.GetAsync(getMangaUrl);
                    
                    if (getMangaResponse.IsSuccessStatusCode)
                    {
                        string getMangaResponseBody = await getMangaResponse.Content.ReadAsStringAsync();
                        var getMangaOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                        var mangaResult = JsonSerializer.Deserialize<JsonElement>(getMangaResponseBody, getMangaOptions);
                        
                        if (mangaResult.TryGetProperty("data", out var mangaData) &&
                            mangaData.TryGetProperty("chapteres", out var chapters))
                        {
                            foreach (var chapter in chapters.EnumerateArray())
                            {
                                if (chapter.TryGetProperty("chapterNumber", out var chapterNum) && 
                                    chapterNum.GetInt32() == chapterNumber)
                                {
                                    _logger.LogWarning($"Chapter {chapterNumber} already exists for manga {mangaId} (found via fallback)");
                                    TempData["ErrorMessage"] = $"Chapter {chapterNumber} đã tồn tại cho manga này. Vui lòng chọn số chapter khác.";
                                    return await ReturnToAddChapterView();
                                }
                            }
                            _logger.LogInformation($"✓ Chapter {chapterNumber} is available for manga {mangaId} (verified via fallback)");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking chapter existence for manga {mangaId}, chapter {chapterNumber}");
                // Continue without blocking - this is a validation enhancement, not critical
                _logger.LogWarning("Continuing with chapter creation despite validation check failure");
            }

            // Validate File Upload
            IFormFile file = Request.Form.Files.GetFile("fileUp");
            if (file == null || file.Length == 0)
            {
                TempData["ErrorMessage"] = "Vui lòng chọn file để upload.";
                return await ReturnToAddChapterView();
            }

            // Validate file type
            var allowedExtensions = new[] { ".pdf", ".png", ".jpg", ".jpeg" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            if (!allowedExtensions.Contains(fileExtension))
            {
                TempData["ErrorMessage"] = "Chỉ chấp nhận file PDF, PNG, JPG, JPEG.";
                return await ReturnToAddChapterView();
            }

            // Validate file size (max 50MB)
            if (file.Length > 50 * 1024 * 1024)
            {
                TempData["ErrorMessage"] = "File không được vượt quá 50MB.";
                return await ReturnToAddChapterView();
            }

            // Create Chapter object
            Chaptere chaptere = new Chaptere
            {
                Id = Guid.NewGuid(),
                ChapterNumber = chapterNumber,
                SubId = 0,
                MangaId = mangaId,
                Name = $"Chapter {chapterNumber}",
                CreatedAt = DateTimeOffset.Now,
                Status = status,
                IsActive = true
            };

            // Upload file to service
            _logger.LogInformation($"Uploading file for chapter {chapterNumber} of manga {mangaId}");
            
            string apiEndpoint = ServiceMangaUrl + "File/CreateImage";
            using var formData = new MultipartFormDataContent();
            using var fileStream = file.OpenReadStream();
            formData.Add(new StreamContent(fileStream), "imageFile", file.FileName);
            
            var uploadResponse = await client.PostAsync(apiEndpoint, formData);
            
            if (!uploadResponse.IsSuccessStatusCode)
            {
                string errorContent = await uploadResponse.Content.ReadAsStringAsync();
                _logger.LogError($"File upload failed: {uploadResponse.StatusCode} - {errorContent}");
                TempData["ErrorMessage"] = "Upload file thất bại. Vui lòng thử lại.";
                return await ReturnToAddChapterView();
            }

            // Parse upload response
            string uploadResponseBody = await uploadResponse.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var uploadData = JsonSerializer.Deserialize<DataResponse>(uploadResponseBody, options);
            
            if (uploadData?.data == null)
            {
                _logger.LogError("Upload response data is null");
                TempData["ErrorMessage"] = "Upload file thất bại. Vui lòng thử lại.";
                return await ReturnToAddChapterView();
            }

            chaptere.FilePdf = uploadData.data;

            // Add chapter to database
            _logger.LogInformation($"Adding chapter {chapterNumber} to database");
            
            string addChapterApi = ServiceMangaUrl + "Manga/AddChapter";
            var chapterFormContent = new FormUrlEncodedContent(new[] {
                new KeyValuePair<string,string>("ChapterNumber", chaptere.ChapterNumber.ToString()),
                new KeyValuePair<string,string>("SubId", chaptere.SubId.ToString()),
                new KeyValuePair<string,string>("MangaId", chaptere.MangaId.ToString()),
                new KeyValuePair<string,string>("Name", chaptere.Name),
                new KeyValuePair<string,string>("Status", chaptere.Status.ToString()),
                new KeyValuePair<string,string>("IsActive", chaptere.IsActive.ToString()),
                new KeyValuePair<string,string>("FilePDF", chaptere.FilePdf)
            });

            var addChapterResponse = await client.PostAsync(addChapterApi, chapterFormContent);
            
            if (!addChapterResponse.IsSuccessStatusCode)
            {
                string errorContent = await addChapterResponse.Content.ReadAsStringAsync();
                _logger.LogError($"Add chapter failed: {addChapterResponse.StatusCode} - {errorContent}");
                TempData["ErrorMessage"] = "Thêm chapter thất bại. Vui lòng thử lại.";
                return await ReturnToAddChapterView();
            }

            // Success
            _logger.LogInformation($"Successfully added chapter {chapterNumber} for manga {mangaId}");
            TempData["SuccessMessage"] = $"Thêm chapter {chapterNumber} thành công!";
            
            return Redirect($"/Public/DetailManga?id={mangaId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while adding chapter");
            TempData["ErrorMessage"] = "Có lỗi xảy ra. Vui lòng thử lại.";
            return await ReturnToAddChapterView();
        }
    }

    private async Task<IActionResult> ReturnToAddChapterView()
    {
        if (Guid.TryParse(Request.Form["mangaId"], out Guid mangaId))
        {
            return await ViewAddChapter(mangaId);
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