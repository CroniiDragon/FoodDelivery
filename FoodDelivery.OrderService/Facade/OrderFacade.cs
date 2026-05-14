using FoodDelivery.OrderService.DTOs;
using FoodDelivery.OrderService.Interfaces;

namespace FoodDelivery.OrderService.Facade;

// Facade: single method PlaceOrderAsync() hides the 3-step process:
//   1. Create order (OrderService)
//   2. Process payment (simple simulation)
//   3. Send notification (HTTP call to NotificationService — fire and forget)
public class OrderFacade
{
    private readonly IOrderService  _orderService;
    private readonly IHttpClientFactory _httpFactory;

    public OrderFacade(IOrderService orderService, IHttpClientFactory httpFactory)
    {
        _orderService = orderService;
        _httpFactory  = httpFactory;
    }

    public async Task<OrderFacadeResult> PlaceOrderAsync(CreateOrderDto dto)
    {
        // Step 1: create and save the order
        var order = await _orderService.CreateAsync(dto);

        // Step 2: process payment (simplified)
        var paymentOk = SimulatePayment(order.PaymentMethod, order.TotalPrice);
        if (!paymentOk)
        {
            await _orderService.CancelAsync(order.Id);
            return new OrderFacadeResult(false, order, "Payment failed.");
        }

        // Step 3: notify customer via HTTP (fire and forget — don't fail order if notification fails)
        _ = SendNotificationAsync(order.CustomerId, $"Order #{order.Id} confirmed! Total: {order.TotalPrice:F2} RON.");

        return new OrderFacadeResult(true, order, "Order placed successfully.");
    }

    private async Task SendNotificationAsync(int recipientId, string message)
    {
        try
        {
            var client = _httpFactory.CreateClient("NotificationService");
            var payload = new
            {
                recipientId,
                recipientType = "Customer",
                channel       = "Push",
                message,
            };
            await client.PostAsJsonAsync("/api/notifications", payload);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Facade] Notification failed (non-critical): {ex.Message}");
        }
    }

    private static bool SimulatePayment(string method, decimal amount)
    {
        Console.WriteLine($"[Payment] Processing {method} — {amount:F2} RON...");
        return true;
    }
}

public record OrderFacadeResult(bool Success, OrderResponseDto Order, string Message);
