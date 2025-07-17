using System.Globalization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using MoscowActivityServices.Abstractions;
using MoscowActivityServices.Abstractions.Models;

namespace MoscowActivityServices.Implementation;

public class ActivityClient: IActivityClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ActivityClient> _logger;
    private readonly string _companyId;
    private readonly int _bookFormId;

    public ActivityClient(HttpClient httpClient, ILogger<ActivityClient> logger, ActivityClientCompanySettings companySettings)
    {
        _httpClient = httpClient;
        _logger = logger;
        _companyId = companySettings.CompanyId;
        _bookFormId = companySettings.BookFormId;
    }

    public async Task<SearchResponse> Search(SearchRequest request)
    {
        try
        {
            string from = request.From.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            string till = request.Till.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            
            var response = await _httpClient.GetAsync(
                $"api/v1/activity/{_companyId}/search?count={request.Count}&from={from}&till={till}&page={request.Page}");
            response.EnsureSuccessStatusCode();

            var activities = await response.Content.ReadFromJsonAsync<SearchResponse>();
            activities.BaseUrl = _httpClient.BaseAddress.ToString();
            activities.BookingFormId = _bookFormId;
            
            return activities;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "При попытке поиска информацию о мероприятих возникла ошибка");
            throw;
        }
    }

    public async Task Book(BookingRequest request)
    {
        var activityId = request.Appointments.First().ActivityId;
        
        try
        {
            request.BookformId = _bookFormId;
            request.RedirectUrl = _httpClient.BaseAddress + $"api/v1/activity/{_companyId}/success-order/{{recordId}}/{{recordHash}}";
            
            var httpRequest =
                new HttpRequestMessage(HttpMethod.Post, $"api/v1/activity/{_companyId}/{activityId}/book");
            
            var requestContent = JsonSerializer.Serialize(request);
            httpRequest.Content = new StringContent(requestContent, new MediaTypeHeaderValue("application/json"));
            
            _logger.LogDebug("Request:\n" + requestContent);

            var response = await _httpClient.SendAsync(httpRequest);

            var responseBody = await response.Content.ReadAsStringAsync();
            _logger.LogDebug("Response:\n" + responseBody);
            
            response.EnsureSuccessStatusCode();
            
            _logger.LogInformation("Запись на слот {activityId} успешно создана", activityId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Попытка записаться на слот {activityId} завершилась ошибкой", activityId);
            throw;
        }
    }
}