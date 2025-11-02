using OpenWeatherMap.Domain.Entities;

namespace OpenWeatherMap.Application.Contracts
{
    public interface IOpenWeatherMapService
    {
        Task<WeatherResponse> GetWeatherMapData(string city);
    }
}
