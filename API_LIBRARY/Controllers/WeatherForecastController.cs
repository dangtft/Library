using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace API_LIBRARY.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly string _apiKey = "b3bfc69bb6434d02bbf94519240805";
        private readonly HttpClient _httpClient;

        public WeatherForecastController()
        {
            _httpClient = new HttpClient();
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<IActionResult> GetWeatherForecast([FromQuery] string city)
        {
            if (string.IsNullOrEmpty(city))
            {
                return BadRequest("City name cannot be empty");
            }

            string apiUrl = $"http://api.worldweatheronline.com/premium/v1/weather.ashx?q={city}&key={_apiKey}&format=json";

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var weatherData = JsonSerializer.Deserialize<WorldWeatherOnlineResponse>(responseBody);

                    if (weatherData != null && weatherData.data != null && weatherData.data.weather != null)
                    {
                        var weatherForecasts = new List<WeatherForecast>();

                        // Lấy dự báo thời tiết cho tối đa 5 ngày tiếp theo
                        for (int i = 1; i < Math.Min(6, weatherData.data.weather.Length); i++)
                        {
                            var weather = weatherData.data.weather[i];

                            // Chuyển đổi dữ liệu từ WorldWeatherOnlineResponse thành WeatherForecast
                            var forecast = new WeatherForecast
                            {
                                Date = DateTime.Now.AddDays(i),
                                TemperatureC = int.TryParse(weather.avgtempC, out int temp) ? temp : 0,
                                Summary = weather.weatherDesc?[0]?.value ?? "N/A"
                            };

                            weatherForecasts.Add(forecast);
                        }

                        return Ok(weatherForecasts);
                    }
                    else
                    {
                        // Trả về một đối tượng BadRequestResult nếu dữ liệu phản hồi không hợp lệ
                        return BadRequest();
                    }
                }
                else
                {
                    // Trả về một đối tượng BadRequestResult nếu không thành công
                    return BadRequest();
                }
            }
            catch (HttpRequestException)
            {
                // Trả về một đối tượng BadRequestResult nếu có lỗi trong quá trình gửi yêu cầu
                return BadRequest();
            }
        }
    }

    public class WorldWeatherOnlineResponse
    {
        public WeatherData data { get; set; }
    }

    public class WeatherData
    {
        public Weather[] weather { get; set; }
    }

    public class Weather
    {
        public string avgtempC { get; set; }
        public WeatherDesc[] weatherDesc { get; set; }
    }

    public class WeatherDesc
    {
        public string value { get; set; }
    }

    public class WeatherForecast
    {
        public DateTime Date { get; set; }
        public int TemperatureC { get; set; }
        public string Summary { get; set; }
    }
}
