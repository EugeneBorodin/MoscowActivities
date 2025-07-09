using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NodaTime;
using UseCases.Handlers;

namespace UseCases.BackgroundServices;

public class SlotSearchingBackgroundService : BackgroundService
{
    private readonly IMediator _mediator;
    private readonly ILogger<SlotSearchingBackgroundService> _logger;

    private const int Delay = 60_000;

    public SlotSearchingBackgroundService(IMediator mediator, ILogger<SlotSearchingBackgroundService> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var today = LocalDate.FromDateTime(DateTime.Now);
                await _mediator.Send(new SlotSearchingRequest
                {
                    From = today,
                    Till = LocalDate.Add(today, Period.FromDays(5)),
                }, stoppingToken);
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