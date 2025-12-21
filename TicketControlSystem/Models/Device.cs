using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketControlSystem.Data.Models;

public class Device : BaseEntity
{
    [ForeignKey(nameof(Event))]
    public int EventId { get; set; }
    public string Name { get; set; } = string.Empty;
    public Status Status { get; set; }
    public DateTime? LastHeartbeat { get; set; }
    public string Location { get; set; } = string.Empty;
    public Event? Event { get; set; }
    public ICollection<Validation>? Validations { get; set; }
}

public enum Status
{
    Active,
    Inactive
}