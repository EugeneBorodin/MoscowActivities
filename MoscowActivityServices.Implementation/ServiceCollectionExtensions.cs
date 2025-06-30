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
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {settings.Token}");
        });

        return services;
    }
}