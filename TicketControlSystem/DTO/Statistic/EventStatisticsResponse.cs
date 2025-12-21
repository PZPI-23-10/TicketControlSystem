namespace Ticket_control_system.DTO.Statistic;

public class EventStatisticsResponse
{
    public int EventId { get; set; }
    public string EventName { get; set; } = string.Empty;
    
    public decimal TotalRevenue { get; set; } 
    public decimal PotentialRevenue { get; set; } 
    
    public int TotalTicketsIssued { get; set; }
    public int TicketsUsed { get; set; }
    public double AttendancePercentage { get; set; } 

    public List<TariffStatistics> TariffStats { get; set; } = new();
}

public class TariffStatistics
{
    public string TariffName { get; set; } = string.Empty;
    public int TicketsSold { get; set; }
    public int TicketsUsed { get; set; }
    public decimal RevenueGenerated { get; set; }
}