using Newtonsoft.Json;

namespace WeatherApi.Infrastructure.OpenWeatherMap
{
  internal class TemperatureData
  {
    [JsonProperty("temp")]
    public float Temperature { get; set; }
  }
}
