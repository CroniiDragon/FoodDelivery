using FoodDelivery.Shared.Models;

namespace FoodDelivery.RestaurantService.Models;

// ── Restaurant ────────────────────────────────────────────────────
// SRP: Gestioneaza doar datele unui restaurant
public class Restaurant : BaseEntity
{
    public string Name        { get; set; } = string.Empty;
    public string Address     { get; set; } = string.Empty;
    public string City        { get; set; } = string.Empty;
    public string Cuisine     { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public bool   IsOpen      { get; set; } = true;

    // Relatia One-to-Many cu EF Core
    public ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
}

// ── MenuItem (clasa de baza abstracta) ───────────────────────────
// OCP: Deschisa pentru extindere (FoodItem, DrinkItem) inchisa pentru modificare
// OOP: Abstractizare - defineste contractul comun al produselor din meniu
public abstract class MenuItem : BaseEntity
{
    public string  Name        { get; set; } = string.Empty;
    public string  Description { get; set; } = string.Empty;
    public decimal BasePrice   { get; set; }
    public bool    IsAvailable { get; set; } = true;
    public string  Category    { get; set; } = string.Empty;

    // Foreign key catre Restaurant
    public int        RestaurantId { get; set; }
    public Restaurant Restaurant  { get; set; } = null!;

    // OOP: Polimorfism - fiecare subtip calculeaza pretul diferit
    public abstract decimal GetFinalPrice();

    // OOP: Polimorfism - fiecare subtip returneaza tipul sau
    public abstract string GetItemType();
}

// ── FoodItem ──────────────────────────────────────────────────────
// LSP: FoodItem poate inlocui MenuItem fara probleme
// SRP: Gestioneaza doar datele specifice unui produs alimentar
public class FoodItem : MenuItem
{
    public int  Calories      { get; set; }
    public bool IsVegetarian  { get; set; } = false;
    public bool IsVegan       { get; set; } = false;
    public int  PreparationTimeMinutes { get; set; } = 15;

    // OOP: Polimorfism - pretul alimentelor nu are taxa extra
    public override decimal GetFinalPrice() => BasePrice;
    public override string  GetItemType()   => "Food";
}

// ── DrinkItem ─────────────────────────────────────────────────────
// LSP: DrinkItem poate inlocui MenuItem fara probleme
// SRP: Gestioneaza doar datele specifice unei bauturi
public class DrinkItem : MenuItem
{
    public double VolumeInLiters { get; set; }
    public bool   IsAlcoholic    { get; set; } = false;
    public bool   IsCold         { get; set; } = true;

    // OOP: Polimorfism - bauturile alcoolice au taxa de 10%
    public override decimal GetFinalPrice()
        => IsAlcoholic ? BasePrice * 1.10m : BasePrice;

    public override string GetItemType() => "Drink";
}
