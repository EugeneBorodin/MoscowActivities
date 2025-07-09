using Microsoft.Extensions.DependencyInjection;
using UseCases.BackgroundServices;

namespace UseCases;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUseCases(this IServiceCollection services)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(assemblies));
        services.AddHostedService<SlotSearchingBackgroundService>();
        
        return services;
    }
}