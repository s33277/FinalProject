namespace RevenueRecognitionSystem.Data.Entities.Authentication;

public class User
{
    public int Id { get; set; }
    public string Login { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string Role { get; set; } = null!;
}
