using System.ComponentModel.DataAnnotations;

namespace TicketControlSystem.Data.Models;

public class BaseEntity
{
    [Key]
    public int Id { get; set; }
}