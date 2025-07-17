using NodaTime;

namespace MoscowActivityServices.Abstractions.Models;

public class Slot
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Location { get; set; }
    public string Specialization { get; set; }
    public int Count { get; set; }
    public string StartDateTime { get; set; }
    public Duration Duration { get; set; }
    public string BookingLink { get; set; }
    public int StaffId { get; set; }
    public int ServiceId { get; set; }
    public int BookingFormId { get; set; }
    public DateTime DateTime { get; set; }
}