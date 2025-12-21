using Ticket_control_system.DTO;

namespace Ticket_control_system.Interfaces;

public interface ITokenService
{
    Token GenerateAccessToken(string id, string email, IList<string> roles, bool rememberMe);
    DateTime GetTokenExpirationTime(string token);
}