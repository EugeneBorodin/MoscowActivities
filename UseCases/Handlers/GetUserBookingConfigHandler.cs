using MediatR;
using Microsoft.Extensions.Logging;
using MoscowActivityServices.Abstractions;
using MoscowActivityServices.Abstractions.Models;

namespace UseCases.Handlers;

public class GetUserBookingConfigRequest: IRequest<UserConfig>
{
    public string Username { get; set; }
}

public class GetUserBookingConfigHandler: IRequestHandler<GetUserBookingConfigRequest, UserConfig>
{
    private readonly IBookingConfigService _bookingConfigService;
    private readonly ILogger<GetUserBookingConfigHandler> _logger;
    
    public GetUserBookingConfigHandler(IBookingConfigService bookingConfigService, ILogger<GetUserBookingConfigHandler> logger)
    {
        _bookingConfigService = bookingConfigService;
        _logger = logger;
    }
    
    public async Task<UserConfig> Handle(GetUserBookingConfigRequest request, CancellationToken cancellationToken)
    {
        var bookingConfig = await _bookingConfigService.GetBookingConfig();
        return bookingConfig.UserConfigs.FirstOrDefault(c => c.Username == request.Username);
    }
}