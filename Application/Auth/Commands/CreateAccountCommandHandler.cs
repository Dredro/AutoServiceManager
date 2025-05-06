using Application.Auth.Contracts;
using Application.Auth.DTOs;
using Domain.Entities.Auth;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Auth.Commands;

public record CreateAccountCommand(
    CreateAccountDTO model
    ) : IRequest<AccountResultDTO>;

public class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand,AccountResultDTO>
{
    private readonly IAuthService _authService;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;
    private readonly IAppDbContext _context;

    public CreateAccountCommandHandler(IAuthService authService, RoleManager<IdentityRole> roleManager, UserManager<User> userManager, IConfiguration configuration, IAppDbContext context)
    {
        _authService = authService;
        _roleManager = roleManager;
        _userManager = userManager;
        _configuration = configuration;
        _context = context;
    }

    public async Task<AccountResultDTO> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _authService.FindUserByEmailAsync(request.model.Email);
        if (existingUser != null)
            throw new ArgumentException("User with this email already exists");
        
        var user = new User
        {
            UserName = request.model.Email,
            Email = request.model.Email
        };
        
        var result = await _userManager.CreateAsync(user, request.model.Password);
        var errorMessage = _authService.GetIdentityErrorMessages(result);
        if (errorMessage != null)
            throw new ArgumentException(errorMessage);
        
        var roleResult = await _authService.AssignUserToRole(user, request.model.Role.ToString());
        if (!roleResult.isSuccess)
            throw new ArgumentException(roleResult.message);
        
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