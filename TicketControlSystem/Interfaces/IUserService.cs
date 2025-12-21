using Ticket_control_system.DTO;

namespace Ticket_control_system.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserResponse>> GetAllUsersAsync();
    Task<UserResponse?> GetUserByIdAsync(int id);
    Task<UserAuthResponse?> RegisterUserAsync(UserRegisterRequest request);
    Task<UserAuthResponse> LoginUserAsync(UserLoginRequest request);
    Task<bool> DeleteUserAsync(int id);
    Task<bool> UpdateUserAsync(int id, UpdateUserRequest request);
}