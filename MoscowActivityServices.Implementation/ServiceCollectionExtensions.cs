using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;
using MoscowActivityServices.Abstractions;
using Utils.Settings;

namespace MoscowActivityServices.Implementation;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMoscowActivityServices(this IServiceCollection services, ActivityClientSettings settings)
    {
        foreach (var data in settings.Clients)
        {
            services.AddHttpClient(data.Key, client =>
            {
                client.BaseAddress = new Uri(data.Value.BaseAddress);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue($"Bearer", settings.Token);
            }).AddTypedClient<IActivityClient>();
        }
        services.AddTransient<IActivityClientFactory, ActivityClientFactory>();
        services.AddTransient<IActivityService, ActivityService>();

        services.AddTransient<IBookingConfigService, BookingConfigService>();

        return services;
    }
}