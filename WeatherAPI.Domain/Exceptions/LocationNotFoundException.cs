namespace WeatherApi.Domain.Exceptions
{
  public class LocationNotFoundException : Exception
  {
    public LocationNotFoundException(string locationId) : base($"location with id '{locationId}' could not be found")
    {
    }
  }
}
