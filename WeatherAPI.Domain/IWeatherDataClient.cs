namespace WeatherApi.Domain
{
  public interface IWeatherDataClient
  {
    Task<Location> GetFiveDayForecastAsync(string Id);
  }
}
