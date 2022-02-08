namespace WeatherApi.Common
{
  public class DateTimeWrapper : IDateTimeWrapper
  {
    public DateTime UtcNow => DateTime.UtcNow;
  }
}
