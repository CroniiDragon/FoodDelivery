namespace FoodDelivery.OrderService.Factories;

//  ABSTRACT FACTORY
public interface IPaymentFactory
{
    string PaymentMethod { get; }
    IPaymentValidator CreateValidator();
    IPaymentProcessor CreateProcessor();
    IPaymentReceipt   CreateReceipt();
}


//CARD

/// <summary>
/// Validates a bank card using the Luhn algorithm (ISO/IEC 7812)
/// Checks if the card number is mathematically valid
/// </summary>
public class CardValidator : IPaymentValidator
{
    public (bool IsValid, string ErrorMessage) Validate(PaymentData data)
    {
        if (string.IsNullOrWhiteSpace(data.CardNumber))
            return (false, "Numarul cardului este obligatoriu.");

        // Curatam numarul — eliminam spatii si cratime
        var number = data.CardNumber.Replace(" ", "").Replace("-", "");

        if (number.Length < 13 || number.Length > 19)
            return (false, "Numarul cardului trebuie sa aiba intre 13 si 19 cifre.");

        if (!number.All(char.IsDigit))
            return (false, "Numarul cardului trebuie sa contina doar cifre.");

        // Luhn Algorithm: iterate digits right to left
        // Double digits in even positions (from the right)
        // Subtract 9 if doubled value > 9
        // Total sum must be a multiple of 10
        var sum         = 0;
        var isSecondPos = false;

        for (int i = number.Length - 1; i >= 0; i--)
        {
            var digit = number[i] - '0';

            if (isSecondPos)
            {
                digit *= 2;
                if (digit > 9) digit -= 9;
            }

            sum         += digit;
            isSecondPos  = !isSecondPos;
        }

        if (sum % 10 != 0)
            return (false, "Numarul cardului este invalid (esuat algoritmul Luhn).");

        if (string.IsNullOrWhiteSpace(data.ExpiryDate))
            return (false, "Data expirarii este obligatorie.");

        var parts = data.ExpiryDate.Split('/');
        if (parts.Length != 2
            || !int.TryParse(parts[0], out var month)
            || !int.TryParse(parts[1], out var year)
            || month < 1 || month > 12)
            return (false, "Data expirarii invalida. Format: MM/YY");

        var expiry = new DateTime(2000 + year, month, 1).AddMonths(1).AddDays(-1);
        if (expiry < DateTime.UtcNow)
            return (false, "Cardul a expirat.");

        // Verify CVV
        if (string.IsNullOrWhiteSpace(data.Cvv) || data.Cvv.Length < 3 || !data.Cvv.All(char.IsDigit))
            return (false, "CVV invalid (3-4 cifre numerice).");

        return (true, string.Empty);
    }
}

public class CardProcessor : IPaymentProcessor
{
    public async Task<PaymentResult> ProcessAsync(PaymentData data)
    {
        // Simulation
        await Task.Delay(100); // latency

        var transactionId = $"TXN-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}";

        // Simulation 95% succes
        var random = new Random();
        if (random.Next(100) < 5)
            return new PaymentResult(false, string.Empty, "Tranzactie refuzata de banca emitenta.");

        return new PaymentResult(true, transactionId);
    }
}

public class CardReceipt : IPaymentReceipt
{
    public string Generate(PaymentData data, PaymentResult result)
    {
        var number  = data.CardNumber?.Replace(" ", "").Replace("-", "") ?? "";
        var masked  = number.Length >= 4 ? "****" + number[^4..] : "****";
        var holder  = data.CardHolder ?? "Titular necunoscut";
        var dateStr = DateTime.UtcNow.ToString("dd.MM.yyyy HH:mm");

        return $"""
            ═══════════════════════════════════════
            CHITANTA PLATA CARD — FoodDelivery
            ═══════════════════════════════════════
            Data:          {dateStr}
            Card:          {masked}
            Titular:       {holder}
            Suma:          {data.Amount:F2} RON
            Status:        {(result.Success ? "APROBAT ✓" : "REFUZAT ✗")}
            ID Tranzactie: {result.TransactionId}
            ═══════════════════════════════════════
            """;
    }
}

// ── PAYPAL ────────────────────────────────────────────────────────

public class PayPalValidator : IPaymentValidator
{
    public (bool IsValid, string ErrorMessage) Validate(PaymentData data)
    {
        if (string.IsNullOrWhiteSpace(data.PayPalEmail))
            return (false, "Emailul PayPal este obligatoriu.");

        // Validare format email 
        var email = data.PayPalEmail.Trim();
        var atIdx = email.IndexOf('@');

        if (atIdx <= 0 || atIdx == email.Length - 1)
            return (false, "Format email PayPal invalid.");

        var domain = email[(atIdx + 1)..];
        if (!domain.Contains('.') || domain.StartsWith('.') || domain.EndsWith('.'))
            return (false, "Domeniu email PayPal invalid.");

        if (data.Amount <= 0)
            return (false, "Suma platii trebuie sa fie pozitiva.");

        if (data.Amount > 10000)
            return (false, "Suma depaseste limita PayPal de 10.000 MDL per tranzactie.");

        return (true, string.Empty);
    }
}

public class PayPalProcessor : IPaymentProcessor
{
    public async Task<PaymentResult> ProcessAsync(PaymentData data)
    {
        // Simultation redirect PayPal + confirmation
        await Task.Delay(150);

        var transactionId = $"PP-{Guid.NewGuid().ToString()[..12].ToUpper()}";
        return new PaymentResult(true, transactionId);
    }
}

public class PayPalReceipt : IPaymentReceipt
{
    public string Generate(PaymentData data, PaymentResult result)
    {
        var dateStr = DateTime.UtcNow.ToString("dd.MM.yyyy HH:mm");
        var email   = data.PayPalEmail ?? "";
        // Mascam partial emailul: user@domain.com → u***@domain.com
        var atIdx   = email.IndexOf('@');
        var masked  = atIdx > 1
            ? email[0] + new string('*', atIdx - 1) + email[atIdx..]
            : email;

        return $"""
            ═══════════════════════════════════════
            CHITANTA PLATA PAYPAL — FoodDelivery
            ═══════════════════════════════════════
            Data:          {dateStr}
            Cont PayPal:   {masked}
            Suma:          {data.Amount:F2} RON
            Status:        {(result.Success ? "CONFIRMAT ✓" : "ESUAT ✗")}
            ID Tranzactie: {result.TransactionId}
            Link confirmare: paypal.com/activity/{result.TransactionId}
            ═══════════════════════════════════════
            """;
    }
}

// ── CASH ──────────────────────────────────────────────────────────

public class CashValidator : IPaymentValidator
{
    public (bool IsValid, string ErrorMessage) Validate(PaymentData data)
    {
        if (data.Amount <= 0)
            return (false, "Suma platii trebuie sa fie pozitiva.");

        return (true, string.Empty);
    }
}

public class CashProcessor : IPaymentProcessor
{
    public async Task<PaymentResult> ProcessAsync(PaymentData data)
    {
        await Task.CompletedTask;
        var receiptNumber = $"CASH-{DateTime.UtcNow:yyyyMMddHHmmss}";
        return new PaymentResult(true, receiptNumber);
    }
}

public class CashReceipt : IPaymentReceipt
{
    public string Generate(PaymentData data, PaymentResult result)
    {
        var dateStr = DateTime.UtcNow.ToString("dd.MM.yyyy HH:mm");
        return $"""
            ═══════════════════════════════════════
            CHITANTA PLATA NUMERAR — FoodDelivery
            ═══════════════════════════════════════
            Data:          {dateStr}
            Metoda:        Numerar la livrare
            Suma totala:   {data.Amount:F2} RON
            Status:        DE ACHITAT LA LIVRARE
            Nr. chitanta:  {result.TransactionId}
            ATENTIE: Curierii nu dau rest pentru sume
            mai mari de 200 RON fara preaviz.
            ═══════════════════════════════════════
            """;
    }
}

public class CardPaymentFactory : IPaymentFactory
{
    public string PaymentMethod  => "Card";
    public IPaymentValidator CreateValidator() => new CardValidator();
    public IPaymentProcessor CreateProcessor() => new CardProcessor();
    public IPaymentReceipt   CreateReceipt()   => new CardReceipt();
}

public class PayPalPaymentFactory : IPaymentFactory
{
    public string PaymentMethod  => "PayPal";
    public IPaymentValidator CreateValidator() => new PayPalValidator();
    public IPaymentProcessor CreateProcessor() => new PayPalProcessor();
    public IPaymentReceipt   CreateReceipt()   => new PayPalReceipt();
}

public class CashPaymentFactory : IPaymentFactory
{
    public string PaymentMethod  => "Numerar";
    public IPaymentValidator CreateValidator() => new CashValidator();
    public IPaymentProcessor CreateProcessor() => new CashProcessor();
    public IPaymentReceipt   CreateReceipt()   => new CashReceipt();
}