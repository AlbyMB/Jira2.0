using Final_Grp6_PROG3340.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Final_Grp6_PROG3340.Services
{
	public class JwtService
	{
		private readonly IConfiguration _config;

		public JwtService(IConfiguration config) => _config = config;

		public string Generate(User user)
		{
			var securityKey = new SymmetricSecurityKey(
				Encoding.UTF8.GetBytes(_config["Jwt:Key"])
			);

			var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

			var claims = new[]
			{
			new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
			new Claim(JwtRegisteredClaimNames.Email, user.Email),
			new Claim("username", user.Username),
			new Claim(ClaimTypes.Role, user.Role)
		};

			var token = new JwtSecurityToken(
				issuer: _config["Jwt:Issuer"],
				audience: _config["Jwt:Audience"],
				claims: claims,
				expires: DateTime.UtcNow.AddDays(int.Parse(_config["Jwt:ExpiryDays"])),
				signingCredentials: credentials
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}
	}
}

