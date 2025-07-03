using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MoscowActivityServices.Abstractions;
using MoscowActivityServices.Abstractions.Models;
using NodaTime;
using Utils.Settings;

namespace MoscowActivityServices.Implementation;

public class ActivityService: IActivityService
{
    private readonly IActivityClientFactory _clientFactory;
    private readonly ILogger<ActivityService> _logger;
    private readonly IOptions<ActivityClientSettings> _activityClientSettings;
    
    public ActivityService(IActivityClientFactory clientFactory, ILogger<ActivityService> logger, IOptions<ActivityClientSettings> activityClientSettings)
    {
        _clientFactory = clientFactory;
        _logger = logger;
        _activityClientSettings = activityClientSettings;
    }
    
    public async Task<IEnumerable<Slot>> FindSlots(SearchRequest request)
    {
        try
        {
            var slots = new List<Slot>();
            var clients = _activityClientSettings.Value.Clients;

            foreach (var client in clients)
            {
                var httpClient = _clientFactory.GetClient(client.Key, client.Value.CompanyId);
                var response = await httpClient.Search(request);
                slots.AddRange(ExtractFreeSlots(response));
            }
            
            return slots;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    private IEnumerable<Slot> ExtractFreeSlots(SearchResponse searchResponse)
    {
        var slots = searchResponse.Data
            .Where(data => data.Capacity > data.RecordsCount)
            .Select(data => new Slot
            {
                Id = data.Id,
                Title = data.Service.Title,
                Location = data.Staff.Name,
                Specialization = data.Staff.Specialization,
                Count = data.Capacity - data.RecordsCount,
                Duration = Duration.FromSeconds(data.DurationDetails.ServicesDuration),
                StartDateTime = data.Date
            });
        
        return slots;
    }
}