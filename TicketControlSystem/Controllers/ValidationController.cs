using Microsoft.AspNetCore.Mvc;
using Ticket_control_system.DTO.Validation;
using Ticket_control_system.Interfaces;

namespace Ticket_control_system.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ValidationController(IValidationService validationService) : ControllerBase
{
    [HttpPost("check")]
    public async Task<IActionResult> Validate([FromBody] ValidationRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var response = await validationService.ValidateTicketAsync(request);
        
        return Ok(response);
    }
    
    
    [HttpGet]
    public async Task<IActionResult> GetValidations([FromQuery] int? eventId)
    {
        try
        {
            var logs = await validationService.GetValidationsAsync(eventId);
            return Ok(logs);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Error retrieving validations: " + ex.Message);
        }
    }
}