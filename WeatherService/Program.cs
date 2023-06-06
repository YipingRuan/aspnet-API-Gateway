using WebCommon.StartupSetup;
using WebCommon.Logging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.SetupSerilog("WeatherService");
builder.CommonSetup();
var app = builder.Build();

// Configure the HTTP request pipeline.

//app.UseAuthorization();
app.CommonSetup();
app.Run();
