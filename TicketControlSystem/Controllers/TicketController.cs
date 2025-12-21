using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QRCoder;
using Ticket_control_system.DTO.Ticket;
using Ticket_control_system.Interfaces;

namespace Ticket_control_system.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TicketController(ITicketService ticketService) : ControllerBase
{
    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> IssueTicket([FromBody] TicketCreateRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var userId = GetCurrentUserId();
            var ticket = await ticketService.CreateTicketAsync(request, userId);

            return Ok(ticket);
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

    [HttpGet]
    public async Task<IActionResult> GetEventTickets([FromQuery] int? eventId)
    {
        try
        {
            var tickets = await ticketService.GetTicketsByEventIdAsync(eventId);
            return Ok(tickets);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, ex.Message);
        }
    }
    
    [HttpDelete("{id}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> RevokeTicket(int id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await ticketService.DeleteTicketAsync(id, userId);
            if (!result) return NotFound();
            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, ex.Message);
        }
    }

    private int GetCurrentUserId()
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdString))
            throw new UnauthorizedAccessException("User ID missing in token.");
        
        return int.Parse(userIdString);
    }
    
    [HttpGet("{id}/qr")]
    public async Task<IActionResult> GetTicketQr(int id)
    {
        var ticketResponse = await ticketService.GetTicketByIdPublicAsync(id); 
        if (ticketResponse == null) return NotFound("Ticket not found");

        string payload = ticketResponse.TicketUid;
        
        using var qrGenerator = new QRCodeGenerator();
        var qrCodeData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
        
        using var qrCode = new PngByteQRCode(qrCodeData);
        byte[] qrCodeImage = qrCode.GetGraphic(20);
        
        return File(qrCodeImage, "image/png");
    }
}