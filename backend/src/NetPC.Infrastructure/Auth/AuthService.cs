using Microsoft.AspNetCore.Identity;
using NetPC.Application.Auth;
using NetPC.Application.DTOs.Auth;
using NetPC.Domain.Users;

namespace NetPC.Infrastructure.Auth;

/// <summary>
/// Service responsible for handling user authentication, including registration, login, and logout.
/// </summary>
public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly IJwtService _jwtService;
    private readonly IAuthRepository _repository;

    public AuthService(UserManager<User> userManager, IJwtService jwtService, IAuthRepository repository)
    {
        _userManager = userManager;
        _jwtService = jwtService;
        _repository = repository;
    }

    public async Task<AuthResult> RegisterAsync(RegisterDto dto)
    {
        var user = new User
        {
            UserName = dto.Username,
            Email = dto.Email,
            EmailConfirmed = true // All accounts are confirmed by default
        };

        // Create user
        var identityResult = await _userManager.CreateAsync(user, dto.Password);
        if (!identityResult.Succeeded)
        {
            var errors = string.Join(", ", identityResult.Errors.Select(e => e.Description));
            return AuthResult.Fail(errors);
        }

        // Generate JWT tokens
        var accessToken = _jwtService.GenerateAccessToken(user);
        var refreshToken = _jwtService.GenerateRefreshToken();
        refreshToken.AssignUser(user.Id);
        await _repository.AddRefreshTokenAsync(refreshToken);

        return AuthResult.Ok(accessToken, refreshToken.Token);
    }

    public async Task<AuthResult> LoginAsync(LoginDto dto)
    {
        // Login
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
            return AuthResult.Fail("Invalid email or password.");

        // Generate JWT tokens
        var accessToken = _jwtService.GenerateAccessToken(user);
        var refreshToken = _jwtService.GenerateRefreshToken();
        refreshToken.AssignUser(user.Id);
        await _repository.AddRefreshTokenAsync(refreshToken);

        return AuthResult.Ok(accessToken, refreshToken.Token);
    }

    public async Task<AuthResult> RefreshAsync(string refreshToken)
    {
        var storedToken = await _repository.GetRefreshTokenByValueAsync(refreshToken);
        if (storedToken is null || !storedToken.IsActive)
            return AuthResult.Fail("Invalid or expired refresh token.");

        var user = storedToken.User;

        // Remove old token
        await _repository.RemoveRefreshTokenAsync(storedToken);

        // Generate new tokens
        var newAccessToken = _jwtService.GenerateAccessToken(user);
        var newRefreshToken = _jwtService.GenerateRefreshToken();
        newRefreshToken.AssignUser(user.Id);
        await _repository.AddRefreshTokenAsync(newRefreshToken);

        return AuthResult.Ok(newAccessToken, newRefreshToken.Token);
    }

    public async Task<bool> LogoutAsync(Guid userId, string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            return false;

        var token = await _repository.GetRefreshTokenAsync(userId, refreshToken);
        if (token == null)
            return false;

        await _repository.RemoveRefreshTokenAsync(token);
        return true;
    }
}
