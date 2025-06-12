using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Backend.Model;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;


namespace Backend.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AuthController : ControllerBase
	{
		[HttpPost("login")]
		public IActionResult Login([FromBody] Login login)
		{
			if (login.Username == "string" && login.Password == "string")
			{
				var token = GenerateJwtToken();
				return Ok(new { token });
			}
			return Unauthorized();
		}
		private string GenerateJwtToken()
		{
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your_secret_key_your_secret_key_your_secret_key"));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				issuer: "your_issuer",
				audience: "your_audience",
				claims: new List<Claim>(),
				expires: DateTime.Now.AddSeconds(25),
				signingCredentials: creds);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}
	}
}

