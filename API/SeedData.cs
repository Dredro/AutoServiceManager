using Application.Auth.Commands;
using Application.Auth.Contracts;
using Application.Auth.DTOs;
using Domain.Entities.Auth;
using MediatR;

namespace API;

public class SeedData
{
    private readonly IAuthService _authService;
    private readonly IMediator _mediator;
    public SeedData(IAuthService authService, IMediator mediator)
    {
        _authService = authService;
        _mediator = mediator;
    }

    public async Task SeedRole()
    {
        await _authService.CreateRoleAsync(Role.Admin);
        await _authService.CreateRoleAsync(Role.Manager);
        await _authService.CreateRoleAsync(Role.StorageManager);
        await _authService.CreateRoleAsync(Role.Mechanic);
    }

    public async Task SeedAdmin()
    {
        var user = await _authService.FindUserByEmailAsync("admin@security.com");
        if (user != null) return;
        var adminDto = new CreateAccountDTO
        {
            Email = "admin@security.com",
            ConfirmPassword = "zaq1@WSXcv",
            Password = "zaq1@WSXcv",
            Role = Role.Admin
        };
        await _mediator.Send(new CreateAccountCommand(adminDto));
    }
}