namespace MoscowActivityServices.Abstractions.Models;

public class BookingConfig
{
    public List<UserConfig> UserConfigs { get; set; } = new ();
}

public class UserConfig
{
    public string Username { get; set; }
    public string Fullname { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public List<SlotTimeToBook> SlotTimeToBookItems { get; set; } = new ();
}

public class SlotTimeToBook
{
    public DayOfWeek DayOfWeek { get; set; }
    public TimeOnly Time { get; set; }
}