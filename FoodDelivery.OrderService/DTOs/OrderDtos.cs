namespace FoodDelivery.OrderService.DTOs;

public class CreateOrderDto
{
    public int    CustomerId      { get; set; }
    public int    RestaurantId    { get; set; }
    public string DeliveryAddress { get; set; } = string.Empty;
    public string PaymentMethod   { get; set; } = string.Empty;
    public string? Notes          { get; set; }
    public List<CreateOrderItemDto> Items { get; set; } = new();
}

public class CreateOrderItemDto
{
    public int     MenuItemId { get; set; }
    public string  ItemName   { get; set; } = string.Empty;
    public int     Quantity   { get; set; }
    public decimal UnitPrice  { get; set; }
}

public class OrderResponseDto
{
    public int     Id             { get; set; }
    public int     CustomerId     { get; set; }
    public int     RestaurantId   { get; set; }
    public string  Status         { get; set; } = string.Empty;
    public decimal TotalPrice     { get; set; }
    public string  PaymentMethod  { get; set; } = string.Empty;
    public string  DeliveryAddress{ get; set; } = string.Empty;
    public DateTime CreatedAt     { get; set; }
    public List<OrderItemDto> Items { get; set; } = new();
}

public class OrderItemDto
{
    public string  ItemName   { get; set; } = string.Empty;
    public int     Quantity   { get; set; }
    public decimal UnitPrice  { get; set; }
    public decimal TotalPrice { get; set; }
}
