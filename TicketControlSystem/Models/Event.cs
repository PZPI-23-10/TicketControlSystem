using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketControlSystem.Data.Models;

public class Event : BaseEntity
{
    [ForeignKey(nameof(Owner))]
    public int OwnerId { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    public EventType EventType { get; set; } = EventType.Other;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    public ICollection<Tariff>? Tariffs { get; set; }
    public User? Owner { get; set; }
}

public enum EventType
{
    Concert = 0,
    Conference = 1,
    Sport = 2,
    Theater = 3,
    Other = 4
}