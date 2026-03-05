using FoodDelivery.Shared.Models;

namespace FoodDelivery.UserService.Models;

// OOP: Abstractizare + Mostenire
// OCP: Deschisa pentru extindere (Customer, Courier) inchisa pentru modificare
// SRP: Gestioneaza doar datele de baza ale unui utilizator
public abstract class User : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    // OOP: Polimorfism - fiecare subtip returneaza rolul sau
    public abstract string GetRole();

    // Encapsulare: logica de hashing ascunsa in clasa de baza
    public void SetPassword(string plainPassword)
    {
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(plainPassword);
    }

    public bool VerifyPassword(string plainPassword)
    {
        return BCrypt.Net.BCrypt.Verify(plainPassword, PasswordHash);
    }
}
