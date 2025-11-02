using Microsoft.Extensions.Configuration;
using OpenWeatherMap.Application.Contracts;
using OpenWeatherMap.Domain.Entities;
using System.Text.Json;

namespace OpenWeatherMap.Infrastructure.Services
{
    public class OpenWeatherMapService : IOpenWeatherMapService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public OpenWeatherMapService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["OpenWeatherMapApiKey"] ?? throw new ArgumentNullException("OpenWeatherMapApiKey hasn't been configured!");  
        }

        public async Task<WeatherResponse> GetWeatherMapData(string city)
        {
            #region CURRENT WEATHER (INCLUDES COORIDNATES)

            var weatherUrl = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={_apiKey}&units=metric";
            var weatherResponse = await _httpClient.GetStringAsync(weatherUrl);
            if (weatherResponse == null)
            {
                throw new Exception("Failed to fetch weather data.");
            }
            using var weatherDoc = JsonDocument.Parse(weatherResponse);

            //===== COORIDNATES =====\\
            var coordinates = weatherDoc.RootElement.GetProperty("coord");
            double latitude = coordinates.GetProperty("lat").GetDouble();
            double longitude = coordinates.GetProperty("lon").GetDouble();

            var main = weatherDoc.RootElement.GetProperty("main"); //HUMIDITY AND TEMPERATURE

            var wind = weatherDoc.RootElement.GetProperty("wind");

            #endregion

            #region AIR POLLUTION

            var airUrl = $"https://api.openweathermap.org/data/2.5/air_pollution?lat={latitude}&lon={longitude}&appid={_apiKey}";
            var airResponse = await _httpClient.GetStringAsync(airUrl);
            if (airResponse == null)
            {
                throw new Exception("Failed to fetch weather data.");
            }
            using var airDoc = JsonDocument.Parse(airResponse);

            var airData = airDoc.RootElement.GetProperty("list")[0];

            int aqi = airData.GetProperty("main").GetProperty("aqi").GetInt32(); //CONTAINS AQI
            var comps = airData.GetProperty("components"); //CONTAINS POLLUTANTS

            #endregion

            var weatherData = new WeatherResponse
            {
                CityName = city,
                Latitude = latitude,
                Longitude = longitude,
                Temperature = main.GetProperty("temp").GetDouble(),
                HumidityPercentage = main.GetProperty("humidity").GetInt32(),
                WindSpeed = wind.GetProperty("speed").GetDouble(),
                AirQualityIndex = aqi,
                Pollutants = new Dictionary<string, double>
                {
                    ["co"] = comps.GetProperty("co").GetDouble(),
                    ["no"] = comps.GetProperty("no").GetDouble(),
                    ["no2"] = comps.GetProperty("no2").GetDouble(),
                    ["o3"] = comps.GetProperty("o3").GetDouble(),
                    ["so2"] = comps.GetProperty("so2").GetDouble(),
                    ["pm2_5"] = comps.GetProperty("pm2_5").GetDouble(),
                    ["pm10"] = comps.GetProperty("pm10").GetDouble(),
                    ["nh3"] = comps.GetProperty("nh3").GetDouble(),
                }
            };

            return weatherData;
        }
    }
}
