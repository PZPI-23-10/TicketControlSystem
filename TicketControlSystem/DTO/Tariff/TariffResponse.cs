namespace Ticket_control_system.DTO.Tariff;

public class TariffResponse
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}