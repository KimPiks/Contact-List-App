using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NetPC.Application.Auth;
using NetPC.Application.DTOs;
using NetPC.Application.DTOs.Auth;
using NetPC.Domain.Users;

namespace NetPC.Infrastructure.Auth;

/// <summary>
/// Service for authentication and authorization purpose
/// </summary>
public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly IJwtService _jwtService;
    private readonly AppDbContext _dbContext;

    public AuthService(UserManager<User> userManager, IJwtService jwtSettings, AppDbContext dbContext)
    {
        _userManager = userManager;
        _jwtService = jwtSettings;
        _dbContext = dbContext;
    }
    
    public async Task<AuthResult> RegisterAsync(RegisterDto dto)
    {
        // User model
        var user = new User()
        {
            UserName = dto.Username,
            Email = dto.Email,
            EmailConfirmed = true
        };
        
        // Register user
        var identityResult = await _userManager.CreateAsync(user, dto.Password);
        // Success
        if (identityResult.Succeeded)
        {
            var accessToken = _jwtService.GenerateAccessToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();
            refreshToken.AssignUser(user.Id);
            _dbContext.RefreshTokens.Add(refreshToken);
            await  _dbContext.SaveChangesAsync();
            
            return AuthResult.Ok(accessToken, refreshToken.Token);
        }
        
        // Error while registering
        var errors = identityResult.Errors.Select(e => e.Description);
        var errorsString = string.Join(", ", errors);
        return AuthResult.Fail(errorsString);
    }

    public async Task<AuthResult> LoginAsync(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
            return AuthResult.Fail("Invalid email or password.");

        var accessToken = _jwtService.GenerateAccessToken(user);
        var refreshToken = _jwtService.GenerateRefreshToken();
        refreshToken.AssignUser(user.Id);
        _dbContext.RefreshTokens.Add(refreshToken);
        await  _dbContext.SaveChangesAsync();
            
        return AuthResult.Ok(accessToken, refreshToken.Token);
    }

            public async Task<bool> LogoutAsync(Guid userId, string refreshToken)
            {
                if (string.IsNullOrWhiteSpace(refreshToken))
                    return false;

                var token = await _dbContext.RefreshTokens
                    .FirstOrDefaultAsync(rt => rt.UserId == userId && rt.Token == refreshToken);

                if (token == null)
                    return false;

                _dbContext.RefreshTokens.Remove(token);
                await _dbContext.SaveChangesAsync();

                return true;
            }
}