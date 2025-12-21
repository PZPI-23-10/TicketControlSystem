using System.ComponentModel.DataAnnotations;

namespace Ticket_control_system.DTO;

public struct UpdateUserRequest
{
    [Required(ErrorMessage = "Username is required.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters.")]
    public string Username { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Role is required.")]
    public string Role { get; set; }
}