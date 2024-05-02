using ConsumeWebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ConsumeWebAPI.Controllers
{
    public class BookController : Controller
    {
        private readonly HttpClient _httpClient;

        public BookController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7112/api/");
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<BookViewModel> books = new List<BookViewModel>();
            HttpResponseMessage response = await _httpClient.GetAsync("Book/GetBooks/get-all-book");

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                books = JsonConvert.DeserializeObject<List<BookViewModel>>(data);
            }
            return View(books);
        }
    }
}
