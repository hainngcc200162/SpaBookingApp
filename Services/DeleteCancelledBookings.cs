using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SpaBookingApp.Services
{
    public class DeleteCancelledBookings : BackgroundService
    {
        private readonly ILogger<DeleteCancelledBookings> _logger;
        private readonly IServiceProvider _services;

        public DeleteCancelledBookings(
            ILogger<DeleteCancelledBookings> logger,
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
                        var service = scope.ServiceProvider.GetRequiredService<IBookingService>();
                    await service.DeleteCancelledBookings();
                    }

                    // Sleep for 3 minutes before running the next iteration
                    await Task.Delay(TimeSpan.FromSeconds(3), stoppingToken);

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while deleting cancelled bookings.");
                }
            }
        }
    }
}
