using FoodDelivery.OrderService.Builder;
using FoodDelivery.OrderService.DTOs;
using FoodDelivery.OrderService.Facade;
using FoodDelivery.OrderService.Interfaces;
using FoodDelivery.OrderService.Models;
using FoodDelivery.OrderService.Observer;
using FoodDelivery.OrderService.State;
using FoodDelivery.OrderService.Strategy;

namespace FoodDelivery.OrderService.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository    _repo;
    private readonly OrderStatusSubject  _subject;
    private readonly DeliveryContext     _delivery;

    public OrderService(IOrderRepository repo, OrderStatusSubject subject, DeliveryContext delivery)
    {
        _repo     = repo;
        _subject  = subject;
        _delivery = delivery;
    }

    public async Task<OrderResponseDto> CreateAsync(CreateOrderDto dto)
    {
        // Builder — constructs and validates the order step by step
        var builder = new OrderBuilder()
            .ForCustomer(dto.CustomerId)
            .FromRestaurant(dto.RestaurantId)
            .DeliverTo(dto.DeliveryAddress)
            .WithPayment(dto.PaymentMethod)
            .WithNote(dto.Notes ?? string.Empty);

        // Strategy — calculate delivery fee based on selected strategy
        var city = dto.DeliveryAddress.Split(',').LastOrDefault()?.Trim() ?? "Chișinău";
        var itemsTotal = dto.Items.Sum(i => i.UnitPrice * i.Quantity);
        var fee = _delivery.GetDeliveryFee(itemsTotal, city);
        builder.WithDeliveryFee(fee);

        foreach (var i in dto.Items)
            builder.AddItem(i.MenuItemId, i.ItemName, i.Quantity, i.UnitPrice);

        var order = builder.Build();
        var saved = await _repo.AddAsync(order);

        // Observer — notify all subscribers about new order
        await _subject.NotifyAsync(saved.Id, "Pending", saved.CustomerId, saved.CourierId);

        return MapToDto(saved);
    }

    public async Task<OrderResponseDto?> GetByIdAsync(int id)
    {
        var o = await _repo.GetWithItemsAsync(id);
        return o == null ? null : MapToDto(o);
    }

    public async Task<IEnumerable<OrderResponseDto>> GetByCustomerAsync(int customerId)
        => (await _repo.GetByCustomerAsync(customerId)).Select(MapToDto);

    public async Task UpdateStatusAsync(int orderId, string status)
    {
        var order = await _repo.GetByIdAsync(orderId);
        if (order == null) return;

        // State — transition through allowed states only
        var ctx = new OrderStateContext(orderId, $"Customer#{order.CustomerId}");
        ApplyStateTransition(ctx, status);

        if (Enum.TryParse<OrderStatus>(ctx.CurrentStatus, out var parsed))
        {
            order.Status = parsed;
            await _repo.UpdateAsync(order);

            // Observer — notify all subscribers about status change
            await _subject.NotifyAsync(orderId, ctx.CurrentStatus, order.CustomerId, order.CourierId);
        }
    }

    public async Task<bool> CancelAsync(int orderId)
    {
        var order = await _repo.GetByIdAsync(orderId);
        if (order == null) return false;
        if (order.Status is OrderStatus.OutForDelivery or OrderStatus.Delivered)
            return false;

        order.Status = OrderStatus.Cancelled;
        await _repo.UpdateAsync(order);

        // Observer — notify cancellation
        await _subject.NotifyAsync(orderId, "Cancelled", order.CustomerId, order.CourierId);
        return true;
    }

    private static void ApplyStateTransition(OrderStateContext ctx, string target)
    {
        switch (target)
        {
            case "Confirmed":      ctx.Confirm();            break;
            case "Preparing":      ctx.StartPreparing();     break;
            case "OutForDelivery": ctx.SendOutForDelivery(); break;
            case "Delivered":      ctx.MarkDelivered();      break;
            case "Cancelled":      ctx.Cancel();             break;
        }
    }

    private static OrderResponseDto MapToDto(Order o) => new()
    {
        Id              = o.Id,
        CustomerId      = o.CustomerId,
        RestaurantId    = o.RestaurantId,
        Status          = o.Status.ToString(),
        TotalPrice      = o.GetTotalPrice(),
        PaymentMethod   = o.PaymentMethod,
        DeliveryAddress = o.DeliveryAddress,
        CreatedAt       = o.CreatedAt,
        Notes           = o.Notes,
        Items           = o.Items.Select(i => new OrderItemDto
        {
            ItemName   = i.ItemName,
            Quantity   = i.Quantity,
            UnitPrice  = i.UnitPrice,
            TotalPrice = i.TotalPrice,
        }).ToList(),
    };
}
