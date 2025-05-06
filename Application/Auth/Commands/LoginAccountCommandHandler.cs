using Application.Auth.Contracts;
using Application.Auth.DTOs;
using Application.Exceptions;
using Domain.Entities.Auth;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Application.Auth.Commands;

public record LoginAccountCommand(
    LoginDTO model) : IRequest<AccountResultDTO>;
public class LoginAccountCommandHandler : IRequestHandler<LoginAccountCommand, AccountResultDTO>
{
    private readonly IAuthService _authService;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;
    private readonly IAppDbContext _context;
    private readonly SignInManager<User> _signInManager;

    public LoginAccountCommandHandler(IAuthService authService, RoleManager<IdentityRole> roleManager, UserManager<User> userManager, IConfiguration configuration, IAppDbContext context, SignInManager<User> signInManager)
    {
        _authService = authService;
        _roleManager = roleManager;
        _userManager = userManager;
        _configuration = configuration;
        _context = context;
        _signInManager = signInManager;
    }

    public async Task<AccountResultDTO> Handle(LoginAccountCommand request, CancellationToken cancellationToken)
    {
        var user = await _authService.FindUserByEmailAsync(request.model.Email);
        if (user is null)
            throw new NotFoundException("User not found");
            
        var result = await _signInManager.CheckPasswordSignInAsync(user, request.model.Password, false);
        if (!result.Succeeded)
            throw new ArgumentException("Invalid credentials");
            
        var jwtToken = await _authService.GenerateToken(user);
        if (string.IsNullOrEmpty(jwtToken))
            throw new Exception("Failed to generate authentication token");
                
        var refreshToken = _authService.GenerateRefreshToken();
        var saveResult = await _authService.SaveRefreshToken(user.Id, refreshToken);
            
        if (!saveResult.isSuccess)
            throw new Exception(saveResult.message);
        var dto = new AccountResultDTO
        {
            Token = jwtToken,
            RefreshToken = refreshToken
        };
        return dto;
    }
}