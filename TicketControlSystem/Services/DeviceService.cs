using Microsoft.EntityFrameworkCore;
using Ticket_control_system.Data;
using Ticket_control_system.DTO.Device;
using Ticket_control_system.Interfaces;
using TicketControlSystem.Data.Models;

namespace Ticket_control_system.Services;

public class DeviceService(ApplicationDbContext context) : IDeviceService
{
    public async Task<Device> CreateDeviceAsync(DeviceCreateRequest request, int userId)
    {
        var eventEntity = await context.Events.FindAsync(request.EventId);
        if (eventEntity == null) throw new KeyNotFoundException("Event not found");

        if (eventEntity.OwnerId != userId)
            throw new UnauthorizedAccessException("You are not the owner of this event.");

        var device = new Device
        {
            EventId = request.EventId,
            Name = request.Name,
            Location = request.Location,
            Status = Status.Active
        };

        context.Devices.Add(device);
        await context.SaveChangesAsync();
        return device;
    }

    public async Task<bool> DeleteDeviceAsync(int deviceId, int userId)
    {
        var device = await context.Devices
            .Include(d => d.Event)
            .FirstOrDefaultAsync(d => d.Id == deviceId);

        if (device == null) return false;

        if (device.Event!.OwnerId != userId)
            throw new UnauthorizedAccessException("Access denied.");

        context.Devices.Remove(device);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Device>> GetDevicesByEventIdAsync(int? eventId)
    {
        var query = context.Devices
            .Include(d => d.Event) 
            .ThenInclude(e => e.Tariffs)   
            .Include(d => d.Event)
            .ThenInclude(e => e.Owner)    
            .AsQueryable();

        if (eventId.HasValue && eventId.Value != 0)
        {
            query = query.Where(d => d.EventId == eventId.Value);
        }

        return await query.ToListAsync();
    }
    
    public async Task RegisterHeartbeatAsync(int deviceId)
    {
        var device = await context.Devices.FindAsync(deviceId);
        if (device == null) return;

        device.LastHeartbeat = DateTime.UtcNow;
        
        if (device.Status == Status.Inactive)
        {
            device.Status = Status.Active;
        }

        await context.SaveChangesAsync();
    }
}