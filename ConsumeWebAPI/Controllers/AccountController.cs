using ConsumeWebAPI.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace ConsumeWebAPI.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public AccountController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
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

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterRequestDTO registerRequestDTO)
        {
            var client = CreateHttpClient();
            var response = await client.PostAsJsonAsync("Account/Register", registerRequestDTO);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Login");
            }
            ModelState.AddModelError(string.Empty, "Registration failed");
            return View(registerRequestDTO);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDTO loginRequestDTO)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["ApiBaseUrl"]);
            var response = await client.PostAsJsonAsync("Account/Login", loginRequestDTO);
            if (response.IsSuccessStatusCode)
            {
                var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseDTO>();
                HttpContext.Session.SetString("JWToken", loginResponse.JwtToken);

                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(loginResponse.JwtToken);
                var username = token.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Email)?.Value;

                if (!string.IsNullOrEmpty(username))
                {
                    HttpContext.Session.SetString("Username", username);
                }

                return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError(string.Empty, "Login failed");
            return View(loginRequestDTO);
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
