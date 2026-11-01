namespace EcomBackend.Infrastructure.Config
{
    public class StripeSettings
    {
        public string PublishableKey { get; set; } = default!;
        public string SecretKey { get; set; } = default!;
        public string WebhookSecret { get; set; } = default!;
        public string FrontendBaseUrl { get; set; } = "http://localhost:3000";
    }

}
