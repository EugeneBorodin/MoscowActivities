using NodaTime;

namespace MoscowActivityServices.Abstractions.Models;

public class Slot
{
    public string Title { get; set; }
    public string Location { get; set; }
    public string Specialization { get; set; }
    public int Count { get; set; }
    public string StartDateTime { get; set; }
    public Duration Duration { get; set; }
}