using Final_Grp6_PROG3340.Data;
using Final_Grp6_PROG3340.Models;
using Final_Grp6_PROG3340.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Google.Apis.Auth;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
	private readonly AppDbContext _db;
	private readonly PasswordService _passwords;
	private readonly JwtService _jwt;

	public AuthController(AppDbContext db, PasswordService passwords, JwtService jwt)
	{
		_db = db;
		_passwords = passwords;
		_jwt = jwt;
	}

	[HttpPost("register")]
	public async Task<IActionResult> Register(RegisterDto dto)
	{
		if (_db.Users.Any(u => u.Username == dto.Username))
			return BadRequest("Username already exists.");
		
		if (_db.Users.Any(u => u.Email == dto.Email))
			return BadRequest("Email already exists.");

		var user = new User
		{
			Name = dto.Name,
			Email = dto.Email,
			Username = dto.Username
		};

		user.PasswordHash = _passwords.Hash(user, dto.Password);

		_db.Users.Add(user);
		await _db.SaveChangesAsync();

		return Ok(new { message = "Registered successfully" });
	}

	[HttpPost("login")]
	public async Task<IActionResult> Login(LoginDto dto)
	{
		var user = _db.Users.FirstOrDefault(u => u.Username == dto.Username);
		if (user == null) return Unauthorized("Invalid username/password.");

		var ok = _passwords.Verify(user, dto.Password, user.PasswordHash);
		if (!ok) return Unauthorized("Invalid username/password.");

		var token = _jwt.Generate(user);

		return Ok(new { token });
	}

	[Authorize]
	[HttpGet("me")]
	public IActionResult Me()
	{
		return Ok(new
		{
			Id = User.FindFirstValue(ClaimTypes.NameIdentifier),
			Email = User.FindFirstValue(JwtRegisteredClaimNames.Email),
			Role = User.FindFirstValue(ClaimTypes.Role),
			Username = User.FindFirstValue("username")
		});
	}

	[HttpPost("google")]
	public async Task<IActionResult> GoogleLogin(GoogleLoginDto dto)
	{
		// Validate Google ID token
		var payload = await GoogleJsonWebSignature.ValidateAsync(dto.IdToken);

		// payload contains email, name, sub (Google user id)
		var email = payload.Email;

		// Check if the user already exists
		var user = _db.Users.FirstOrDefault(u => u.Email == email);

		if (user == null)
		{
			user = new User
			{
				Name = payload.Name,
				Email = payload.Email,
				Username = payload.Email,
				Role = "User",
				PasswordHash = "", 
				CreatedAt = DateTime.UtcNow
			};

			_db.Users.Add(user);
			await _db.SaveChangesAsync();
		}

		var jwt = _jwt.Generate(user);

		return Ok(new { token = jwt });
	}

}
