using System.ComponentModel.DataAnnotations;
using Domain.Entities.Auth;

namespace Application.Auth.DTOs;

public class CreateAccountDTO : LoginDTO
{
    [Required,Compare(nameof(Password))]
    public string ConfirmPassword { get; set; } = string.Empty;
    [Required]
    public Role Role { get; set; }
}