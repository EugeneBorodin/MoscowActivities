using System.Globalization;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using MoscowActivityServices.Abstractions;
using MoscowActivityServices.Abstractions.Models;

namespace MoscowActivityServices.Implementation;

public class ActivityClient: IActivityClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ActivityClient> _logger;
    private readonly string _companyId;

    public ActivityClient(HttpClient httpClient, ILogger<ActivityClient> logger, string companyId)
    {
        _httpClient = httpClient;
        _logger = logger;
        _companyId = companyId;
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
            
            return activities;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "При попытке поиска информацию о мероприятих возникла ошибка");
            throw;
        }
    }
}