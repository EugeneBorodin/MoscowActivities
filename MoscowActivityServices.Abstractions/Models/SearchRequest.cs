using System.Text.Json.Serialization;
using NodaTime;

namespace MoscowActivityServices.Abstractions.Models
{
    public class SearchRequest
    {
        [JsonPropertyName("count")] public int Count { get; set; } = 100;

        [JsonPropertyName("from")]
        public LocalDate From { get; set; }

        [JsonPropertyName("till")]
        public LocalDate Till { get; set; }

        [JsonPropertyName("page")] public int Page { get; set; } = 1;
        
        [JsonPropertyName("company_id")]
        public int CompanyId { get; set; }
    }
}