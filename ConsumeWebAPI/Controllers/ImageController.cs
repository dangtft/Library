using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using ConsumeWebAPI.DTOs;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;

namespace ConsumeWebAPI.Controllers
{
    public class ImageController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public ImageController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        private HttpClient CreateHttpClient()
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["ApiBaseUrl"]);
            var token = HttpContext.Session.GetString("JWToken");
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            return client;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(ImageUploadRequestDTO request)
        {
            if (request.File != null && request.File.Length > 0)
            {
                request.FileName = request.File.FileName;
                var client = CreateHttpClient();
                var formData = new MultipartFormDataContent();
                formData.Add(new StreamContent(request.File.OpenReadStream()), "File", request.FileName);
                formData.Add(new StringContent(request.FileName), "FileName");
                formData.Add(new StringContent(request.FileDescription), "FileDescription");

                var response = await client.PostAsync("Image/Upload", formData);
                if (response.IsSuccessStatusCode)
                {
                    TempData["UploadSuccess"] = "Image uploaded successfully!";
                    return RedirectToAction("Index");
                }
            }
            ModelState.AddModelError(string.Empty, "Upload failed. Please select a file.");
            return View(request);
        }

        [HttpGet]
        public IActionResult GetAllInfoImages()
        {
            var client = CreateHttpClient();
            var response = client.GetAsync("Image/GetAllInfoImages").Result;
            if (response.IsSuccessStatusCode)
            {
                var images = response.Content.ReadAsAsync<List<ImageInfoDTO>>().Result;
                return View(images);
            }
            return View(new List<ImageInfoDTO>());
        }

        [HttpGet]
        public IActionResult Download(int id)
        {
            var client = CreateHttpClient();
            var response = client.GetAsync($"Image/Download?id={id}").Result;
            if (response.IsSuccessStatusCode)
            {
                var fileBytes = response.Content.ReadAsByteArrayAsync().Result;
                var contentType = response.Content.Headers.ContentType.MediaType;
                var fileName = response.Content.Headers.ContentDisposition.FileName;

                return File(fileBytes, contentType, fileName);
            }
            // Handle error if needed
            return RedirectToAction("Index", "Home");
        }
    }
}
