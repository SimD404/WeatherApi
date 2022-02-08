using Microsoft.Extensions.Configuration;
using System.Runtime.CompilerServices;

namespace WeatherApi.Common
{
  public  class ConfigurationService : IConfigurationService
  {

    private readonly IConfiguration configuration;
    public ConfigurationService(IConfiguration configuration)
    {
      this.configuration = configuration;
    }

    public string WeatherApiKey => GetValue();

    public string WeatherApiBaseUrl => GetValue();

    private string GetValue([CallerMemberName] string caller = "") =>
      configuration[caller] ?? throw new InvalidOperationException($"Configuration property '{caller}' could not be found");
  }
}
