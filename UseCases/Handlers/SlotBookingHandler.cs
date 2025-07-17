using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using MoscowActivityServices.Abstractions;
using MoscowActivityServices.Abstractions.Models;
using NodaTime;
using Telegram.Bot;

namespace UseCases.Handlers;

public class SlotBookingRequest : IRequest
{
    public UserConfig UserConfig { get; set; }
    public LocalDate From { get; set; }
    public LocalDate Till { get; set; }
}

public class SlotBookingHandler : IRequestHandler<SlotBookingRequest>
{
    private IActivityService _activityService;
    private readonly IMemoryCache _cache;
    private readonly ILogger<SlotBookingHandler> _logger;
    private readonly ITelegramBotClient _botClient;
    private const string CACHE_PREFIX = "slot_booking_";

    public SlotBookingHandler(IActivityService activityService, IMemoryCache cache, ILogger<SlotBookingHandler> logger, ITelegramBotClient botClient)
    {
        _activityService = activityService;
        _cache = cache;
        _logger = logger;
        _botClient = botClient;
    }
    
    public async Task Handle(SlotBookingRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var slots = await _activityService.FindSlots(new SearchRequest
            {
                From = request.From,
                Till = request.Till,
            });
            
            var bookingRequests = GenerateBookingRequests(slots, request.UserConfig);

            var tasks = new List<Task>();
            
            foreach (var bookingRequest in bookingRequests)
            {
                var activityId = bookingRequest.Appointments.First().ActivityId;
                if (_cache.TryGetValue(CACHE_PREFIX + activityId, out _))
                {
                    continue;
                }
                
                tasks.Add(_activityService.Book(bookingRequest));
                _cache.Set(CACHE_PREFIX + activityId, bookingRequest);
            }

            await Task.WhenAll(tasks);

            if (tasks.Any())
            {
                var message = "Сработала автоматическая запись на слоты:\n" +
                              string.Join('\n', bookingRequests.Select(r => r.Appointments.First().Datetime));
            
                await _botClient.SendMessage(request.UserConfig.ChatId, message, cancellationToken: cancellationToken);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Попытка записаться в найденные слоты закончилась ошибкой");
            var exceptionText = JsonSerializer.Serialize(e);
            
            await _botClient.SendMessage(request.UserConfig.ChatId, "Попытка записаться в найденные слоты закончилась ошибкой: " + exceptionText, cancellationToken: cancellationToken);
        }
    }

    private IEnumerable<BookingRequest> GenerateBookingRequests(IEnumerable<Slot> slots, UserConfig userConfig)
    {
        var requests = userConfig.SlotParams
            .SelectMany(slotParam => slots
                .Where(s => slotParam.DayOfWeek == s.DateTime.DayOfWeek
                            && slotParam.Time == TimeOnly.FromDateTime(s.DateTime)
                            && slotParam.PeopleCount <= s.Count)
                // .DistinctBy(s => s.DateTime) // раскомментировать, если запись будет происходить на все корты сразу
                .Select(s => new BookingRequest
                {
                    Appointments = new()
                    {
                        new()
                        {
                            ActivityId = s.Id,
                            ActivityType = 2, // default type for this activity
                            Datetime = s.DateTime,
                            StaffId = s.StaffId,
                            Services = [s.ServiceId],
                            ClientsCount = slotParam.PeopleCount,
                        }
                    },
                    BookformId = s.BookingFormId,
                    Email = userConfig.Email,
                    Fullname = userConfig.Fullname,
                    Phone = userConfig.PhoneNumber,
                    IsChargeRequiredPriority = true,
                    NotifyBySms = 1, // default for this type of booking
                }));

        return requests;
    }
}