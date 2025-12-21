using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketControlSystem.Data.Models;

public class Tariff : BaseEntity
{
    [ForeignKey(nameof(Event))]
    public int EventId { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    public decimal Price { get; set; }

    public Event? Event { get; set; }
    public ICollection<Ticket>? Tickets { get; set; }
}