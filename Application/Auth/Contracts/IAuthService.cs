using Domain.Entities.Auth;
using Microsoft.AspNetCore.Identity;

namespace Application.Auth.Contracts;

public interface IAuthService
{
    Task<string?> GenerateToken(User user);
    public Task<User?> FindUserByEmailAsync(string email);
    public string? GetIdentityErrorMessages(IdentityResult result);
    public Task<(string message, bool isSuccess)> AssignUserToRole(User user, string roleName);
    public string GenerateRefreshToken();
    public Task<(string message, bool isSuccess)> SaveRefreshToken(string userId, string token);
    public Task CreateRoleAsync(Role role);
}