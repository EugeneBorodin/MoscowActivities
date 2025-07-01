using EntryPoints.TelegramBot;
using MoscowActivityServices.Implementation;
using Utils.Settings;

var builder = WebApplication.CreateBuilder(args);

var settings = builder.Configuration.GetSection("Settings").Get<Settings>();
builder.Services.AddMoscowActivityServices(settings.ActivityClient);
builder.Services.AddTelegramBot(settings.Bot);
    
var app = builder.Build();

app.MapGet("/", () => "Hello World! This is the MoscowActivities!");

app.Run();