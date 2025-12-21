using Microsoft.EntityFrameworkCore;
using Ticket_control_system.Data;
using TicketControlSystem.Data.Models;

namespace Ticket_control_system.BackgroundServices;

public class DeviceStatusChecker(IServiceProvider serviceProvider) : BackgroundService
{
    private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(30);
    private readonly TimeSpan _timeoutThreshold = TimeSpan.FromMinutes(2);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    var thresholdTime = DateTime.UtcNow - _timeoutThreshold;
                    
                    var deadDevices = await context.Devices
                        .Where(d => d.Status == Status.Active && 
                                    (d.LastHeartbeat == null || d.LastHeartbeat < thresholdTime))
                        .ToListAsync(stoppingToken);

                    if (deadDevices.Any())
                    {
                        foreach (var device in deadDevices)
                        {
                            device.Status = Status.Inactive;
                        }
                        await context.SaveChangesAsync(stoppingToken);
                        Console.WriteLine($"[Watchdog] Set {deadDevices.Count} devices to Inactive.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Watchdog] Error: {ex.Message}");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }
    }
}