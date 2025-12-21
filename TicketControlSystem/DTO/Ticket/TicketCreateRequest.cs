using System.ComponentModel.DataAnnotations;

namespace Ticket_control_system.DTO.Ticket;

public class TicketCreateRequest
{
    [Required]
    public int TariffId { get; set; }

    [Required]
    public string TicketOwnerName { get; set; } = string.Empty; 
    
    public DateTime? ValidFrom { get; set; } 
    public DateTime? ValidTo { get; set; }
    
    [Required]
    [Range(1, int.MaxValue)]
    public int MaxUses { get; set; }
}