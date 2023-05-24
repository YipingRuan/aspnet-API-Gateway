using WebCommon.Extentions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.CommonSetup();
var app = builder.Build();

// Configure the HTTP request pipeline.

//app.UseAuthorization();
app.CommonSetup();
app.Run();
