using Microsoft.EntityFrameworkCore;
using Ticket_control_system.Data;
using Ticket_control_system.DTO.Ticket;
using Ticket_control_system.Interfaces;
using TicketControlSystem.Data.Models;

namespace Ticket_control_system.Services;

public class TicketService(ApplicationDbContext context) : ITicketService
{
    public async Task<TicketResponse> CreateTicketAsync(TicketCreateRequest request, int userId)
    {
        var tariff = await context.Tariffs
            .Include(t => t.Event)
            .FirstOrDefaultAsync(t => t.Id == request.TariffId);

        if (tariff == null)
            throw new KeyNotFoundException("Tariff not found.");

        if (tariff.Event == null)
            throw new InvalidOperationException("Tariff is not assigned to any event.");
        
        if (tariff.Event.OwnerId != userId)
        {
            throw new UnauthorizedAccessException("You are not authorized to issue tickets for this event.");
        }
        
        string uniqueId = Guid.NewGuid().ToString();
        
        var ticket = new Ticket
        {
            TariffId = tariff.Id,
            TicketUid = uniqueId,
            TicketOwnerName = request.TicketOwnerName,
            Status = "active",
            MaxUses = request.MaxUses,
            CurrentUses = 0,
            ValidFrom = request.ValidFrom ?? DateTime.UtcNow,
            ValidTo = request.ValidTo ?? tariff.Event.EndTime 
        };

        context.Tickets.Add(ticket);
        await context.SaveChangesAsync();

        return MapToResponse(ticket, tariff.Name, tariff.Event.Name);
    }

    public async Task<TicketResponse> GetTicketByIdPublicAsync(int userId)
    {
        var myticket = context.Tickets
            .Include(t => t.Tariff)
            .ThenInclude(tr => tr.Event)
            .FirstOrDefault(t => t.Id == userId);
        
        var ticket = new Ticket
        {
            TariffId = myticket.Id,
            TicketUid = myticket.TicketUid,
            TicketOwnerName = myticket.TicketOwnerName,
            Status = myticket.Status,
            MaxUses = myticket.MaxUses,
            CurrentUses = myticket.CurrentUses,
            ValidFrom = myticket.ValidFrom,
            ValidTo = myticket.ValidTo
        };
        
        return MapToResponse(ticket, myticket.Tariff.Name, myticket.Tariff.Event.Name);
    }

    public async Task<IEnumerable<TicketResponse>> GetTicketsByEventIdAsync(int? eventId)
    {
        var query = context.Tickets
            .Include(t => t.Tariff)
            .ThenInclude(tr => tr.Event)
            .AsQueryable();
        

        if (eventId.HasValue && eventId.Value != 0)
        {
            query = query.Where(t => t.Tariff.EventId == eventId.Value);
        }

        var tickets = await query.ToListAsync();

        return tickets.Select(t => MapToResponse(t, t.Tariff.Name, t.Tariff.Event.Name));
    }
    
    public async Task<bool> DeleteTicketAsync(int ticketId, int userId)
    {
        var ticket = await context.Tickets
            .Include(t => t.Tariff)
            .ThenInclude(tr => tr.Event)
            .FirstOrDefaultAsync(t => t.Id == ticketId);

        if (ticket == null) return false;
        
        if (ticket.Tariff.Event.OwnerId != userId)
            throw new UnauthorizedAccessException("Access denied.");

        context.Tickets.Remove(ticket);
        await context.SaveChangesAsync();
        return true;
    }

    private static TicketResponse MapToResponse(Ticket ticket, string tariffName, string eventName)
    {
        return new TicketResponse
        {
            Id = ticket.Id,
            TicketUid = ticket.TicketUid,
            OwnerName = ticket.TicketOwnerName,
            TariffName = tariffName,
            EventName = eventName,
            Status = ticket.Status,
            MaxUses = ticket.MaxUses,
            CurrentUses = ticket.CurrentUses,
            ValidTo = ticket.ValidTo
        };
    }
}