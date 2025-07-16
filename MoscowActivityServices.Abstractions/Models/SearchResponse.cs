using System.Text.Json.Serialization;

namespace MoscowActivityServices.Abstractions.Models
{
    public class SearchResponse
    {
        [JsonPropertyName("data")]
        public List<ScheduleData> Data { get; set; }
        public int BookingFormId { get; set; }
        public string BaseUrl { get; set; }
    }

    public class ScheduleData
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("date")]
        public string Date { get; set; }

        [JsonPropertyName("timestamp")]
        public long Timestamp { get; set; }
        
        [JsonPropertyName("capacity")]
        public int Capacity { get; set; }
        
        [JsonPropertyName("comment")]
        public string Comment { get; set; }

        [JsonPropertyName("records_count")]
        public int RecordsCount { get; set; }

        [JsonPropertyName("prepaid")]
        public string Prepaid { get; set; }

        [JsonPropertyName("staff")]
        public Staff Staff { get; set; }

        [JsonPropertyName("service")]
        public Service Service { get; set; }

        [JsonPropertyName("duration_details")]
        public DurationDetails DurationDetails { get; set; }
    }

    public class Staff
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("company_id")]
        public int CompanyId { get; set; }

        [JsonPropertyName("specialization")]
        public string Specialization { get; set; }
    }

    public class Service
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("title")]
        public string Title { get; set; }
    }

    public class DurationDetails
    {
        [JsonPropertyName("services_duration")]
        public int ServicesDuration { get; set; }
    }
}