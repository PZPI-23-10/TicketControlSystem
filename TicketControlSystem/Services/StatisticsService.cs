using Microsoft.EntityFrameworkCore;
using Ticket_control_system.Data;
using Ticket_control_system.DTO.Statistic;
using Ticket_control_system.Interfaces;

namespace Ticket_control_system.Services;

public class StatisticsService(ApplicationDbContext context) : IStatisticsService
{
    public async Task<EventStatisticsResponse> GetEventStatisticsAsync(int eventId)
    {
        var eventEntity = await context.Events
            .FirstOrDefaultAsync(e => e.Id == eventId);

        if (eventEntity == null)
            throw new KeyNotFoundException("Event not found");

        var tickets = await context.Tickets
            .Include(t => t.Tariff)
            .Where(t => t.Tariff.EventId == eventId)
            .ToListAsync();

        int totalIssued = tickets.Count;

        var usedTickets = tickets.Where(t => t.CurrentUses > 0).ToList();
        int totalUsed = usedTickets.Count;

        decimal potentialRevenue = tickets.Sum(t => t.Tariff.Price);

        decimal actualRevenue = usedTickets.Sum(t => t.Tariff.Price);

        double attendancePercent = totalIssued == 0 ? 0 : (double)totalUsed / totalIssued * 100;

        var tariffStats = tickets
            .GroupBy(t => t.Tariff.Name)
            .Select(g => new TariffStatistics
            {
                TariffName = g.Key,
                TicketsSold = g.Count(),
                TicketsUsed = g.Count(t => t.CurrentUses > 0),
                RevenueGenerated = g.Where(t => t.CurrentUses > 0).Sum(t => t.Tariff.Price)
            })
            .ToList();

        return new EventStatisticsResponse
        {
            EventId = eventEntity.Id,
            EventName = eventEntity.Name,
            TotalRevenue = actualRevenue,
            PotentialRevenue = potentialRevenue,
            TotalTicketsIssued = totalIssued,
            TicketsUsed = totalUsed,
            AttendancePercentage = Math.Round(attendancePercent, 2),
            TariffStats = tariffStats
        };
    }
}