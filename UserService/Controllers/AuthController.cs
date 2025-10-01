using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using UserService.Application.Interfaces;
using UserService.Application.Services;
using UserService.Infrastructure.Data;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IJwtService _jwtService;
    private readonly IRefreshTokenService _refreshService;
    private readonly IUserService _userService;
    private readonly AppDbContext _db;

    public AuthController(IJwtService jwtService, IRefreshTokenService refreshService,IUserService userService ,AppDbContext db)
    {
        _jwtService = jwtService;
        _refreshService = refreshService;
        _db = db;
        _userService = userService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<object>> LogIn([FromBody] LoginRequest loginRequest)
    {
        if (loginRequest == null)
        {
            return BadRequest("Login data is invalid.");
        }

        var user = await _userService.AuthenticateAsync(loginRequest.Email, loginRequest.Password);
        if (user == null)
        {
            return Unauthorized("Invalid email or password.");
        }

        // Access Token üret
        var accessToken = _jwtService.GenerateJwtToken(user);

        // Refresh Token üret
        var refreshToken = _refreshService.GenerateRefreshToken(user.UserId);

        // Kullanıcıya döndür
        return Ok(new
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        });
    }


    [HttpPost("refresh")]
    public IActionResult Refresh(string refreshToken)
    {
        var userId = _refreshService.ValidateRefreshToken(refreshToken);
        if (userId == null) return Unauthorized();

        var user = _db.Users.Find(userId.Value);
        var newAccessToken = _jwtService.GenerateJwtToken(user);

        return Ok(new { accessToken = newAccessToken });
    }

    [HttpPost("logout")]
    public IActionResult Logout(string refreshToken)
    {
        _refreshService.RevokeRefreshToken(refreshToken);
        return Ok("Logged out successfully");
    }
}
