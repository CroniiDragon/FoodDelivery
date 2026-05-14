using FoodDelivery.RestaurantService.Models;

namespace FoodDelivery.RestaurantService.Decorator;

// Component interface — both real MenuItems and decorators implement this
public interface IMenuItemPricing
{
    string Name         { get; }
    decimal GetPrice();
    string GetDescription();
}

// Concrete component: wraps a real MenuItem
public class MenuItemPricing : IMenuItemPricing
{
    private readonly MenuItem _item;

    public MenuItemPricing(MenuItem item) => _item = item;

    public string  Name           => _item.Name;
    public decimal GetPrice()     => _item.GetFinalPrice();
    public string  GetDescription() => $"{_item.Name} ({_item.GetItemType()})";
}

// Base decorator — holds a reference to another IMenuItemPricing
public abstract class MenuItemDecorator : IMenuItemPricing
{
    protected readonly IMenuItemPricing _inner;

    protected MenuItemDecorator(IMenuItemPricing inner) => _inner = inner;

    public virtual string  Name             => _inner.Name;
    public virtual decimal GetPrice()       => _inner.GetPrice();
    public virtual string  GetDescription() => _inner.GetDescription();
}

// Decorator 1: adds VAT (19%)
public class VatDecorator : MenuItemDecorator
{
    private const decimal VatRate = 0.19m;

    public VatDecorator(IMenuItemPricing inner) : base(inner) { }

    public override decimal GetPrice()       => _inner.GetPrice() * (1 + VatRate);
    public override string  GetDescription() => _inner.GetDescription() + " + TVA 19%";
}

// Decorator 2: adds packaging fee
public class PackagingDecorator : MenuItemDecorator
{
    private readonly decimal _packagingFee;

    public PackagingDecorator(IMenuItemPricing inner, decimal fee = 1.5m) : base(inner)
        => _packagingFee = fee;

    public override decimal GetPrice()       => _inner.GetPrice() + _packagingFee;
    public override string  GetDescription() => _inner.GetDescription() + $" + Ambalaj {_packagingFee:F2} RON";
}

// Decorator 3: applies a promotional discount
public class DiscountDecorator : MenuItemDecorator
{
    private readonly decimal _discountPercent;

    public DiscountDecorator(IMenuItemPricing inner, decimal discountPercent = 10m) : base(inner)
        => _discountPercent = discountPercent;

    public override decimal GetPrice()       => _inner.GetPrice() * (1 - _discountPercent / 100);
    public override string  GetDescription() => _inner.GetDescription() + $" -{_discountPercent}% discount";
}
