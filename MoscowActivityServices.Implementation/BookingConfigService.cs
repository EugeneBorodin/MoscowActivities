using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using MoscowActivityServices.Abstractions;
using MoscowActivityServices.Abstractions.Models;

namespace MoscowActivityServices.Implementation;

public class BookingConfigService : IBookingConfigService
{
    private const string BookingConfigFileName = "booking-config.json";
    
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<BookingConfigService> _logger;
    
    public BookingConfigService(IMemoryCache memoryCache, ILogger<BookingConfigService> logger)
    {
        _memoryCache = memoryCache;
        _logger = logger;
    }
    
    public async Task<BookingConfig> GetBookingConfig()
    {
        try
        {
            if (_memoryCache.TryGetValue<BookingConfig>(BookingConfigFileName, out var bookingConfig))
            {
                return bookingConfig;
            }
            
            bookingConfig = await GetBookingConfigFromFile();
            
            _memoryCache.Set(BookingConfigFileName, bookingConfig, TimeSpan.FromDays(1));
        
            return bookingConfig;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Не удалось получить конфигурацию для автозаписи");
            throw;
        }
    }

    public async Task UpdateBookingConfig(BookingConfig bookingConfig)
    {
        try
        {
            var cfg = JsonSerializer.Serialize(bookingConfig);
            await File.WriteAllTextAsync(BookingConfigFileName, cfg);
        
            _memoryCache.Set(BookingConfigFileName, bookingConfig, TimeSpan.FromDays(1));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Не удалось обновить конфигурацию для автозаписи");
            throw;
        }
    }

    private async Task<BookingConfig> GetBookingConfigFromFile()
    {
        BookingConfig cfg;
        
        try
        {
            await using var fileStream = new FileStream(BookingConfigFileName, FileMode.Open);
            cfg = await JsonSerializer.DeserializeAsync<BookingConfig>(fileStream);
            
            return cfg;
        }
        catch (FileNotFoundException e)
        {
            _logger.LogWarning(e, "Файла {fileName} для хранения конфига автозаписи не существует. Он будет создан",
                BookingConfigFileName);
            
            cfg = new BookingConfig();
            var configContent = JsonSerializer.Serialize(cfg);
            await File.WriteAllTextAsync(BookingConfigFileName, configContent);

            return cfg;
        }
    }
}