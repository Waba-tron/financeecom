namespace EcomBackend.Application.Payments
{
    public interface IPaymentGateway
    {
        Task<(string SessionId, string CheckoutUrl)> CreateCheckoutSessionAsync(
            int orderId,
            long amountCents,
            string currency,
            CancellationToken cancellationToken = default
        );

        PaymentWebhookEvent ParseWebhookEvent(string payload, string signatureHeader);
    }
}
