namespace EcomBackend.Domain.Payments;

public enum PaymentStatus
{
    Pending,
    Succeeded,
    Failed
}

public class Payment
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public long AmountCents { get; set; }
    public string Currency { get; set; } = default!;
    public string SessionId { get; set; } = default!;
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

    public Payment(int orderId, long amountCents, string currency, string sessionId)
    {
        OrderId = orderId;
        AmountCents = amountCents;
        Currency = currency;
        SessionId = sessionId;
    }
}
