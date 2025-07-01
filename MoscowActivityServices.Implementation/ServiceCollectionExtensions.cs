using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;
using MoscowActivityServices.Abstractions;
using Utils.Settings;

namespace MoscowActivityServices.Implementation;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMoscowActivityServices(this IServiceCollection services, ActivityClientSettings settings)
    {
        services.AddHttpClient<IActivityClient, ActivityClient>(client =>
        {
            client.BaseAddress = new Uri(settings.BaseAddress);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue($"Bearer", settings.Token);
        });
        
        services.AddTransient<IActivityService, ActivityService>();

        return services;
    }
}