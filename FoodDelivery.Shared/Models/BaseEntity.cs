namespace FoodDelivery.Shared.Models;

// Clasa de baza pentru TOATE entitatile din toate microserviciile
// OOP: Abstractizare - defineste contractul comun
// SRP: O singura responsabilitate - identitatea entitatii
public abstract class BaseEntity
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; } = false;
}
