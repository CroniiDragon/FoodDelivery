namespace FoodDelivery.Shared.Configuration;

public sealed class AppConfiguration
{
    private static AppConfiguration? _instance;
    private static readonly object _lock = new();

    public decimal DefaultDeliveryFee { get; set; } = 10m;
    public decimal ExpressDeliveryFee { get; set; } = 25m;
    public int MaxItemsPerOrder { get; set; } = 20;

    private AppConfiguration() { }

    public static AppConfiguration Instance
    {
        get
        {
            if (_instance is null)
                lock (_lock) { _instance ??= new AppConfiguration(); }
            return _instance;
        }
    }
}