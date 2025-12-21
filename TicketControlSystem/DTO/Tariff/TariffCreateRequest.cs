using System.ComponentModel.DataAnnotations;

namespace Ticket_control_system.DTO.Tariff;

public class TariffCreateRequest
{
    [Required]
    public int EventId { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "Price must be greater than or equal to 0")]
    public decimal Price { get; set; }
}