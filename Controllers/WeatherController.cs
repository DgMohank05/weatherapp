using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using WeatherApp.Models;
using System;

namespace WeatherApp.Controllers
{
    public class WeatherController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<WeatherController> _logger;

        public WeatherController(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<WeatherController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetWeather(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
            {
                ViewBag.ErrorMessage = "Please enter a valid city name.";
                return View("Index", null);
            }

            // Retrieve API key from configuration
            string apiKey = _configuration["OpenWeatherApi:ApiKey"];
            var url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={apiKey}&units=metric";

            try
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetStringAsync(url);

                // Log the response for debugging purposes
                _logger.LogInformation("API Response: {Response}", response);

                if (string.IsNullOrEmpty(response))
                {
                    ViewBag.ErrorMessage = "No data returned from the API.";
                    return View("Index", null);
                }

                var weatherData = JsonConvert.DeserializeObject<WeatherApiResponse>(response);

                if (weatherData == null || weatherData.Main == null)
                {
                    ViewBag.ErrorMessage = "Weather data not found.";
                    return View("Index", null);
                }

                var weatherViewModel = new WeatherViewModel
                {
                    City = weatherData.Name,
                    Temperature = weatherData.Main.Temp,
                    Humidity = weatherData.Main.Humidity,
                    WeatherDescription = weatherData.Weather[0].Description
                };

                return View("Index", weatherViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving weather data.");
                ViewBag.ErrorMessage = "Error retrieving weather data: " + ex.Message;
                return View("Index", null);
            }
        }
    }
}
