using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MoscowActivityServices.Abstractions;

namespace MoscowActivityServices.Implementation;

public class ActivityClientFactory: IActivityClientFactory
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ActivityClient> _logger;

    public ActivityClientFactory(IServiceProvider serviceProvider)
    {
        _httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
        _logger = serviceProvider.GetRequiredService<ILogger<ActivityClient>>();
    }
    
    public IActivityClient GetClient(string clientName, string companyId)
    {
        var client = _httpClientFactory.CreateClient(clientName);
        return new ActivityClient(client, _logger, companyId);
    }
}