using Newtonsoft.Json;

namespace WeatherApi.Infrastructure.OpenWeatherMap
{
  internal class DataPoint
  {
    [JsonProperty("main")]
    public TemperatureData TemperatureData { get; set; }

    [JsonProperty("dt_txt")]
    public DateTime Timestamp { get; set; }
  }
}
