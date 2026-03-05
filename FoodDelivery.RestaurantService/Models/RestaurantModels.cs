using FoodDelivery.Shared.Models;

namespace FoodDelivery.RestaurantService.Models;

public class Restaurant : BaseEntity
{
    public string Name        { get; set; } = string.Empty;
    public string Address     { get; set; } = string.Empty;
    public string City        { get; set; } = string.Empty;
    public string Cuisine     { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public bool   IsOpen      { get; set; } = true;

    // Relation One-to-Many
    public ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
}

public abstract class MenuItem : BaseEntity
{
    public string  Name        { get; set; } = string.Empty;
    public string  Description { get; set; } = string.Empty;
    public decimal BasePrice   { get; set; }
    public bool    IsAvailable { get; set; } = true;
    public string  Category    { get; set; } = string.Empty;

    public int        RestaurantId { get; set; }
    public Restaurant Restaurant  { get; set; } = null!;

    public abstract decimal GetFinalPrice();

    public abstract string GetItemType();
}

public class FoodItem : MenuItem
{
    public int  Calories      { get; set; }
    public bool IsVegetarian  { get; set; } = false;
    public bool IsVegan       { get; set; } = false;
    public int  PreparationTimeMinutes { get; set; } = 15;

    public override decimal GetFinalPrice() => BasePrice;
    public override string  GetItemType()   => "Food";
}

public class DrinkItem : MenuItem
{
    public double VolumeInLiters { get; set; }
    public bool   IsAlcoholic    { get; set; } = false;
    public bool   IsCold         { get; set; } = true;

    public override decimal GetFinalPrice()
        => IsAlcoholic ? BasePrice * 1.10m : BasePrice;

    public override string GetItemType() => "Drink";
}
