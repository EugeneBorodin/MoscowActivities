using MoscowActivityServices.Abstractions.Models;

namespace MoscowActivityServices.Abstractions;

public interface IBookingConfigService
{
    Task<BookingConfig> GetBookingConfig();
    Task UpdateBookingConfig(BookingConfig bookingConfig);
}