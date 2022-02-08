using System.Collections.Concurrent;
using WeatherApi.Domain;

namespace WeatherApi.Infrastructure.Caching
{
  internal class InMemoryWeatherDataCache : IWeatherDataCache
  {
    private readonly IDictionary<string, Location> cache = new ConcurrentDictionary<string, Location>();
    public Task<(bool result, Location location)> TryGetLocationAsync(string id)
    {
      var result = cache.TryGetValue(id, out var location);
      return Task.FromResult((result, location));
    }

    public Task UpdateAsync(Location location)
    {
      cache.Add(location.Id, location);
      return Task.CompletedTask;
    }
  }
}
