using MediatR;

namespace EcomBackend.Application.Payments.Commands.ProcessPaymentWebhook;

public record ProcessPaymentWebhookCommand(
    string Payload,
    string SignatureHeader
) : IRequest<Unit>;
