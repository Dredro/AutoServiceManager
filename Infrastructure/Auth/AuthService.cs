using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Application.Auth.Contracts;
using Domain.Entities.Auth;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Auth;

public class AuthService : IAuthService
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;
    private readonly SignInManager<User> _signInManager;
    private readonly AppDbContext _context;
    private readonly ILogger<AuthService> _logger;

    public AuthService(RoleManager<IdentityRole> roleManager, UserManager<User> userManager, IConfiguration configuration, SignInManager<User> signInManager, AppDbContext context, ILogger<AuthService> logger)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _configuration = configuration;
        _signInManager = signInManager;
        _context = context;
        _logger = logger;
    }
    public async Task CreateRoleAsync(Role role)
    {
        var existingRole = await FindRoleByNameAsync(role.ToString());
            if (existingRole != null)
                return;
                
            var result = await _roleManager.CreateAsync(new IdentityRole(role.ToString()));
            if (!result.Succeeded)
            {
                var errors = GetIdentityErrorMessages(result);
                return;
            }
    }
    public async Task<string?> GenerateToken(User user)
    {
        try
        {
            var jwtKey = _configuration["Jwt:Key"] 
                         ?? throw new InvalidOperationException("JWT key not configured");
                
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var userRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? string.Empty;
            
            var userClaims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Role, userRole),
            };
            
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: userClaims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: credentials
            );
            
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate JWT token for user {UserId}", user.Id);
            return null;
        }
    }
    public async Task<(string,bool)> SaveRefreshToken(string userId, string token)
    {
        try
        {
            var existingToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(t => t.UserID == userId);
                
            if (existingToken == null)
            {
                _context.RefreshTokens.Add(new RefreshToken 
                { 
                    UserID = userId, 
                    Token = token,
                    Created = DateTime.UtcNow,
                    Expires = DateTime.UtcNow.AddDays(7)
                });
            }
            else
            {
                existingToken.Token = token;
                existingToken.Created = DateTime.UtcNow;
                existingToken.Expires = DateTime.UtcNow.AddDays(7);
            }
            
            await _context.SaveChangesAsync();
            return ("Refresh token saved successfully",true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save refresh token for user {UserId}", userId);
            return ("Failed to save refresh token",false);
        }
    }
    public  string GenerateRefreshToken() 
        => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    private async Task<IdentityRole?> FindRoleByNameAsync(string roleName)
        => await _roleManager.FindByNameAsync(roleName);
    public async Task<(string, bool)> AssignUserToRole(User user, string roleName)
    {
        if (user is null) 
            return ("User cannot be null", false);
    
        if (string.IsNullOrEmpty(roleName)) 
            return ("Role name cannot be empty", false);
    
        var role = await FindRoleByNameAsync(roleName);
        if (role == null)
            return ("Role not found!", false);
    
        var result = await _userManager.AddToRoleAsync(user, roleName);
        if (!result.Succeeded)
        {
            var errors = string.Join(Environment.NewLine, result.Errors.Select(e => e.Description));
            return (errors, false);
        }
    
        return ($"{user.Email} assigned to {roleName} role", true);
    }

    public async Task<User?> FindUserByEmailAsync(string email)
        => await _userManager.FindByEmailAsync(email);
    public string? GetIdentityErrorMessages(IdentityResult result)
    {
        if (result.Succeeded) return null;
        return string.Join(Environment.NewLine, result.Errors.Select(e => e.Description));
    }
}