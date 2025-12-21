using Ticket_control_system.DTO.Statistic;

namespace Ticket_control_system.Interfaces;

public interface IStatisticsService
{
    Task<EventStatisticsResponse> GetEventStatisticsAsync(int eventId);
}