using System.ComponentModel.DataAnnotations;

namespace Ticket_control_system.DTO.Device;

public class DeviceCreateRequest
{
    [Required]
    public int EventId { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
}