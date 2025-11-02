namespace OpenWeatherMap.Domain.Entities
{
    public class WeatherResponse
    {
        public string CityName { get; set; } = string.Empty;
        public double Temperature { get; set; } //IN CELSIUS
        public int HumidityPercentage { get; set; }
        public double WindSpeed { get; set; } //IN M/S
        public int AirQualityIndex { get; set; } 
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public Dictionary<string, double> Pollutants { get; set; } = new Dictionary<string, double>();
    }
}
