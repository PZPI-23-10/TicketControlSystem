using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ticket_control_system.DTO.Device;
using Ticket_control_system.Interfaces;

namespace Ticket_control_system.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DeviceController(IDeviceService deviceService) : ControllerBase
{
    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> AddDevice([FromBody] DeviceCreateRequest request)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var device = await deviceService.CreateDeviceAsync(request, userId);
            return Ok(device);
        }
        catch (UnauthorizedAccessException ex) { return StatusCode(403, ex.Message); }
        catch (Exception ex) { return BadRequest(ex.Message); }
    }

    [HttpDelete("{id}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> RemoveDevice(int id)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var res = await deviceService.DeleteDeviceAsync(id, userId);
        return res ? NoContent() : NotFound();
    }
    
    [HttpGet]
    public async Task<IActionResult> GetDevices([FromQuery] int? eventId)
    {
        var devices = await deviceService.GetDevicesByEventIdAsync(eventId);
        return Ok(devices);
    }
    
    [HttpPost("heartbeat/{deviceId}")]
    public async Task<IActionResult> Heartbeat(int deviceId)
    {
        await deviceService.RegisterHeartbeatAsync(deviceId);
        return Ok();
    }
}