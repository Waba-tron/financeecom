namespace EcomBackend.Application.Payments;

public enum PaymentWebhookEventType
{
    Unknown,
    CheckoutCompleted,
    CheckoutExpired,
    PaymentFailed
}

public record PaymentWebhookEvent(
    PaymentWebhookEventType Type,
    string? SessionId
);
