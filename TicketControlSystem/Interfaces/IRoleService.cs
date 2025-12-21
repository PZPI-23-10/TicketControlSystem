using Ticket_control_system.DTO;

namespace Ticket_control_system.Interfaces;

public interface IRoleService
{
    Task<IEnumerable<RoleDto>> GetAllRolesAsync();
    Task<bool> CreateRoleAsync(string roleName);
    Task<bool> DeleteRoleAsync(int roleId);
    Task<bool> IsExist(string roleName);
    Task<bool> RemoveAllRolesAsync(int userId);
    Task<bool> AddRoleToUserAsync (int userId, string roleName);
}