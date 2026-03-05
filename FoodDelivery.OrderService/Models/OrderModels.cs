using FoodDelivery.Shared.Models;

namespace FoodDelivery.OrderService.Models;

public class Order : BaseEntity
{
    public int    CustomerId      { get; set; }
    public int    RestaurantId    { get; set; }
    public string DeliveryAddress { get; set; } = string.Empty;
    public string PaymentMethod   { get; set; } = string.Empty;
    public OrderStatus Status     { get; set; } = OrderStatus.Pending;
    public decimal DeliveryFee    { get; set; } = 10m;
    public int?   CourierId       { get; set; }
    public string? Notes          { get; set; }

    // relation One-to-Many
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();

    public decimal GetTotalPrice() => Items.Sum(i => i.TotalPrice) + DeliveryFee;
}

public class OrderItem : BaseEntity
{
    public int     OrderId    { get; set; }
    public Order   Order      { get; set; } = null!;
    public int     MenuItemId { get; set; }
    public string  ItemName   { get; set; } = string.Empty;
    public int     Quantity   { get; set; }
    public decimal UnitPrice  { get; set; }
    public decimal TotalPrice => UnitPrice * Quantity;
}

public enum OrderStatus
{
    Pending,        // Inregistrata
    Confirmed,      // Confirmata de restaurant
    Preparing,      // In preparare
    OutForDelivery, // La curier
    Delivered,      // Livrata
    Cancelled       // Anulata
}
