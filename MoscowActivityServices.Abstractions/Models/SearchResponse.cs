using System.Text.Json.Serialization;

namespace MoscowActivityServices.Abstractions.Models
{
    public class SearchResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("data")]
        public List<ScheduleData> Data { get; set; }

        [JsonPropertyName("meta")]
        public Meta Meta { get; set; }
    }

    public class ScheduleData
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("service_id")]
        public int ServiceId { get; set; }

        [JsonPropertyName("company_id")]
        public int CompanyId { get; set; }

        [JsonPropertyName("staff_id")]
        public int StaffId { get; set; }

        [JsonPropertyName("date")]
        public string Date { get; set; }

        [JsonPropertyName("timestamp")]
        public long Timestamp { get; set; }

        [JsonPropertyName("length")]
        public int Length { get; set; }

        [JsonPropertyName("capacity")]
        public int Capacity { get; set; }

        [JsonPropertyName("color")]
        public string Color { get; set; }

        [JsonPropertyName("instructions")]
        public string Instructions { get; set; }

        [JsonPropertyName("stream_link")]
        public string StreamLink { get; set; }

        [JsonPropertyName("font_color")]
        public string FontColor { get; set; }

        [JsonPropertyName("notified")]
        public bool Notified { get; set; }

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

        [JsonPropertyName("labels")]
        public List<string> Labels { get; set; }

        [JsonPropertyName("resource_instances")]
        public List<string> ResourceInstances { get; set; }

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

        [JsonPropertyName("api_id")]
        public string ApiId { get; set; }

        [JsonPropertyName("user_id")]
        public int? UserId { get; set; }

        [JsonPropertyName("rating")]
        public int Rating { get; set; }

        [JsonPropertyName("prepaid")]
        public string Prepaid { get; set; }

        [JsonPropertyName("show_rating")]
        public int ShowRating { get; set; }

        [JsonPropertyName("comments_count")]
        public int CommentsCount { get; set; }

        [JsonPropertyName("votes_count")]
        public int VotesCount { get; set; }

        [JsonPropertyName("average_score")]
        public double AverageScore { get; set; }

        [JsonPropertyName("avatar")]
        public string Avatar { get; set; }

        [JsonPropertyName("avatar_big")]
        public string AvatarBig { get; set; }

        [JsonPropertyName("position")]
        public List<string> Position { get; set; }
    }

    public class Service
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("image_url")]
        public string ImageUrl { get; set; }

        [JsonPropertyName("category_id")]
        public int CategoryId { get; set; }

        [JsonPropertyName("is_category")]
        public bool IsCategory { get; set; }

        [JsonPropertyName("salon_service_id")]
        public int SalonServiceId { get; set; }

        [JsonPropertyName("comment")]
        public string Comment { get; set; }

        [JsonPropertyName("price_min")]
        public decimal PriceMin { get; set; }

        [JsonPropertyName("price_max")]
        public decimal PriceMax { get; set; }

        [JsonPropertyName("prepaid")]
        public string Prepaid { get; set; }

        [JsonPropertyName("abonement_restriction")]
        public int AbonementRestriction { get; set; }

        [JsonPropertyName("category")]
        public ServiceCategory Category { get; set; }
    }

    public class ServiceCategory
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("category_id")]
        public int CategoryId { get; set; }

        [JsonPropertyName("is_category")]
        public bool IsCategory { get; set; }

        [JsonPropertyName("salon_service_id")]
        public int SalonServiceId { get; set; }

        [JsonPropertyName("prepaid")]
        public string Prepaid { get; set; }

        [JsonPropertyName("abonement_restriction")]
        public int AbonementRestriction { get; set; }

        // [JsonPropertyName("category")]
        // public ServiceCategory Category { get; set; }
    }

    public class DurationDetails
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("services_duration")]
        public int ServicesDuration { get; set; }

        [JsonPropertyName("technical_break_duration")]
        public int TechnicalBreakDuration { get; set; }
    }

    public class Meta
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }
    }
}