using MediatR;
using Microsoft.Extensions.Logging;
using MoscowActivityServices.Abstractions;
using MoscowActivityServices.Abstractions.Models;

namespace UseCases.Handlers;

public class UpdateUserBookingConfigRequest: IRequest
{
    public UserConfig UserConfig { get; set; }
}

public class UpdateUserBookingConfigHandler: IRequestHandler<UpdateUserBookingConfigRequest>
{
    private readonly IBookingConfigService _bookingConfigService;
    private readonly ILogger<UpdateUserBookingConfigHandler> _logger;
    
    public UpdateUserBookingConfigHandler(IBookingConfigService bookingConfigService, ILogger<UpdateUserBookingConfigHandler> logger)
    {
        _bookingConfigService = bookingConfigService;
        _logger = logger;
    }
    
    public async Task Handle(UpdateUserBookingConfigRequest request, CancellationToken cancellationToken)
    {
        var bookingConfig = await _bookingConfigService.GetBookingConfig();
        var userConfig = bookingConfig.UserConfigs.FirstOrDefault(c => c.Username == request.UserConfig.Username);

        if (userConfig != null)
        {
            userConfig.Username = request.UserConfig.Username;
            userConfig.ChatId = request.UserConfig.ChatId;
            userConfig.Fullname = request.UserConfig.Fullname;
            userConfig.PhoneNumber = request.UserConfig.PhoneNumber;
            userConfig.Email = request.UserConfig.Email;
            userConfig.SlotParams = request.UserConfig.SlotParams;
        }
        else
        {
            bookingConfig.UserConfigs.Add(request.UserConfig);
        }
        
        await _bookingConfigService.UpdateBookingConfig(bookingConfig);

        _logger.LogInformation("Конфигурация для пользователя {username} сохранена", request.UserConfig.Username);
        
    }
}