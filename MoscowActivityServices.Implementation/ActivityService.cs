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
            var clients = _activityClientSettings.Value.Clients;

            var tasks = new List<Task<SearchResponse>>();

            foreach (var client in clients)
            {
                var clientCompanySettings = new ActivityClientCompanySettings
                {
                    BookFormId = client.Value.BookFormId,
                    CompanyId = client.Value.CompanyId,
                };
                
                var httpClient = _clientFactory.GetClient(client.Key, clientCompanySettings);
                tasks.Add(httpClient.Search(request));
            }
            
            await Task.WhenAll(tasks);
            
            var slots = new List<Slot>();

            foreach (var task in tasks)
            {
                if (task.IsCompletedSuccessfully)
                {
                    slots.AddRange(ExtractFreeSlots(task.Result));
                }
            }

            return slots;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    public async Task Book(BookingRequest request)
    {
        try
        {
            var clients = _activityClientSettings.Value.Clients;
            var clientData = clients.FirstOrDefault(c => c.Value.BookFormId == request.BookformId);
            
            var clientCompanySettings = new ActivityClientCompanySettings
            {
                BookFormId = clientData.Value.BookFormId,
                CompanyId = clientData.Value.CompanyId,
            };
            
            var httpClient = _clientFactory.GetClient(clientData.Key, clientCompanySettings);

            await httpClient.Book(request);
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
            .Where(IsSlotAvailable)
            .Select(data => new Slot
            {
                Id = data.Id,
                Title = data.Service.Title,
                Location = data.Staff.Name,
                Specialization = data.Staff.Specialization,
                Count = data.Capacity - data.RecordsCount,
                Duration = Duration.FromSeconds(data.DurationDetails.ServicesDuration),
                StartDateTime = data.Date,
                StaffId = data.Staff.Id,
                ServiceId = data.Service.Id,
                BookingLink = $"{searchResponse.BaseUrl}company/{data.Staff.CompanyId}/activity/info/{data.Id}",
                BookingFormId = searchResponse.BookingFormId,
                DateTime = DateTimeOffset.FromUnixTimeSeconds(data.Timestamp)
            });
        
        return slots;
    }

    private bool IsSlotAvailable(ScheduleData data)
    {
        return 
            data.Timestamp > new Instant().ToUnixTimeSeconds() && 
            data.Capacity > data.RecordsCount;
    }
}