namespace FoodDelivery.UserService.Models;

// LSP: Courier poate inlocui User fara probleme
// SRP: Gestioneaza doar datele specifice unui curier
public class Courier : User
{
    public string VehicleType { get; set; } = string.Empty;
    public bool IsAvailable { get; set; } = true;
    public double Rating { get; set; } = 5.0;
    public int TotalDeliveries { get; set; } = 0;

    // OOP: Polimorfism - suprascriem metoda abstracta
    public override string GetRole() => "Courier";
}
