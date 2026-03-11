namespace FoodDelivery.OrderService.Factories;

/// <summary>
/// Resolver: selecteaza fabrica de plata potrivita pe baza metodei de plata.
/// Primeste toate fabricile prin DI — OCP respectat.
/// </summary>
public class PaymentFactoryResolver
{
    private readonly IEnumerable<IPaymentFactory> _factories;

    public PaymentFactoryResolver(IEnumerable<IPaymentFactory> factories)
        => _factories = factories;

    public IPaymentFactory Resolve(string paymentMethod)
    {
        var factory = _factories.FirstOrDefault(f =>
            f.PaymentMethod.Equals(paymentMethod, StringComparison.OrdinalIgnoreCase));

        // Fallback
        return factory ?? _factories.First(f => f.PaymentMethod == "Numerar");
    }

    public IEnumerable<string> GetAvailableMethods()
        => _factories.Select(f => f.PaymentMethod);
}
