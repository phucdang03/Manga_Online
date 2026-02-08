using Microsoft.AspNetCore.Mvc;
using Service.MangaOnline.Models;
using Service.MangaOnline.ResponseModels;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Client.Manager.Controllers
{
    public class ProfileController : Controller
    {
        private string baseUrl = "http://localhost:5098/api/";
        private readonly HttpClient client = null;

        public ProfileController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);

        }

        [HttpGet]
        public async Task<IActionResult> Index(Guid id)
        {
            HttpResponseMessage response = await client.GetAsync(baseUrl + "User?id=" + id);
            string responseBody = await response.Content.ReadAsStringAsync();
            var option = new JsonSerializerOptions()
            { PropertyNameCaseInsensitive = true };
            var user = JsonSerializer.Deserialize<User>(responseBody, option);

            HttpResponseMessage response2 = await client.GetAsync(baseUrl + "User/GetUserToken/" + id);
            string responseBody2 = await response2.Content.ReadAsStringAsync();
            var option2 = new JsonSerializerOptions();
            UserToken? token = JsonSerializer.Deserialize<UserToken>(responseBody2, option2);
            ViewBag.ExpiresTime = token.Expires;



            return View("/Views/Manager/UserProfile.cshtml", user);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(ProfileUser collection)
        {
            IFormFile? file = Request.Form.Files.GetFile("fileUp");
            if(file != null)
            {
                var formData = new MultipartFormDataContent();
                formData.Add(new StreamContent(file!.OpenReadStream()), "imageFile", file.FileName);
                var responseImg = await client.PostAsync("http://localhost:5098/" + "File/CreateImage", formData);

                if (responseImg.IsSuccessStatusCode)
                {
                    string responseBodyImg = await responseImg.Content.ReadAsStringAsync();
                    var optionImg = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var data = JsonSerializer.Deserialize<DataResponse>(responseBodyImg, optionImg);
                    collection.Avatar = data!.data;
                }
            }

            HttpResponseMessage response = await client.PostAsJsonAsync(baseUrl + "User/UpdateUser", collection);
            string responseBody = await response.Content.ReadAsStringAsync();
            var option = new JsonSerializerOptions()
            { PropertyNameCaseInsensitive = true };

            // return Redirect("/profile?id=" + collection.Id);
            return Redirect("/Auth/AuthLogin");
        }
    }
}
