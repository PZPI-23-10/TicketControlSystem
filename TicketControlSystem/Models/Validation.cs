using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketControlSystem.Data.Models;

public class Validation : BaseEntity
{
    [ForeignKey(nameof(Ticket))]
    public int TicketId { get; set; }
    [ForeignKey(nameof(Device))]
    public int DeviceId { get; set; }
    public DateTime ValidationTime { get; set; } = DateTime.UtcNow;
    public Result Result { get; set; }
    public Ticket? Ticket { get; set; }
    public Device? Device { get; set; }
}

public enum Result
{
    Success,
    Invalid,
}