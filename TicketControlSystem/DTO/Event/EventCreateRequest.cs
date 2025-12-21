using System.ComponentModel.DataAnnotations;
using TicketControlSystem.Data.Models;


namespace Ticket_control_system.DTO.Event;

public class EventCreateRequest
{
    [Required(ErrorMessage = "Event name is required.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Event name must be between 3 and 50 characters.")]
    public string Name { get; set; } = string.Empty;

    [Required]
    public EventType EventType { get; set; }

    [Required]
    public DateTime StartTime { get; set; }

    [Required]
    public DateTime EndTime { get; set; }
}