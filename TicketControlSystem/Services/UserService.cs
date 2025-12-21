using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Ticket_control_system.Data;
using Ticket_control_system.DTO;
using Ticket_control_system.Interfaces;
using TicketControlSystem.Data.Models;

namespace Ticket_control_system.Services;

public class UserService(UserManager<User> userManager,
    ApplicationDbContext? context,
    ITokenService tokenService) :  IUserService
{
    public async Task<IEnumerable<UserResponse>> GetAllUsersAsync()
    {
        var users = await context.Users
            .Include(u => u.OwnedEvents)
            .ToListAsync();
        
        var responseList = new List<UserResponse>();
        foreach (var user in users)
        {
            var roles = await userManager.GetRolesAsync(user);

            responseList.Add(new UserResponse
            {
                Id = user.Id,
                Username = user.UserName,
                Email = user.Email,
                Role = roles,
                OwnedEvents = user.OwnedEvents.Select(e => e.Name).ToList(),
            });
        }
        return responseList;
    }

    public async Task<UserResponse?> GetUserByIdAsync(int id)
    {
        var user = await context.Users
            .Include(u => u.OwnedEvents)
            .FirstOrDefaultAsync(u => u.Id == id);
        if (user == null) return null;

        IEnumerable<string> userRoles = await userManager.GetRolesAsync(user);
        
        return new UserResponse
        {
            Id = user.Id,
            Username = user.UserName,
            Email = user.Email,
            Role = userRoles,
            OwnedEvents = user.OwnedEvents.Select(e => e.Name).ToList(),
        };
    }

    public async Task<UserAuthResponse?> RegisterUserAsync(UserRegisterRequest request)
    {
        var user = new User
        {
            UserName = request.Username,
            Email = request.Email,
        };

        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Creating user error: {errors}");
        }
        
        string roleName = request.Role;
        
        var roleResult = await userManager.AddToRoleAsync(user, roleName);

        if (!roleResult.Succeeded)
        {
            var errors = roleResult.Errors.Select(e => e.Description);
            throw new InvalidOperationException($"Failed to assign role: {string.Join(" | ", errors)}");
        }
        var roles = await userManager.GetRolesAsync(user);
        
        Token accessToken = tokenService.GenerateAccessToken(user.Id.ToString(), user.Email, roles,false);
        
        return new UserAuthResponse(user.Id, accessToken.TokenKey);
    }

    public async Task<UserAuthResponse?> LoginUserAsync(UserLoginRequest request)
    {
        User? user = await context.Set<User>().FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null)
            throw new InvalidOperationException("User with this email does not exist.");
        
        bool passwordValid = await userManager.CheckPasswordAsync(user, request.Password);
    
        if (!passwordValid)
        {
            throw new InvalidOperationException("Invalid Password.");
        }

        var roles = await userManager.GetRolesAsync(user);
        
        Token accessToken = tokenService.GenerateAccessToken(user.Id.ToString(), user.Email, roles, request.RememberMe);

        return new UserAuthResponse(user.Id, accessToken.TokenKey);
    }
    
    public async Task<bool> DeleteUserAsync(int id)
    {
        var user = await context.Users.FindAsync(id);
        if (user == null) return false;

        context.Users.Remove(user);
        await context.SaveChangesAsync();
        return true;
    }
    
    public async Task<bool> UpdateUserAsync(int id, UpdateUserRequest request)
    {
        var existingUser = await context.Users.FindAsync(id);
        if (existingUser == null)
        {
            return false;
        }

        existingUser.UserName = request.Username;
        existingUser.Email = request.Email;
        
        var currentRoles = await userManager.GetRolesAsync(existingUser);
        if (currentRoles.Count > 0)
        {
            var removeResult = await userManager.RemoveFromRolesAsync(existingUser, currentRoles);
            if (!removeResult.Succeeded)
            {
                return false; 
            }
        }
        
        if (!string.IsNullOrEmpty(request.Role))
        {
            var addResult = await userManager.AddToRoleAsync(existingUser, request.Role);
            if (!addResult.Succeeded)
            {
                return false;
            }
        }
        
        await context.SaveChangesAsync();
        return true;
    }
}