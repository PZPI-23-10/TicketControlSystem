using Microsoft.EntityFrameworkCore;
using Ticket_control_system.Data;
using Ticket_control_system.DTO.Event;
using Ticket_control_system.Interfaces;
using TicketControlSystem.Data.Models;

namespace Ticket_control_system.Services;

public class EventService(ApplicationDbContext context) : IEventService
{
    public async Task<EventResponse> CreateEventAsync(EventCreateRequest request, int ownerId)
    {
        if (request.StartTime >= request.EndTime)
        {
            throw new ArgumentException("Start time must be before End time.");
        }
        
        var newEvent = new Event
        {
            Name = request.Name,
            EventType = request.EventType,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            OwnerId = ownerId
        };

        context.Events.Add(newEvent);
        await context.SaveChangesAsync();
        
        return new EventResponse
        {
            Id = newEvent.Id,
            Name = newEvent.Name,
            EventType = newEvent.EventType.ToString(),
            StartTime = newEvent.StartTime,
            EndTime = newEvent.EndTime,
            OwnerId = newEvent.OwnerId
        };
    }

    public async Task<IEnumerable<EventResponse>> GetAllEventsAsync()
    {
        return await context.Events
            .Select(e => new EventResponse
            {
                Id = e.Id,
                Name = e.Name,
                EventType = e.EventType.ToString(),
                StartTime = e.StartTime,
                EndTime = e.EndTime,
                OwnerId = e.OwnerId
            })
            .ToListAsync();
    }

    public async Task<EventResponse?> GetEventByIdAsync(int id)
    {
        var myevent = await context.Events.FirstOrDefaultAsync(e => e.Id == id);

        return new EventResponse
        {
            Id = myevent.Id,
            Name = myevent.Name,
            EventType = myevent.EventType.ToString(),
            StartTime = myevent.StartTime,
            EndTime = myevent.EndTime,
            OwnerId = myevent.OwnerId
        };
    }

    public async Task<bool> DeleteEventAsync(int id)
    {
        var myevent = await context.Events.FirstOrDefaultAsync(e => e.Id == id);
        if (myevent == null) return false;

        context.Events.Remove(myevent);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<EventResponse> UpdateEventAsync(int id, EventUpdateRequest request)
    {
        var myevent = await context.Events.FirstOrDefaultAsync(e => e.Id == id);
        if (myevent == null) return null;
        
        myevent.Name = request.Name;
        myevent.EventType = request.EventType;
        myevent.StartTime = request.StartTime;
        myevent.EndTime = request.EndTime;
        
        await context.SaveChangesAsync();
        return new EventResponse
        {
            Id = myevent.Id,
            Name = myevent.Name,
            EventType = myevent.EventType.ToString(),
            StartTime = myevent.StartTime,
            EndTime = myevent.EndTime,
            OwnerId = myevent.OwnerId
        };
    }
}