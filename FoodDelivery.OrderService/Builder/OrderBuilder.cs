using FoodDelivery.OrderService.Models;
using FoodDelivery.Shared.Configuration;

namespace FoodDelivery.OrderService.Builder;

public class OrderBuilder
{
    private readonly Order _order = new();
    private readonly List<OrderItem> _items = [];

    public OrderBuilder ForCustomer(int customerId)
    {
        _order.CustomerId = customerId;
        return this;
    }

    public OrderBuilder FromRestaurant(int restaurantId)
    {
        _order.RestaurantId = restaurantId;
        return this;
    }

    public OrderBuilder DeliverTo(string address)
    {
        _order.DeliveryAddress = address;
        return this;
    }

    public OrderBuilder WithPayment(string method)
    {
        _order.PaymentMethod = method;
        return this;
    }

    public OrderBuilder WithNote(string note)
    {
        _order.Notes = note;
        return this;
    }

    public OrderBuilder WithDeliveryFee(decimal fee)
    {
        _order.DeliveryFee = fee;
        return this;
    }

    public OrderBuilder AddItem(int menuItemId, string name, int quantity, decimal unitPrice)
    {
        _items.Add(new OrderItem
        {
            MenuItemId = menuItemId,
            ItemName   = name,
            Quantity   = quantity,
            UnitPrice  = unitPrice,
        });
        return this;
    }

    public Order Build()
    {
        var config = AppConfiguration.Instance;

        if (_order.CustomerId == 0)
            throw new InvalidOperationException("Customer is required.");

        if (_order.RestaurantId == 0)
            throw new InvalidOperationException("Restaurant is required.");

        if (string.IsNullOrWhiteSpace(_order.DeliveryAddress))
            throw new InvalidOperationException("Delivery address is required.");

        if (_items.Count == 0)
            throw new InvalidOperationException("Order must have at least one item.");

        if (_items.Count > config.MaxItemsPerOrder)
            throw new InvalidOperationException($"Order cannot exceed {config.MaxItemsPerOrder} items.");

        if (_order.DeliveryFee == 0)
            _order.DeliveryFee = config.DefaultDeliveryFee;

        _order.Items = _items;
        _order.Status = OrderStatus.Pending;

        return _order;
    }
}
