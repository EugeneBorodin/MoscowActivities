using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MoscowActivityServices.Abstractions;
using NodaTime;
using UseCases.Handlers;

namespace UseCases.BackgroundServices;

public class SlotBookingBackgroundService: BackgroundService
{
    private readonly IMediator _mediator;
    private readonly IBookingConfigService _bookingConfigService;
    private readonly ILogger<SlotBookingBackgroundService> _logger;
    
    private const int Delay = 10_000;

    public SlotBookingBackgroundService(IMediator mediator, IBookingConfigService bookingConfigService, ILogger<SlotBookingBackgroundService> logger)
    {
        _mediator = mediator;
        _bookingConfigService = bookingConfigService;
        _logger = logger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var today = LocalDate.FromDateTime(DateTime.Now);

                var tasks = new List<Task>();
                
                var bookingConfig = await _bookingConfigService.GetBookingConfig();

                foreach (var userConfig in bookingConfig.UserConfigs)
                {
                    var slotBookingRequest = new SlotBookingRequest
                    {
                        From = today,
                        Till = LocalDate.Add(today, Period.FromDays(5)),
                        UserConfig = userConfig,
                    };
                    
                    tasks.Add(_mediator.Send(slotBookingRequest, stoppingToken));
                }

                await Task.WhenAll(tasks);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
            
            await Task.Delay(Delay, stoppingToken);
        }
    }
}