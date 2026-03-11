using FoodDelivery.OrderService.Models;
using FoodDelivery.Shared.Configuration;

namespace FoodDelivery.OrderService.Builder;

// Director knows HOW to build specific order types using the builder.
// Client code doesn't need to know the construction steps.
public class OrderDirector
{
    private readonly OrderBuilder _builder;

    public OrderDirector(OrderBuilder builder)
    {
        _builder = builder;
    }

    // Standard order: default delivery fee from config
    public Order BuildStandardOrder(
        int customerId, int restaurantId,
        string address, string paymentMethod,
        List<(int id, string name, int qty, decimal price)> items)
    {
        var config = AppConfiguration.Instance;

        var b = _builder
            .ForCustomer(customerId)
            .FromRestaurant(restaurantId)
            .DeliverTo(address)
            .WithPayment(paymentMethod)
            .WithDeliveryFee(config.DefaultDeliveryFee);

        foreach (var (id, name, qty, price) in items)
            b.AddItem(id, name, qty, price);

        return b.Build();
    }

    // Express order: higher delivery fee, adds express note automatically
    public Order BuildExpressOrder(
        int customerId, int restaurantId,
        string address, string paymentMethod,
        List<(int id, string name, int qty, decimal price)> items)
    {
        var config = AppConfiguration.Instance;

        var b = _builder
            .ForCustomer(customerId)
            .FromRestaurant(restaurantId)
            .DeliverTo(address)
            .WithPayment(paymentMethod)
            .WithDeliveryFee(config.ExpressDeliveryFee)
            .WithNote("EXPRESS delivery requested — priority handling.");

        foreach (var (id, name, qty, price) in items)
            b.AddItem(id, name, qty, price);

        return b.Build();
    }
}
