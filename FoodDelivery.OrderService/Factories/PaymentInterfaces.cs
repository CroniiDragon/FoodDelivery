namespace FoodDelivery.OrderService.Factories;

// ABSTRACT FACTORY — Payment component family
// Each payment needs 3 compatible components:
// Validator checks data before payment
// Processor executes the transaction
// Receipt generates proof of payment

public record PaymentResult(
    bool   Success,
    string TransactionId,
    string ErrorMessage = ""
);

public class PaymentData
{
    public decimal Amount        { get; set; }
    public string  PaymentMethod { get; set; } = string.Empty;
    // Card specific
    public string? CardNumber    { get; set; }
    public string? CardHolder    { get; set; }
    public string? ExpiryDate    { get; set; }
    public string? Cvv           { get; set; }
    // PayPal specific
    public string? PayPalEmail   { get; set; }
}

public interface IPaymentValidator
{
    /// <summary>
    /// Validates payment data:
    /// Card: Luhn algorithm + expiration date + CVV
    /// PayPal: valid email format
    /// Cash: always valid
    /// </summary>
    (bool IsValid, string ErrorMessage) Validate(PaymentData data);
}

public interface IPaymentProcessor
{
    Task<PaymentResult> ProcessAsync(PaymentData data);
}

public interface IPaymentReceipt
{
    string Generate(PaymentData data, PaymentResult result);
}
