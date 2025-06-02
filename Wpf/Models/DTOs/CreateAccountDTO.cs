using System.ComponentModel.DataAnnotations;

namespace Wpf.Models.DTOs;
public class CreateAccountDTO : LoginDTO
{
    [Required,Compare(nameof(Password))]
    public string ConfirmPassword { get; set; } = string.Empty;
    [Required]
    public Role Role { get; set; }
}