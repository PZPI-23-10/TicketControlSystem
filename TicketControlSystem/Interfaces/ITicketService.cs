using Ticket_control_system.DTO.Ticket;

namespace Ticket_control_system.Interfaces;

public interface ITicketService
{
    Task<TicketResponse> CreateTicketAsync(TicketCreateRequest request, int userId);
    Task<IEnumerable<TicketResponse>> GetTicketsByEventIdAsync(int? eventId);
    Task<IEnumerable<TicketResponse>> GetTicketsByOwnerUserIdAsync(int ownerUserId);
    Task<bool> DeleteTicketAsync(int ticketId, int userId);
    Task<TicketResponse?> GetTicketByIdPublicAsync(int ticketId);
}
