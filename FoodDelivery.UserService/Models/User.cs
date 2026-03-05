using FoodDelivery.Shared.Models;

namespace FoodDelivery.UserService.Models;

public abstract class User : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public abstract string GetRole();

    public void SetPassword(string plainPassword)
    {
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(plainPassword);
    }

    public bool VerifyPassword(string plainPassword)
    {
        return BCrypt.Net.BCrypt.Verify(plainPassword, PasswordHash);
    }
}
