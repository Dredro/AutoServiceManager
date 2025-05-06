namespace Domain.Entities.Auth;

public class RefreshToken
{
    public int Id { get; set; }
    public string? UserID { get; set; }
    public string? Token { get; set; }
    public DateTime Created { get; set; }
    public DateTime Expires { get; set; }
}