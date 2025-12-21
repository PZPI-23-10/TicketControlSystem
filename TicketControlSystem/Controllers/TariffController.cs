using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ticket_control_system.DTO.Tariff;
using Ticket_control_system.Interfaces;

namespace Ticket_control_system.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TariffController(ITariffService tariffService) : ControllerBase
{
    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> CreateTariff([FromBody] TariffCreateRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var userId = GetCurrentUserId();
            var result = await tariffService.CreateTariffAsync(request, userId);
            return CreatedAtAction(nameof(GetTariffsByEvent), new { eventId = result.EventId }, result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error: " + ex.Message);
        }
    }

    [HttpDelete("{id}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> DeleteTariff(int id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var success = await tariffService.DeleteTariffAsync(id, userId);

            if (!success)
                return NotFound("Tariff not found.");

            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, ex.Message);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetTariffsByEvent([FromQuery] int? eventId)
    {
        var tariffs = await tariffService.GetTariffsByEventIdAsync(eventId);
        return Ok(tariffs);
    }
    
    private int GetCurrentUserId()
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdString))
            throw new UnauthorizedAccessException("User ID missing in token.");
        
        return int.Parse(userIdString);
    }
}