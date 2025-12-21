namespace Ticket_control_system.DTO.Event;

public class EventResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string EventType { get; set; } 
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int OwnerId { get; set; } 
}