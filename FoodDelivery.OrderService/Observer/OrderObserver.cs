namespace FoodDelivery.OrderService.Observer;

// Observer interface — all observers implement this
public interface IOrderObserver
{
    string ObserverName { get; }
    Task OnStatusChangedAsync(int orderId, string newStatus, int customerId, int? courierId);
}

// Subject interface
public interface IOrderSubject
{
    void Subscribe(IOrderObserver observer);
    void Unsubscribe(IOrderObserver observer);
    Task NotifyAsync(int orderId, string newStatus, int customerId, int? courierId);
}

// Observer 1: notifies the customer
public class CustomerNotificationObserver : IOrderObserver
{
    public string ObserverName => "CustomerNotifier";

    public async Task OnStatusChangedAsync(int orderId, string newStatus, int customerId, int? courierId)
    {
        var message = newStatus switch
        {
            "Confirmed"      => $"Order #{orderId} confirmed by restaurant!",
            "Preparing"      => $"Order #{orderId} is being prepared.",
            "OutForDelivery" => $"Order #{orderId} is on the way!",
            "Delivered"      => $"Order #{orderId} delivered. Enjoy!",
            "Cancelled"      => $"Order #{orderId} has been cancelled.",
            _                => $"Order #{orderId} status: {newStatus}"
        };

        Console.WriteLine($"[Observer→Customer #{customerId}] {message}");
        await Task.CompletedTask;
    }
}

// Observer 2: notifies the courier (only when relevant)
public class CourierNotificationObserver : IOrderObserver
{
    public string ObserverName => "CourierNotifier";

    public async Task OnStatusChangedAsync(int orderId, string newStatus, int customerId, int? courierId)
    {
        if (courierId == null) return;

        if (newStatus is "OutForDelivery" or "Delivered")
        {
            Console.WriteLine($"[Observer→Courier #{courierId}] Order #{orderId} → {newStatus}");
        }

        await Task.CompletedTask;
    }
}

// Observer 3: logs every status change
public class OrderAuditObserver : IOrderObserver
{
    public string ObserverName => "AuditLog";

    public async Task OnStatusChangedAsync(int orderId, string newStatus, int customerId, int? courierId)
    {
        Console.WriteLine($"[Audit] {DateTime.UtcNow:HH:mm:ss} — Order #{orderId} → {newStatus}");
        await Task.CompletedTask;
    }
}

// Subject: the order status manager
public class OrderStatusSubject : IOrderSubject
{
    private readonly List<IOrderObserver> _observers = [];

    public void Subscribe(IOrderObserver observer)   => _observers.Add(observer);
    public void Unsubscribe(IOrderObserver observer) => _observers.Remove(observer);

    public async Task NotifyAsync(int orderId, string newStatus, int customerId, int? courierId)
    {
        foreach (var observer in _observers)
            await observer.OnStatusChangedAsync(orderId, newStatus, customerId, courierId);
    }
}
