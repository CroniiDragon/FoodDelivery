// ═══════════════════════════════════════════════════════
// DTOs/RestaurantDtos.cs
// ═══════════════════════════════════════════════════════
namespace FoodDelivery.RestaurantService.DTOs;

public class CreateRestaurantDto
{
    public string Name        { get; set; } = string.Empty;
    public string Address     { get; set; } = string.Empty;
    public string City        { get; set; } = string.Empty;
    public string Cuisine     { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
}

public class CreateMenuItemDto
{
    public string  Name         { get; set; } = string.Empty;
    public string  Description  { get; set; } = string.Empty;
    public decimal BasePrice    { get; set; }
    public string  Category     { get; set; } = string.Empty;
    public string  ItemType     { get; set; } = "Food"; // "Food" sau "Drink"
    public int     RestaurantId { get; set; }

    // Campuri specifice FoodItem
    public int?  Calories     { get; set; }
    public bool? IsVegetarian { get; set; }
    public bool? IsVegan      { get; set; }

    // Campuri specifice DrinkItem
    public double? VolumeInLiters { get; set; }
    public bool?   IsAlcoholic    { get; set; }
}

public class RestaurantResponseDto
{
    public int    Id          { get; set; }
    public string Name        { get; set; } = string.Empty;
    public string Address     { get; set; } = string.Empty;
    public string City        { get; set; } = string.Empty;
    public string Cuisine     { get; set; } = string.Empty;
    public bool   IsOpen      { get; set; }
    public int    MenuItemCount { get; set; }
}

public class MenuItemResponseDto
{
    public int     Id          { get; set; }
    public string  Name        { get; set; } = string.Empty;
    public string  Description { get; set; } = string.Empty;
    public decimal FinalPrice  { get; set; }
    public string  ItemType    { get; set; } = string.Empty;
    public string  Category    { get; set; } = string.Empty;
    public bool    IsAvailable { get; set; }
    public int     RestaurantId { get; set; }
}
