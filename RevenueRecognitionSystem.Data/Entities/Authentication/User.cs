using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace RevenueRecognitionSystem.Data.Entities;

public class User
{
    public int Id { get; set; }
    public string Login { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string Role { get; set; } = null!;
}