using Ticket_control_system.DTO.Tariff;

namespace Ticket_control_system.Interfaces;

public interface ITariffService
{
    Task<TariffResponse> CreateTariffAsync(TariffCreateRequest request, int userId);
    Task<bool> DeleteTariffAsync(int tariffId, int userId);
    Task<IEnumerable<TariffResponse>> GetTariffsByEventIdAsync(int? eventId);
}