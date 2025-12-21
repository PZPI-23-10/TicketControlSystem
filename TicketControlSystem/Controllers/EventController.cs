using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ticket_control_system.DTO.Event;
using Ticket_control_system.Interfaces;

namespace TicketSystem.Controllers;

[ApiController]
[Route("api/Events")]
public class EventController(IEventService eventService) : ControllerBase
{
    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Owner,Admin")]
    public async Task<IActionResult> CreateEvent([FromBody] EventCreateRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        try
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdString))
                return Unauthorized("User ID not found in token.");

            int userId = int.Parse(userIdString);
            
            var createdEvent = await eventService.CreateEventAsync(request, userId);

            return Ok(createdEvent);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error: " + ex.Message);
        }
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllEvents()
    {
        var events = await eventService.GetAllEventsAsync();
        return Ok(events);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetEventById(int id)
    {
        var myevent = await eventService.GetEventByIdAsync(id);
        return Ok(myevent);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEvent(int id, EventUpdateRequest request)
    {
        var updated = await eventService.UpdateEventAsync(id, request);

        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEvent(int id)
    {
        await eventService.DeleteEventAsync(id);
        
        return Ok();
    }
}