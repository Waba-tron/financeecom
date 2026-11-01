using EcomBackend.Application.Payments;
using EcomBackend.Infrastructure.Config;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace EcomBackend.Infrastructure.Payments
{
    public class StripePaymentGateway : IPaymentGateway
    {
        private readonly StripeSettings _settings;

        public StripePaymentGateway(IOptions<StripeSettings> settings)
        {
            _settings = settings.Value;
            StripeConfiguration.ApiKey = _settings.SecretKey;
        }

        public async Task<(string SessionId, string CheckoutUrl)> CreateCheckoutSessionAsync(
            int orderId,
            long amountCents,
            string currency,
            CancellationToken cancellationToken = default)
        {
            var options = new SessionCreateOptions
            {
                Mode = "payment",
                SuccessUrl = $"{_settings.FrontendBaseUrl}/checkout/success?orderId={orderId}",
                CancelUrl = $"{_settings.FrontendBaseUrl}/checkout",
                Metadata = new Dictionary<string, string>
                {
                    { "orderId", orderId.ToString() }
                },
                LineItems = new List<SessionLineItemOptions>
                {
                    new()
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = currency,
                            UnitAmount = amountCents,
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = $"Order #{orderId}"
                            }
                        },
                        Quantity = 1
                    }
                }
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options, cancellationToken: cancellationToken);

            return (session.Id, session.Url!);
        }

        public PaymentWebhookEvent ParseWebhookEvent(string payload, string signatureHeader)
        {
            var stripeEvent = EventUtility.ConstructEvent(
                payload,
                signatureHeader,
                _settings.WebhookSecret,
                throwOnApiVersionMismatch: false
            );

            return stripeEvent.Type switch
            {
                EventTypes.CheckoutSessionCompleted =>
                    new PaymentWebhookEvent(
                        PaymentWebhookEventType.CheckoutCompleted,
                        (stripeEvent.Data.Object as Session)?.Id),

                EventTypes.CheckoutSessionExpired =>
                    new PaymentWebhookEvent(
                        PaymentWebhookEventType.CheckoutExpired,
                        (stripeEvent.Data.Object as Session)?.Id),

                EventTypes.CheckoutSessionAsyncPaymentFailed =>
                    new PaymentWebhookEvent(
                        PaymentWebhookEventType.PaymentFailed,
                        (stripeEvent.Data.Object as Session)?.Id),

                _ => new PaymentWebhookEvent(PaymentWebhookEventType.Unknown, null)
            };
        }
    }
}
