using FoodDelivery.RestaurantService.Models;

namespace FoodDelivery.RestaurantService.Prototype;

// Prototype interface — all cloneable entities implement this
public interface IPrototype<T>
{
    T ShallowClone();
    T DeepClone();
}

// Prototype for Restaurant
// ShallowClone → new restaurant (same cuisine/city), empty menu
// DeepClone    → new restaurant with full menu copied (seasonal variant)
public class RestaurantPrototype : IPrototype<Restaurant>
{
    private readonly Restaurant _source;

    public RestaurantPrototype(Restaurant source)
    {
        _source = source;
    }

    // Shallow: copies restaurant data only, no menu items
    // Use case: open a new branch (same chain, new location)
    public Restaurant ShallowClone()
    {
        return new Restaurant
        {
            Name        = _source.Name + " (Branch)",
            Address     = string.Empty,
            City        = _source.City,
            Cuisine     = _source.Cuisine,
            PhoneNumber = string.Empty,
            IsOpen      = false,
            MenuItems   = new List<MenuItem>(),
        };
    }

    // Deep: copies restaurant + all menu items
    // Use case: create seasonal menu variant (summer menu from spring menu)
    public Restaurant DeepClone()
    {
        var clone = new Restaurant
        {
            Name        = _source.Name + " (Seasonal Copy)",
            Address     = _source.Address,
            City        = _source.City,
            Cuisine     = _source.Cuisine,
            PhoneNumber = _source.PhoneNumber,
            IsOpen      = false,
            MenuItems   = new List<MenuItem>(),
        };

        foreach (var item in _source.MenuItems)
        {
            MenuItem clonedItem = item switch
            {
                FoodItem f => new FoodItem
                {
                    Name                   = f.Name,
                    Description            = f.Description,
                    BasePrice              = f.BasePrice,
                    Category               = f.Category,
                    IsAvailable            = f.IsAvailable,
                    Calories               = f.Calories,
                    IsVegetarian           = f.IsVegetarian,
                    IsVegan                = f.IsVegan,
                    PreparationTimeMinutes = f.PreparationTimeMinutes,
                },
                DrinkItem d => new DrinkItem
                {
                    Name           = d.Name,
                    Description    = d.Description,
                    BasePrice      = d.BasePrice,
                    Category       = d.Category,
                    IsAvailable    = d.IsAvailable,
                    VolumeInLiters = d.VolumeInLiters,
                    IsAlcoholic    = d.IsAlcoholic,
                    IsCold         = d.IsCold,
                },
                _ => throw new InvalidOperationException($"Unknown menu item type: {item.GetType().Name}")
            };

            clone.MenuItems.Add(clonedItem);
        }

        return clone;
    }
}
