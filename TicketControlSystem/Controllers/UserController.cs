using Microsoft.AspNetCore.Mvc;
using Ticket_control_system.DTO;
using Ticket_control_system.Interfaces;

namespace Ticket_control_system.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController(IUserService userService, IRoleService roleService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserResponse>>> GetUsers()
        {
            var users = await userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponse>> GetUser(int id)
        {
            var user = await userService.GetUserByIdAsync(id);
            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult<UserResponse>> CreateUser([FromBody] UserRegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!(await roleService.IsExist(request.Role)))
            {
                return BadRequest("Role does not exist");
            }
            
            await userService.RegisterUserAsync(request);
            
            return Created();
        }
        
        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<UserAuthResponse>> LoginUser([FromBody] UserLoginRequest request)
        {
            UserAuthResponse result = await userService.LoginUserAsync(request);

            return Ok(result);
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var deleted = await userService.DeleteUserAsync(id);
            
            if (!deleted)
                return NotFound();

            return NoContent();
        }
        
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequest request)
        {
            var updated = await userService.UpdateUserAsync(id, request);
            
            if (!updated)
                return NotFound("User not found");

            return NoContent();
        }
    }
}