using NodaTime;

namespace Utils;

public static class DateTimeHelper
{
    public static LocalDateTime ConvertToLocalDateTime(long timestamp)
    {
        var instant = Instant.FromUnixTimeSeconds(timestamp);
        
        var moscowTz = DateTimeZoneProviders.Tzdb.GetZoneOrNull("Europe/Moscow");
        
        var localDateTime = instant.InZone(zone: moscowTz).LocalDateTime;

        return localDateTime;
    }
    
    public static DateTime ConvertToDateTime(long timestamp)
    {
        var instant = Instant.FromUnixTimeSeconds(timestamp);
        
        var moscowTz = DateTimeZoneProviders.Tzdb.GetZoneOrNull("Europe/Moscow");
        
        var dateTime = instant.InZone(zone: moscowTz).ToDateTimeUnspecified();

        return dateTime;
    }
}