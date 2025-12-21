using Microsoft.AspNetCore.Mvc;
using Ticket_control_system.Interfaces;

namespace Ticket_control_system.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoleController(IRoleService roleService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetRoles()
    {
        var roles = await roleService.GetAllRolesAsync();
        return Ok(roles);
    }

    [HttpPost]
    public async Task<IActionResult> CreateRole([FromBody] string roleName)
    {
        if (string.IsNullOrWhiteSpace(roleName))
            return BadRequest("Role name cannot be empty.");

        var result = await roleService.CreateRoleAsync(roleName);

        if (result)
            return Ok($"Role '{roleName}' created successfully.");
            
        return BadRequest("Role creation failed or role already exists.");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRole(int id)
    {
        var result = await roleService.DeleteRoleAsync(id);

        if (result)
            return Ok($"Role with ID {id} deleted.");

        return NotFound($"Role with ID {id} not found.");
    }
}