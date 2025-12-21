using System.ComponentModel.DataAnnotations;

namespace Ticket_control_system.DTO.Validation;

public class ValidationRequest
{
    [Required]
    public string TicketUid { get; set; } = string.Empty;
    
    [Required]
    public int DeviceId { get; set; }
}