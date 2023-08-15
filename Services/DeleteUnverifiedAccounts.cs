using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpaBookingApp.Services
{
    public class DeleteUnverifiedAccounts : BackgroundService
    {
    private readonly ILogger<DeleteUnverifiedAccounts> _logger;
    private readonly IServiceProvider _services;

    public DeleteUnverifiedAccounts(
        ILogger<DeleteUnverifiedAccounts> logger,
        IServiceProvider services)
    {
        _logger = logger;
        _services = services;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _services.CreateScope())
                {
                    var service = scope.ServiceProvider.GetRequiredService<IAuthRepository>();
                    await service.DeleteUnverifiedAccounts();
                }

                // Sleep for 20 minutes before running the next iteration
                await Task.Delay(TimeSpan.FromMinutes(3), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting unverified accounts.");
            }
        }
    }
    }
}