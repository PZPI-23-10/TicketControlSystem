using Ticket_control_system.DTO.Event;

namespace Ticket_control_system.Interfaces;

public interface IEventService
{
    Task<EventResponse> CreateEventAsync(EventCreateRequest request, int ownerId);
    Task<IEnumerable<EventResponse>> GetAllEventsAsync();
    Task<EventResponse> GetEventByIdAsync(int eventId);
    Task<bool> DeleteEventAsync(int eventId);
    Task<EventResponse> UpdateEventAsync(int eventId, EventUpdateRequest request);
}