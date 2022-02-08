namespace WeatherApi.Common
{
  public interface IConfigurationService 
  {    
    string WeatherApiKey { get; }
    string WeatherApiBaseUrl { get; }
  }
}