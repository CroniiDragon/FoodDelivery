namespace FoodDelivery.OrderService.Strategy;

// Strategy interface — all delivery fee algorithms implement this
public interface IDeliveryStrategy
{
    string Name { get; }
    decimal Calculate(decimal orderTotal, string city);
}

// Strategy 1: Standard delivery — flat fee
public class StandardDeliveryStrategy : IDeliveryStrategy
{
    public string Name => "Standard";

    public decimal Calculate(decimal orderTotal, string city)
    {
        // Free delivery for orders over 100 RON
        return orderTotal >= 100m ? 0m : 10m;
    }
}

// Strategy 2: Express delivery — higher fee, fixed
public class ExpressDeliveryStrategy : IDeliveryStrategy
{
    public string Name => "Express";

    public decimal Calculate(decimal orderTotal, string city)
    {
        // Express is always 25 RON regardless of order total
        return 25m;
    }
}

// Strategy 3: City-based delivery — fee depends on city
public class CityDeliveryStrategy : IDeliveryStrategy
{
    public string Name => "CityBased";

    private static readonly Dictionary<string, decimal> CityFees = new()
    {
        { "Chișinău", 10m },
        { "Bălți",    15m },
        { "Cahul",    20m },
        { "Ungheni",  18m },
    };

    public decimal Calculate(decimal orderTotal, string city)
        => CityFees.TryGetValue(city, out var fee) ? fee : 25m;
}

// Context: uses whichever strategy is set
public class DeliveryContext
{
    private IDeliveryStrategy _strategy;

    public DeliveryContext(IDeliveryStrategy strategy) => _strategy = strategy;

    // Strategy can be swapped at runtime
    public void SetStrategy(IDeliveryStrategy strategy) => _strategy = strategy;

    public decimal GetDeliveryFee(decimal orderTotal, string city)
    {
        var fee = _strategy.Calculate(orderTotal, city);
        Console.WriteLine($"[Strategy:{_strategy.Name}] City={city}, Total={orderTotal:F2} → Fee={fee:F2} RON");
        return fee;
    }
}
