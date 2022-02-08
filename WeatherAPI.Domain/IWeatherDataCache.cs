namespace WeatherApi.Domain
{
  public interface IWeatherDataCache
  {
    Task<(bool result, Location location)> TryGetLocationAsync(string id);
    Task UpdateAsync(Location location);
  }
}
