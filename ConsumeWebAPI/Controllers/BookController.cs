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

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"Book/GetBook/{id}");

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                BookViewModel book = JsonConvert.DeserializeObject<BookViewModel>(data);
                return View(book);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            var viewModel = new BookViewModel
            {
                AuthorNames = new List<string>() 
            };
            return View(viewModel);
        }

   
        [HttpPost]
        public async Task<IActionResult> Create(BookViewModel book)
        {
            var content = new StringContent(JsonConvert.SerializeObject(book), System.Text.Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PostAsync("Book/AddBook", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index","Book");
            }
            else
            {
                return View(book);
            }
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"Book/GetBook/{id}");

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                BookViewModel book = JsonConvert.DeserializeObject<BookViewModel>(data);
                return View(book);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, BookViewModel book)
        {
            var content = new StringContent(JsonConvert.SerializeObject(book), System.Text.Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PutAsync($"Book/UpdateBook/{id}", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "Book");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Không thể chỉnh sửa sách. Vui lòng thử lại sau.");
                return View(book);
            }
        }



        // Xác nhận việc xóa một cuốn sách
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"Book/DeleteBook/{id}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "Book");
            }
            else
            {
                return NotFound();
            }
        }
    }
}
