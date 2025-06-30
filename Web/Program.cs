using MoscowActivityServices.Implementation;
using Utils.Settings;

var builder = WebApplication.CreateBuilder(args);

var activityServicesSettings = builder.Configuration.Get<ActivityClientSettings>();
builder.Services.AddMoscowActivityServices(activityServicesSettings);
    
var app = builder.Build();

app.MapGet("/", () => "Hello World! This is the MoscowActivities!");

app.Run();