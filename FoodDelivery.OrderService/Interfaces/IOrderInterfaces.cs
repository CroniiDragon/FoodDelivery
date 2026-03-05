// ════════════════ Interfaces/IOrderInterfaces.cs ══════════
using FoodDelivery.Shared.Interfaces;
using FoodDelivery.OrderService.DTOs;
using FoodDelivery.OrderService.Models;

namespace FoodDelivery.OrderService.Interfaces;

public interface IOrderRepository : IBaseRepository<Order>
{
    Task<IEnumerable<Order>> GetByCustomerAsync(int customerId);
    Task<IEnumerable<Order>> GetByRestaurantAsync(int restaurantId);
    Task<Order?> GetWithItemsAsync(int orderId);
}

// DIP: Controller-ul depinde de aceasta interfata
public interface IOrderService
{
    Task<OrderResponseDto> CreateAsync(CreateOrderDto dto);
    Task<OrderResponseDto?> GetByIdAsync(int id);
    Task<IEnumerable<OrderResponseDto>> GetByCustomerAsync(int customerId);
    Task UpdateStatusAsync(int orderId, string status);
    Task<bool> CancelAsync(int orderId);
}
