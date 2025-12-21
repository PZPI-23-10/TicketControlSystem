using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketControlSystem.Data.Models;

public class Ticket : BaseEntity
{
    [ForeignKey(nameof(Tariff))]
    public int TariffId { get; set; }
    public string TicketUid { get; set; } = string.Empty;
    public string Status { get; set; } = "active";
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
    public int MaxUses { get; set; }
    public int CurrentUses { get; set; }
    public string TicketOwnerName { get; set; }
    public Tariff? Tariff { get; set; }
    public ICollection<Validation>? Validations { get; set; }
}
