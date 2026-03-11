using FoodDelivery.OrderService.Builder;
using FoodDelivery.OrderService.DTOs;
using FoodDelivery.OrderService.Interfaces;
using FoodDelivery.OrderService.Models;

namespace FoodDelivery.OrderService.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _repo;

    public OrderService(IOrderRepository repo) => _repo = repo;

    public async Task<OrderResponseDto> CreateAsync(CreateOrderDto dto)
    {
        var builder  = new OrderBuilder();
        var director = new OrderDirector(builder);

        var items = dto.Items
            .Select(i => (i.MenuItemId, i.ItemName, i.Quantity, i.UnitPrice))
            .ToList();

        // Director decides construction steps based on order type
        var order = dto.IsExpress
            ? director.BuildExpressOrder(dto.CustomerId, dto.RestaurantId, dto.DeliveryAddress, dto.PaymentMethod, items)
            : director.BuildStandardOrder(dto.CustomerId, dto.RestaurantId, dto.DeliveryAddress, dto.PaymentMethod, items);

        if (!string.IsNullOrWhiteSpace(dto.Notes) && !dto.IsExpress)
            order.Notes = dto.Notes;

        var saved = await _repo.AddAsync(order);
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
        if (Enum.TryParse<OrderStatus>(status, out var parsed))
        {
            order.Status = parsed;
            await _repo.UpdateAsync(order);
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
        return true;
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
