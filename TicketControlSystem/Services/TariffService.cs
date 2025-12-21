using Microsoft.EntityFrameworkCore;
using Ticket_control_system.Data;
using Ticket_control_system.DTO.Tariff;
using Ticket_control_system.Interfaces;
using TicketControlSystem.Data.Models;

namespace Ticket_control_system.Services;

public class TariffService(ApplicationDbContext context) : ITariffService
{
    public async Task<TariffResponse> CreateTariffAsync(TariffCreateRequest request, int userId)
    {
        var targetEvent = await context.Events.FindAsync(request.EventId);

        if (targetEvent == null)
        {
            throw new KeyNotFoundException("Event not found.");
        }
        
        if (targetEvent.OwnerId != userId)
        {
            throw new UnauthorizedAccessException("You are not the owner of this event.");
        }

        // 3. Создаем тариф
        var tariff = new Tariff
        {
            EventId = request.EventId,
            Name = request.Name,
            Price = request.Price
        };

        context.Tariffs.Add(tariff);
        await context.SaveChangesAsync();

        return new TariffResponse
        {
            Id = tariff.Id,
            EventId = tariff.EventId,
            Name = tariff.Name,
            Price = tariff.Price
        };
    }

    public async Task<bool> DeleteTariffAsync(int tariffId, int userId)
    {
        var tariff = await context.Tariffs
            .Include(t => t.Event)
            .FirstOrDefaultAsync(t => t.Id == tariffId);

        if (tariff == null) return false;
        
        if (tariff.Event != null && tariff.Event.OwnerId != userId)
        {
            throw new UnauthorizedAccessException("You are not the owner of the event associated with this tariff.");
        }

        context.Tariffs.Remove(tariff);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<TariffResponse>> GetTariffsByEventIdAsync(int? eventId)
    {
        var query = context.Tariffs.AsQueryable();
        
        if (eventId.HasValue && eventId.Value != 0)
        {
            query = query.Where(t => t.EventId == eventId.Value);
        }

        return await query
            .Select(t => new TariffResponse
            {
                Id = t.Id,
                EventId = t.EventId,
                Name = t.Name,
                Price = t.Price
            })
            .ToListAsync();
    }
}