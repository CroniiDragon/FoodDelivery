namespace FoodDelivery.OrderService.State;

// State interface — each state knows what transitions are allowed
public interface IOrderState
{
    string StatusName { get; }
    IOrderState Confirm(OrderStateContext ctx);
    IOrderState StartPreparing(OrderStateContext ctx);
    IOrderState SendOutForDelivery(OrderStateContext ctx);
    IOrderState MarkDelivered(OrderStateContext ctx);
    IOrderState Cancel(OrderStateContext ctx);
}

// Context: holds the current state
public class OrderStateContext
{
    public int    OrderId    { get; }
    public string CustomerName { get; }

    private IOrderState _state;

    public OrderStateContext(int orderId, string customerName)
    {
        OrderId      = orderId;
        CustomerName = customerName;
        _state       = new PendingState();
        Console.WriteLine($"[State] Order #{orderId} created → {_state.StatusName}");
    }

    public string CurrentStatus => _state.StatusName;

    public void Confirm()          => Transition(_state.Confirm(this));
    public void StartPreparing()   => Transition(_state.StartPreparing(this));
    public void SendOutForDelivery() => Transition(_state.SendOutForDelivery(this));
    public void MarkDelivered()    => Transition(_state.MarkDelivered(this));
    public void Cancel()           => Transition(_state.Cancel(this));

    private void Transition(IOrderState newState)
    {
        if (newState != _state)
        {
            Console.WriteLine($"[State] Order #{OrderId}: {_state.StatusName} → {newState.StatusName}");
            _state = newState;
        }
    }
}

// Base state: invalid transitions print a warning and stay in current state
public abstract class BaseOrderState : IOrderState
{
    public abstract string StatusName { get; }

    protected IOrderState Invalid(string action, IOrderState current)
    {
        Console.WriteLine($"[State] Cannot '{action}' from state '{StatusName}'.");
        return current;
    }

    public virtual IOrderState Confirm(OrderStateContext ctx)           => Invalid("Confirm", this);
    public virtual IOrderState StartPreparing(OrderStateContext ctx)    => Invalid("StartPreparing", this);
    public virtual IOrderState SendOutForDelivery(OrderStateContext ctx) => Invalid("SendOutForDelivery", this);
    public virtual IOrderState MarkDelivered(OrderStateContext ctx)     => Invalid("MarkDelivered", this);
    public virtual IOrderState Cancel(OrderStateContext ctx)            => Invalid("Cancel", this);
}

public class PendingState : BaseOrderState
{
    public override string StatusName => "Pending";
    public override IOrderState Confirm(OrderStateContext ctx) => new ConfirmedState();
    public override IOrderState Cancel(OrderStateContext ctx)  => new CancelledState();
}

public class ConfirmedState : BaseOrderState
{
    public override string StatusName => "Confirmed";
    public override IOrderState StartPreparing(OrderStateContext ctx) => new PreparingState();
    public override IOrderState Cancel(OrderStateContext ctx)         => new CancelledState();
}

public class PreparingState : BaseOrderState
{
    public override string StatusName => "Preparing";
    public override IOrderState SendOutForDelivery(OrderStateContext ctx) => new OutForDeliveryState();
}

public class OutForDeliveryState : BaseOrderState
{
    public override string StatusName => "OutForDelivery";
    public override IOrderState MarkDelivered(OrderStateContext ctx) => new DeliveredState();
}

public class DeliveredState : BaseOrderState
{
    public override string StatusName => "Delivered";
    // Terminal state — no transitions allowed
}

public class CancelledState : BaseOrderState
{
    public override string StatusName => "Cancelled";
    // Terminal state — no transitions allowed
}
