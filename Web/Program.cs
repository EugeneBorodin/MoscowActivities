using EntryPoints.TelegramBot;
using MoscowActivityServices.Implementation;
using UseCases;
using Utils.Settings;

var builder = WebApplication.CreateBuilder(args);

var settings = builder.Configuration.GetSection("Settings").Get<Settings>();

builder.Services.Configure<ActivityClientSettings>(builder.Configuration.GetSection("Settings:ActivityClient"));
builder.Services.Configure<BotSettings>(builder.Configuration.GetSection("Settings:Bot"));

builder.Services.AddMoscowActivityServices(settings.ActivityClient);
builder.Services.AddMemoryCache();
builder.Services.AddTelegramBot(settings.Bot);

builder.Services.AddUseCases();
    
var app = builder.Build();

app.MapGet("/", () => "Hello World! This is the MoscowActivities!");

app.Run();