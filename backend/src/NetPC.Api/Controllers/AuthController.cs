using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetPC.Application.Auth;
using NetPC.Application.DTOs;
using NetPC.Application.DTOs.Auth;

namespace NetPC.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
	private readonly IAuthService _authService;

	public AuthController(IAuthService authService)
	{
		_authService = authService;
	}

	[HttpPost("register")]
	public async Task<ActionResult<AuthResult>> Register([FromBody] RegisterDto dto)
	{
		var result = await _authService.RegisterAsync(dto);
		if (!result.Success)
			return BadRequest(result);

		return Ok(result);
	}

	[HttpPost("login")]
	public async Task<ActionResult<AuthResult>> Login([FromBody] LoginDto dto)
	{
		var result = await _authService.LoginAsync(dto);
		if (!result.Success)
			return Unauthorized(result);

		return Ok(result);
	}

	[Authorize]
	[HttpPost("logout")]
	public async Task<IActionResult> Logout([FromBody] LogoutDto dto)
	{
		if (string.IsNullOrWhiteSpace(dto.RefreshToken))
			return BadRequest("Refresh token is required.");

		var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier)
						  ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

		if (!Guid.TryParse(userIdValue, out var userId))
			return Unauthorized();

		var success = await _authService.LogoutAsync(userId, dto.RefreshToken);
		if (!success)
			return Unauthorized();

		return NoContent();
	}
}