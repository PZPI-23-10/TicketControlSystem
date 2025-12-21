using Ticket_control_system.DTO.Tariff;
using Ticket_control_system.DTO.Validation;

namespace Ticket_control_system.Interfaces;

public interface IValidationService
{
    Task<ValidationResponse> ValidateTicketAsync(ValidationRequest request);
    Task<IEnumerable<ValidationResponse>> GetValidationsAsync(int? eventId);
}