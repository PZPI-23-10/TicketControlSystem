namespace Ticket_control_system.DTO.Validation;

public class ValidationResponse
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime ValidationTime { get; set; }
    public string? TicketOwner { get; set; }
    public int CurrentUses { get; set; }
    public int MaxUses { get; set; }
}