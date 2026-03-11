namespace FoodDelivery.UserService.Models;

public class Courier : User
{
    public string VehicleType { get; set; } = string.Empty;
    public bool IsAvailable { get; set; } = true;
    public double Rating { get; set; } = 5.0;
    public int TotalDeliveries { get; set; } = 0;

    public override string GetRole() => "Courier";
}
