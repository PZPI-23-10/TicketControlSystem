using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ticket_control_system.Interfaces;

namespace Ticket_control_system.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StatisticsController(IStatisticsService statsService) : ControllerBase
{
    [HttpGet("{eventId}")]
    public async Task<IActionResult> GetEventStats(int eventId)
    {
        try
        {
            var stats = await statsService.GetEventStatisticsAsync(eventId);
            
            return Ok(stats);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, ex.Message);
        }
    }
}