using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using OpenWeatherMap.Infrastructure.Services;
using System.Net;
using System.Text;

namespace OpenWeatherMap.UnitTests
{
    public class OpenWeatherMapServiceTests
    {
        [Fact]
        public async Task GetWeatherMapData_ReturnsWeatherResponse_WhenDataIsValid()
        {
            var city = "London";

            //===== MOCK DATA FOR THE WEATHER RESPONSE =====\\
            var weatherApiResponseJson = @"
        {
            ""coord"": { ""lat"": 51.5074, ""lon"": -0.1278 },
            ""main"": { ""temp"": 15.0, ""humidity"": 80 },
            ""wind"": { ""speed"": 5.5 },
            ""name"": ""London""
        }";

            //===== MOCK DATA FOR THE AIR POLLUTION RESPONSE =====\\
            var airApiResponseJson = @"
            {
            ""list"": [{
                ""main"": { ""aqi"": 3 },
                ""components"": {
                    ""co"": 0.3,
                    ""no"": 0.0,
                    ""no2"": 0.1,
                    ""o3"": 0.2,
                    ""so2"": 0.05,
                    ""pm2_5"": 10.0,
                    ""pm10"": 20.0,
                    ""nh3"": 0.1
                }
            }]
            }";

            
            var handlerMock = new Mock<HttpMessageHandler>();

            //========== MOCKING HTTPCLIENT TO SIMULATE TWO SUCCESSFUL OPENWEATHERMAP API RESPONSES (WEATHER AND AIR POLLUTION) =====\\
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.ToString().Contains("weather?q=London")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(weatherApiResponseJson, Encoding.UTF8, "application/json")
                });

            // Mock the air pollution API call
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.ToString().Contains("air_pollution?lat=51.5074&lon=-0.1278")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(airApiResponseJson, Encoding.UTF8, "application/json")
                });

            var httpClient = new HttpClient(handlerMock.Object);
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(c => c["OpenWeatherMapApiKey"]).Returns("test-api-key");

            var openWeatherMapService = new OpenWeatherMapService(httpClient, configurationMock.Object);

            // Act
            var result = await openWeatherMapService.GetWeatherMapData(city);

            // Assert
            Assert.Equal("London", result.CityName);
            Assert.Equal(15.0, result.Temperature);
            Assert.Equal(80, result.HumidityPercentage);
            Assert.Equal(5.5, result.WindSpeed);
            Assert.Equal(3, result.AirQualityIndex);
            Assert.Equal(51.5074, result.Latitude);
            Assert.Equal(-0.1278, result.Longitude);
            Assert.Equal(0.3, result.Pollutants["co"]);
            Assert.Equal(0.0, result.Pollutants["no"]);
            Assert.Equal(0.1, result.Pollutants["no2"]);
            Assert.Equal(0.2, result.Pollutants["o3"]);
            Assert.Equal(0.05, result.Pollutants["so2"]);
            Assert.Equal(10.0, result.Pollutants["pm2_5"]);
            Assert.Equal(20.0, result.Pollutants["pm10"]);
            Assert.Equal(0.1, result.Pollutants["nh3"]);
        }

        [Fact]
        public async Task GetWeatherMapData_ThrowsException_WhenWeatherApiFails()
        {
            var city = "London";

            //========== MOCKING HTTPCLIENT TO SIMULATE A FAILED OPENWEATHERMAP API RESPONSE =====\\
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.ToString().Contains("weather?q=London")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError
                });

            var httpClient = new HttpClient(handlerMock.Object);
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(c => c["OpenWeatherMapApiKey"]).Returns("test-api-key");

            var openWeatherMapService = new OpenWeatherMapService(httpClient, configurationMock.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<HttpRequestException>(() => openWeatherMapService.GetWeatherMapData(city));
            Assert.Equal("Response status code does not indicate success: 500 (Internal Server Error).", exception.Message);
        }
    }

}