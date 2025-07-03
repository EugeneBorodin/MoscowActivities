using Web.Handlers;

namespace Web.BackgroundServices;

public class SlotSearchingBackgroundService : BackgroundService
{
    private readonly SlotSearchingHandler _slotSearchingHandler;
    private readonly ILogger<SlotSearchingBackgroundService> _logger;

    private const int Delay = 60_000;

    public SlotSearchingBackgroundService(SlotSearchingHandler slotSearchingHandler, ILogger<SlotSearchingBackgroundService> logger)
    {
        _slotSearchingHandler = slotSearchingHandler;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await _slotSearchingHandler.Handle(stoppingToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
            
            await Task.Delay(Delay, stoppingToken);
        }
    }
}