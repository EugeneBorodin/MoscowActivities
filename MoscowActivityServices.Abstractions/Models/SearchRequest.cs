using System.Text.Json.Serialization;
using NodaTime;

namespace MoscowActivityServices.Abstractions.Models
{
    public class SearchRequest
    {
        [JsonPropertyName("count")] public int Count { get; set; } = 1000;

        [JsonPropertyName("from")]
        public LocalDate From { get; set; }

        [JsonPropertyName("till")]
        public LocalDate Till { get; set; }

        [JsonPropertyName("page")] public int Page { get; set; } = 1;   
    }
}