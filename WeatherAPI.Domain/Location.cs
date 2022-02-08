namespace WeatherApi.Domain
{
  public class Location
  {
    public Location(string id, IEnumerable<Forecast> forecasts)
    {
      Id = id ?? throw new ArgumentNullException(nameof(id));
      Forecasts = forecasts ?? throw new ArgumentNullException(nameof(forecasts));
    }

    public string Id { get; }
    public IEnumerable<Forecast> Forecasts { get; }
  }
}
