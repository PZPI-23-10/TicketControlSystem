using TicketControlSystem.Data.Models;

namespace Ticket_control_system.DTO;

public struct UserResponse
{
    public int Id { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public IEnumerable<string> Role { get; set; }
    public ICollection<string> OwnedEvents { get; set; }
}