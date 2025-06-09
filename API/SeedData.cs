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

    public async Task SeedMechanic() 
    {
        var user = await _authService.FindUserByEmailAsync("mechanic@security.com");
        if (user != null) return;
        var mechanicDto = new CreateAccountDTO
        {
            Email = "mechanic@security.com",
            ConfirmPassword = "zaq1@WSXcv",
            Password = "zaq1@WSXcv",
            Role = Role.Mechanic,
        };
        await _mediator.Send(new CreateAccountCommand(mechanicDto));
    }

    public async Task SeedStoreManager()
    {
        var user = await _authService.FindUserByEmailAsync("storemanager@security.com");
        if (user != null) return;
        var storeManagerDto = new CreateAccountDTO
        {
            Email = "storemanager@security.com",
            ConfirmPassword = "zaq1@WSXcv",
            Password = "zaq1@WSXcv",
            Role = Role.StorageManager,
        };
        await _mediator.Send(new CreateAccountCommand(storeManagerDto));
    }
    public async Task SeedManager()
    {
        var user = await _authService.FindUserByEmailAsync("manager@security.com");
        if (user != null) return;
        var managerDto = new CreateAccountDTO
        {
            Email = "manager@security.com",
            ConfirmPassword = "zaq1@WSXcv",
            Password = "zaq1@WSXcv",
            Role = Role.Manager
        };
        await _mediator.Send(new CreateAccountCommand(managerDto));
    }
}