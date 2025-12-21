using Microsoft.EntityFrameworkCore;
using Ticket_control_system.Data;
using Ticket_control_system.DTO.Validation;
using Ticket_control_system.Interfaces;
using TicketControlSystem.Data.Models;

namespace Ticket_control_system.Services;

public class ValidationService(ApplicationDbContext context) : IValidationService
{
    public async Task<ValidationResponse> ValidateTicketAsync(ValidationRequest request)
    {
        var device = await context.Devices.FindAsync(request.DeviceId);
        
        if (device == null || device.Status == Status.Inactive)
            return BuildResponse(false, "Device unavailable", null);

        var ticket = await context.Tickets
            .Include(t => t.Tariff)
            .ThenInclude(tr => tr.Event)
            .FirstOrDefaultAsync(t => t.TicketUid == request.TicketUid);
        
        if (ticket == null)
        {
            return BuildResponse(false, "Ticket not found", null);
        }
        
        if (ticket.Tariff.EventId != device.EventId)
        {
            await SaveValidationLogAsync(device.Id, ticket.Id, Result.Invalid);
            return BuildResponse(false, "Ticket is for another event", ticket);
        }

        if (ticket.Status != "active")
        {
            await SaveValidationLogAsync(device.Id, ticket.Id, Result.Invalid);
            return BuildResponse(false, $"Ticket status is {ticket.Status}", ticket);
        }
        
        var now = DateTime.UtcNow;
        if (now < ticket.ValidFrom || now > ticket.ValidTo)
        {
            await SaveValidationLogAsync(device.Id, ticket.Id, Result.Invalid);
            return BuildResponse(false, "Ticket expired or time not valid", ticket);
        }
        
        if (ticket.CurrentUses >= ticket.MaxUses)
        {
            await SaveValidationLogAsync(device.Id, ticket.Id, Result.Invalid);
            return BuildResponse(false, "All ticket uses consumed", ticket);
        }

        ticket.CurrentUses++;

        context.Tickets.Update(ticket);
        
        await SaveValidationLogAsync(device.Id, ticket.Id, Result.Success);

        return BuildResponse(true, "Welcome!", ticket);
    }

    public async Task<IEnumerable<ValidationResponse>> GetValidationsAsync(int? eventId)
    {
        var query = context.Validations
            .Include(v => v.Ticket)       
            .Include(v => v.Device)       
            .AsQueryable();

        if (eventId.HasValue && eventId.Value != 0)
        {
            query = query.Where(v => v.Device != null && v.Device.EventId == eventId.Value);
        }

        var result = await query
            .OrderByDescending(v => v.ValidationTime)
            .ToListAsync();

        return result.Select(v => new ValidationResponse
        {
            IsSuccess = v.Result == Result.Success,
            Message = v.Result == Result.Success ? "Access Granted" : "Access Denied",
            ValidationTime = v.ValidationTime,
            TicketOwner = v.Ticket?.TicketOwnerName ?? "Unknown",
            CurrentUses = v.Ticket?.CurrentUses ?? 0,
            MaxUses = v.Ticket?.MaxUses ?? 0
        });
    }

    private async Task SaveValidationLogAsync(int deviceId, int ticketId, Result result)
    {
        var validation = new Validation
        {
            DeviceId = deviceId,
            TicketId = ticketId,
            Result = result,
            ValidationTime = DateTime.UtcNow
        };

        context.Validations.Add(validation);
        await context.SaveChangesAsync();
    }

    private static ValidationResponse BuildResponse(bool success, string msg, Ticket? ticket)
    {
        return new ValidationResponse
        {
            IsSuccess = success,
            Message = msg,
            ValidationTime = DateTime.UtcNow,
            TicketOwner = ticket?.TicketOwnerName,
            CurrentUses = ticket?.CurrentUses ?? 0,
            MaxUses = ticket?.MaxUses ?? 0
        };
    }

}