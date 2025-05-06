namespace Application.Auth.DTOs;

public record AccountResultDTO()
{
    public string Token { get; set; }
    public string RefreshToken { get; set; }
}