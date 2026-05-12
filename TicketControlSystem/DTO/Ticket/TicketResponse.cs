namespace Ticket_control_system.DTO.Ticket;

public class TicketResponse
{
    public int Id { get; set; }
    public int? OwnerUserId { get; set; }
    public string TicketUid { get; set; } = string.Empty;
    public string TariffName { get; set; } = string.Empty;
    public string EventName { get; set; } = string.Empty;
    public string OwnerName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int MaxUses { get; set; }
    public int CurrentUses { get; set; }
    public DateTime ValidTo { get; set; }
}
