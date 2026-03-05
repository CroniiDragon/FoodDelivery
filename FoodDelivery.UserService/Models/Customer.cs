namespace FoodDelivery.UserService.Models;

// LSP: Customer poate inlocui User fara probleme
// SRP: Gestioneaza doar datele specifice unui client
public class Customer : User
{
    public string DeliveryAddress { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;

    // OOP: Polimorfism - suprascriem metoda abstracta
    public override string GetRole() => "Customer";
}
