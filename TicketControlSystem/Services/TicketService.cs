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

        User? ownerUser = null;
        if (request.OwnerUserId.HasValue)
        {
            ownerUser = await context.Users.FindAsync(request.OwnerUserId.Value);
            if (ownerUser == null)
                throw new KeyNotFoundException("Ticket owner user not found.");
        }
        
        string uniqueId = Guid.NewGuid().ToString();
        
        var ticket = new Ticket
        {
            TariffId = tariff.Id,
            OwnerUserId = ownerUser?.Id,
            TicketUid = uniqueId,
            TicketOwnerName = string.IsNullOrWhiteSpace(request.TicketOwnerName)
                ? ownerUser?.UserName ?? ownerUser?.Email ?? string.Empty
                : request.TicketOwnerName,
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

    public async Task<TicketResponse?> GetTicketByIdPublicAsync(int ticketId)
    {
        var myticket = await context.Tickets
            .Include(t => t.Tariff)
            .ThenInclude(tr => tr.Event)
            .FirstOrDefaultAsync(t => t.Id == ticketId);

        if (myticket == null) return null;
        
        return MapToResponse(myticket, myticket.Tariff.Name, myticket.Tariff.Event.Name);
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

    public async Task<IEnumerable<TicketResponse>> GetTicketsByOwnerUserIdAsync(int ownerUserId)
    {
        var tickets = await context.Tickets
            .Include(t => t.Tariff)
            .ThenInclude(tr => tr.Event)
            .Where(t => t.OwnerUserId == ownerUserId)
            .ToListAsync();

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
            OwnerUserId = ticket.OwnerUserId,
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
