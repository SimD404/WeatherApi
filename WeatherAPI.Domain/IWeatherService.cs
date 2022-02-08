namespace WeatherApi.Domain
{
  public interface IWeatherService
  {
    Task<IDictionary<string, List<Forecast>>> GetLocationsWithTemperatureAboveAsync(IEnumerable<string> locationIds, Unit unit, float minTemperature);

    Task<IEnumerable<Forecast>> GetFiveDayForecast(string id);
  }
}
