namespace Ticket_control_system.DTO;

public class Token
{
    public string TokenKey { get; set; } = string.Empty;
    public DateTime Expires { get; set; }
}