using Microsoft.AspNetCore.Mvc;
using UserService.Application.Services;  // UserService'i kullanıyoruz
using UserService.Domain.Entities;     // User modelini kullanıyoruz
using Microsoft.AspNetCore.Identity.Data;
using UserService.Application.Interfaces;

namespace UserService.API.Controllers
{
    [Route("api/users/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJwtService _jwtService;

        public UserController(IUserService userService, IJwtService jwtService)
        {
            _userService = userService;
            _jwtService = jwtService;
        }

        // Kullanıcı kaydı (Sign Up)
        [HttpPost("signup")]
        public async Task<ActionResult> SignUp([FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest("User data is invalid.");
            }

            await _userService.CreateUserAsync(user);
            return CreatedAtAction(nameof(GetUserByIdAsync), new { id = user.UserId }, user);
        }

        // Kullanıcı silme (Mantıksal silme)
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            await _userService.DeleteUserAsync(id);
            return NoContent();  // 204 No Content döndürüyoruz, çünkü veritabanını güncelledik
        }

        // Kullanıcıyı ID ile almak (isteğe bağlı)
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUserByIdAsync(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);


            if (user == null)
            {
                return NotFound("User not found.");
            }

            return Ok(user);
        }
    }
}
