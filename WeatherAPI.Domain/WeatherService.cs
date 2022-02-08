using WeatherApi.Common;

namespace WeatherApi.Domain
{
  internal class WeatherService : IWeatherService
  {
    private readonly IWeatherDataCache weatherDataCache;
    private readonly IWeatherDataClient weatherDataClient;
    private readonly IDateTimeWrapper dateTimeWrapper;

    public WeatherService(
      IWeatherDataCache weatherDataCache,
      IWeatherDataClient weatherDataClient,
      IDateTimeWrapper dateTimeWrapper)
    {
      this.weatherDataCache = weatherDataCache;
      this.weatherDataClient = weatherDataClient;
      this.dateTimeWrapper = dateTimeWrapper;
    }

    public async Task<IDictionary<string, List<Forecast>>> GetLocationsWithTemperatureAboveAsync(IEnumerable<string> locationIds, Unit unit, float minTemperature)
    {
      var tomorrow = DateOnly.FromDateTime(dateTimeWrapper.UtcNow + TimeSpan.FromDays(1));

      var results = new Dictionary<string, List<Forecast>>();
      foreach (var id in locationIds)
      {
        var location = await FindLocationAsync(id);
        var tomorrowsForecasts = location.Forecasts.Where(x => x.GetDate() == tomorrow);
        
        var forecastsAboveMinimumTemperature = tomorrowsForecasts.Where(x => x.GetTemperatureInUnit(unit) > minTemperature);

        if (forecastsAboveMinimumTemperature.Any())
        {
          var resultsForLocation = forecastsAboveMinimumTemperature.Select(x => x.ConvertToUnit(unit)).ToList();
          results.Add(id, resultsForLocation);
        }
      }

      return results;
    }

    public async Task<IEnumerable<Forecast>> GetFiveDayForecast(string id)
    {
      var location = await FindLocationAsync(id);

      return location.Forecasts;
    }

    private async Task<Location> FindLocationAsync(string locationId)
    {
      var (foundInCache, location) = await weatherDataCache.TryGetLocationAsync(locationId);
      if (foundInCache && LocationHasFutureDates(location))
      {
        return location;
      }
      else
      {
        var result = await weatherDataClient.GetFiveDayForecastAsync(locationId);

        var locationWithUpcomingForecasts = FindForecastsForUpcomingDays(result);
        await weatherDataCache.UpdateAsync(locationWithUpcomingForecasts);

        return result;
      }
    }

    private bool LocationHasFutureDates(Location location)
    {
      var today = DateOnly.FromDateTime(dateTimeWrapper.UtcNow);
      return location.Forecasts.All(x => x.GetDate() > today);
    }

    private Location FindForecastsForUpcomingDays(Location location)
    {
      var today = DateOnly.FromDateTime(dateTimeWrapper.UtcNow);
      var forecastForComingDays = location.Forecasts.Where(x => x.GetDate() > today);

      return new Location(location.Id, forecastForComingDays);
    }
  }
}