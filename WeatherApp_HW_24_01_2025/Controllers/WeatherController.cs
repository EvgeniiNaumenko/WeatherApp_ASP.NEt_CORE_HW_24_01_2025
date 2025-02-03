using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using WeatherApp_HW_24_01_2025.Models;

namespace WeatherApp_HW_24_01_2025.Controllers
{
    public class WeatherController : Controller
    {
        private readonly HttpClient _httpClient;
        private const string ApiKey = "be12c0b2fb15042bf5ed7616d4bb41a5";
        private const string ApiUrl = "https://api.openweathermap.org/data/2.5/weather?q={0}&appid={1}&units=metric&lang=ru";

        public WeatherController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetWeather(string city)
        {
            if (string.IsNullOrEmpty(city))
            {
                ViewBag.Error = "Введите название города.";
                return View("Index");
            }

            try
            {
                string requestUrl = string.Format(ApiUrl, city, ApiKey);
                HttpResponseMessage response = await _httpClient.GetAsync(requestUrl);

                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.Error = "Город не найден или произошла ошибка запроса.";
                    return View("Index");
                }

                var json = await response.Content.ReadAsStringAsync();
                var weatherResponse = JsonSerializer.Deserialize<JsonElement>(json);

                var weather = new Weather
                {
                    City = city,
                    Temperature = weatherResponse.GetProperty("main").GetProperty("temp").ToString() + "°C",
                    Description = weatherResponse.GetProperty("weather")[0].GetProperty("description").GetString()
                };

                return View("WeatherResult", weather);
            }
            catch (HttpRequestException)
            {
                ViewBag.Error = "Ошибка соединения с сервером. Попробуйте позже.";
            }
            catch (JsonException)
            {
                ViewBag.Error = "Ошибка обработки данных о погоде.";
            }
            catch (Exception)
            {
                ViewBag.Error = "Произошла непредвиденная ошибка.";
            }

            return View("Index");
        }
    }
}
