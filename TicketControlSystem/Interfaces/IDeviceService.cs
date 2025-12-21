using Ticket_control_system.DTO.Device;
using TicketControlSystem.Data.Models;

namespace Ticket_control_system.Interfaces;

public interface IDeviceService
{
    Task<Device> CreateDeviceAsync(DeviceCreateRequest request, int userId);
    Task<bool> DeleteDeviceAsync(int deviceId, int userId);
    Task<IEnumerable<Device>> GetDevicesByEventIdAsync(int? eventId);
    Task RegisterHeartbeatAsync(int deviceId);
}