namespace FoodDelivery.UserService.Models;
public class Customer : User
{
    public string DeliveryAddress { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;

    public override string GetRole() => "Customer";
}
