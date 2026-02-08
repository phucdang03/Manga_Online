using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Service.MangaOnline.Commons;
using Service.MangaOnline.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Client.Manager.Controllers;

public class AuthController : Controller
{
    
    private readonly ILogger<HomeController> _logger;
    private readonly HttpClient client = null;
    private string ServiceMangaUrl = "";
    [BindProperty] public String gmailUserUpdate { get; set; }
    [BindProperty] public String hashForm { get; set; }
    [BindProperty] public String fromForm { get; set; }
    [BindProperty] public String toForm { get; set; }
    [BindProperty] public String valueForm { get; set; }

    public AuthController(ILogger<HomeController> logger)
    {
        client = new HttpClient();
        var contentType = new MediaTypeWithQualityHeaderValue("application/json");
        client.DefaultRequestHeaders.Accept.Add(contentType);
        ServiceMangaUrl = "http://localhost:5098/";
        _logger = logger;
    }

    

    [HttpGet]
    public IActionResult AuthLogin(String? noti)
    {
        if (noti is null)
        {
            return View("AuthLogin");
        }

        ViewData["notice"] = "abc";
        return View("AuthLogin");

    }
    [HttpGet]
    public IActionResult AuthChangePassword()
    {
        return View("AuthChangePassword");
    }
    [HttpGet]
    public IActionResult AuthRegistration()
    {
        return View("AuthRegistration");
    }
    [HttpGet]
    public IActionResult AuthVerifyEmail()
    {
        return View("AuthVerifyEmail");
    }

    [HttpGet]
    public IActionResult HistoryAccount()
    {
        return View("HistoryAccount");
    }

    [HttpGet]
    public IActionResult UpdateAccount(String? noti)
    {
        if (noti is null)
        {
            return View("UpdateAccount");
        }

        ViewData["notice"] = noti;
        return View("UpdateAccount");
    }

    [HttpPost]
    public async Task<IActionResult> PostUpdateAccountAsync()
    {

        if (gmailUserUpdate != null)
        {
            var content = new StringContent(String.Empty ,Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PutAsync(ServiceMangaUrl + "Auth/UpdateRole?gmail=" + gmailUserUpdate, content);
            if (response.IsSuccessStatusCode)
            {
                var formContent = new FormUrlEncodedContent(new[] {
                    new KeyValuePair<string,string>("Id", Guid.NewGuid().ToString()),
                    new KeyValuePair<string,string>("User", gmailUserUpdate),
                    new KeyValuePair<string,string>("Hash", hashForm),
                    new KeyValuePair<string,string>("From", fromForm),
                    new KeyValuePair<string,string>("To", toForm),
                    new KeyValuePair<string,string>("Value", valueForm),
                    new KeyValuePair<string,string>("Date", DateTime.Now.ToString()),
            });

                HttpResponseMessage response2 = await client.PostAsync(ServiceMangaUrl + "Auth/CreateTransaction", formContent);
                if (response2.IsSuccessStatusCode)
                {
                    Response.Cookies.Delete("ACCESS_TOKEN");
                    Response.Cookies.Delete("USER_DATA");
                    return AuthLogin("Chúc mừng nâng cấp thành công");
                }
            };
        }
        return UpdateAccount("false");
    }
}