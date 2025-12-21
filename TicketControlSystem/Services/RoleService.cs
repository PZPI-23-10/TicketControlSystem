using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Ticket_control_system.DTO;
using Ticket_control_system.Interfaces;
using TicketControlSystem.Data.Models;

namespace Ticket_control_system.Services;

public class RoleService(RoleManager<IdentityRole<int>> roleManager,
    UserManager<User> userManager) : IRoleService
{
    public async Task<IEnumerable<RoleDto>> GetAllRolesAsync()
    {
        return await roleManager.Roles
            .Select(r => new RoleDto
            {
                Name = r.Name,
                Id = r.Id
            })
            .ToListAsync();
    }
    
    public async Task<bool> CreateRoleAsync(string roleName)
    {
        if (await roleManager.RoleExistsAsync(roleName))
        {
            return false;
        }

        var result = await roleManager.CreateAsync(new IdentityRole<int>(roleName));
        
        return result.Succeeded;
    }

    public async Task<bool> DeleteRoleAsync(int roleId)
    {
        var role = await roleManager.FindByIdAsync(roleId.ToString());
        
        if (role == null)
        {
            return false;
        }

        var result = await roleManager.DeleteAsync(role);
        
        return result.Succeeded;
    }

    public async Task<bool> IsExist(string roleName)
    {
        var result = await roleManager.RoleExistsAsync(roleName);
        return result;
    }
    
    public async Task<bool> RemoveAllRolesAsync(int userId)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());

        if (user == null)
        {
            return false; 
        }
        
        var currentRoles = await userManager.GetRolesAsync(user);

        if (currentRoles.Count == 0)
        {
            return true; 
        }
        
        var result = await userManager.RemoveFromRolesAsync(user, currentRoles);

        return result.Succeeded;
    }
    
    public async Task<bool> AddRoleToUserAsync(int userId, string roleName)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        
        if (user == null)
        {
            return false;
        }
        
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            return false;
        }
        
        if (await userManager.IsInRoleAsync(user, roleName))
        {
            return true; 
        }
        
        var result = await userManager.AddToRoleAsync(user, roleName);
        return result.Succeeded;
    }
}