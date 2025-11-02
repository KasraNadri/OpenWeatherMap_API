using Microsoft.AspNetCore.Mvc;
using OpenWeatherMap.Application.Contracts;
using OpenWeatherMap.Domain.Entities;

namespace OpenWeatherMap.APi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OpenWeatherMapController : ControllerBase
    {
        private readonly IOpenWeatherMapService _openWeatherMapService;

        public OpenWeatherMapController(IOpenWeatherMapService openWeatherMapService)
        {
            _openWeatherMapService = openWeatherMapService;
        }

        [HttpGet]
        public async Task<ActionResult<WeatherResponse>> GetWeatherDataAsync([FromQuery] string cityName)
        {

            if (string.IsNullOrWhiteSpace(cityName))
                return BadRequest("City is required");

            var result = await _openWeatherMapService.GetWeatherMapData(cityName);

            return result is null ? NotFound() : Ok(result);
        }
    }
}
