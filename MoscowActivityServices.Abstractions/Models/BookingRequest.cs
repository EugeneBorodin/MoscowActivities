using System.Text.Json.Serialization;

namespace MoscowActivityServices.Abstractions.Models;

public class BookingRequest
{
    [JsonPropertyName("appointments")]
    public List<Appointment> Appointments { get; set; }

    [JsonPropertyName("bookform_id")]
    public int BookformId { get; set; }

    [JsonPropertyName("fullname")]
    public string Fullname { get; set; }

    [JsonPropertyName("comment")]
    public string Comment { get; set; }

    [JsonPropertyName("phone")]
    public string Phone { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; }

    [JsonPropertyName("isMobile")]
    public bool IsMobile { get; set; }

    [JsonPropertyName("notify_by_sms")]
    public int NotifyBySms { get; set; } = 1;

    [JsonPropertyName("is_support_charge")]
    public bool IsSupportCharge { get; set; }

    [JsonPropertyName("is_charge_required_priority")]
    public bool IsChargeRequiredPriority { get; set; } = true;

    [JsonPropertyName("redirect_url")]
    public string RedirectUrl { get; set; }
}

public class Appointment
{
    [JsonPropertyName("activityId")]
    public int ActivityId { get; set; }

    [JsonPropertyName("activityType")]
    public int ActivityType { get; set; } = 2;

    [JsonPropertyName("clients_count")]
    public int ClientsCount { get; set; }

    [JsonPropertyName("datetime")]
    public DateTime Datetime { get; set; }

    [JsonPropertyName("staff_id")]
    public int StaffId { get; set; }

    [JsonPropertyName("services")]
    public List<int> Services { get; set; }

    [JsonPropertyName("is_trial_service")]
    public bool IsTrialService { get; set; }
}