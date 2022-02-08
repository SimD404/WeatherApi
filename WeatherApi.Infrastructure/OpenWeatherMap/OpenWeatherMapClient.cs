using Newtonsoft.Json;
using WeatherApi.Common;
using WeatherApi.Domain;
using WeatherApi.Domain.Exceptions;

namespace WeatherApi.Infrastructure.OpenWeatherMap
{
  internal class OpenWeatherMapClient : IWeatherDataClient
  {
    private readonly HttpClient httpClient;
    private readonly IConfigurationService configurationService;

    public OpenWeatherMapClient(HttpClient httpClient, IConfigurationService configurationService)
    {
      this.httpClient = httpClient;
      this.configurationService = configurationService;
    }

    public async Task<Location> GetFiveDayForecastAsync(string id)
    {
      var result = await httpClient.GetAsync($"forecast?q={id}&units=metric&appid={configurationService.WeatherApiKey}");

      if(!result.IsSuccessStatusCode)
      {
        throw new LocationNotFoundException(id);
      }

      var content = await result.Content.ReadAsStringAsync();
      var apiReponse = JsonConvert.DeserializeObject<OpenWeatherMapResponse>(content);

      if(apiReponse == null)
      {
        throw new LocationNotFoundException(id);
      }

      var forecasts = apiReponse.DataPoints.Select(x => new Forecast(DateTime.SpecifyKind(x.Timestamp, DateTimeKind.Utc), x.TemperatureData.Temperature, Unit.Celsius));

      return new Location(id, forecasts);
    }
  }
}
