using System.Diagnostics;

namespace WeatherApi.Domain
{
  public class Forecast
  {
    public Forecast(DateTime timestamp, float temperature, Unit unit)
    {
      Timestamp = timestamp;
      Temperature = temperature;
      Unit = unit;
    }

    public DateOnly GetDate() => DateOnly.FromDateTime(Timestamp);


    public Forecast ConvertToUnit(Unit unit)
    {
      return Unit == unit ? this : new Forecast(Timestamp, GetTemperatureInUnit(unit), unit);
    }

    public float GetTemperatureInUnit(Unit unit)
    {
      if (unit == Unit)
      {
        return Temperature;
      }

      return unit switch
      {
        Unit.Celsius => (Temperature - 32) * 5 / 9,
        Unit.Fahrenheit => (Temperature * 9 / 5) + 32,
        _ => throw new NotImplementedException($"Conversion for {unit} has not been implemented")
      };
    }

    public DateTime Timestamp { get; }
    public float Temperature { get; }
    public Unit Unit { get; }
  }
}
