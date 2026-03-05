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
        var order = new Order
        {
            CustomerId      = dto.CustomerId,
            RestaurantId    = dto.RestaurantId,
            DeliveryAddress = dto.DeliveryAddress,
            PaymentMethod   = dto.PaymentMethod,
            Notes           = dto.Notes,
            Items = dto.Items.Select(i => new OrderItem
            {
                MenuItemId = i.MenuItemId,
                ItemName   = i.ItemName,
                Quantity   = i.Quantity,
                UnitPrice  = i.UnitPrice,
            }).ToList()
        };
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
        if (order.Status == OrderStatus.OutForDelivery || order.Status == OrderStatus.Delivered)
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
        Items           = o.Items.Select(i => new OrderItemDto
        {
            ItemName   = i.ItemName,
            Quantity   = i.Quantity,
            UnitPrice  = i.UnitPrice,
            TotalPrice = i.TotalPrice,
        }).ToList()
    };
}
