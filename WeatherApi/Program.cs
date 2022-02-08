using System.Text.Json.Serialization;
using WeatherApi.Common;
using WeatherApi.Domain;
using WeatherApi.Infrastructure;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(configure =>
{
  var enumConverter = new JsonStringEnumConverter();
  configure.JsonSerializerOptions.Converters.Add(enumConverter);
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var configurationService = new ConfigurationService(builder.Configuration);

builder.Services.AddSingleton<IConfigurationService>(configurationService);

builder.Services.AddDomainLayer();
builder.Services.AddInfrastructureLayer(x =>
{
  x.WeatherApiBaseUrl = configurationService.WeatherApiBaseUrl;
  x.UseMongoDbCache();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
