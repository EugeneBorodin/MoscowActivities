using Microsoft.Extensions.Logging;
using MoscowActivityServices.Abstractions;
using MoscowActivityServices.Abstractions.Models;
using NodaTime;

namespace MoscowActivityServices.Implementation;

public class ActivityService: IActivityService
{
    private readonly IActivityClient _client;
    private readonly ILogger<ActivityService> _logger;
    
    public ActivityService(IActivityClient client, ILogger<ActivityService> logger)
    {
        _client = client;
        _logger = logger;
    }
    
    public async Task<IEnumerable<Slot>> FindSlots(SearchRequest request)
    {
        try
        {
            var response = await _client.Search(request);
            var slots = ExtractFreeSlots(response);
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
                Title = data.Service.Title,
                Location = data.Staff.Name,
                Specialization = data.Staff.Specialization,
                Count = data.Capacity - data.RecordsCount,
                Duration = Duration.FromMinutes(data.DurationDetails.ServicesDuration),
                StartDateTime = data.Date
            });
        
        return slots;
    }
}