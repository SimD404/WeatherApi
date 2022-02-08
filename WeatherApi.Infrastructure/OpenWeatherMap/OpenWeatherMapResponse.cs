using Newtonsoft.Json;

namespace WeatherApi.Infrastructure.OpenWeatherMap
{
  internal class OpenWeatherMapResponse
  {
    [JsonProperty("list")]
    public IEnumerable<DataPoint> DataPoints { get; set; }
  }
}
