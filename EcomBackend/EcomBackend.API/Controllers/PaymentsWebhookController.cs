using EcomBackend.Application.Payments.Commands.ProcessPaymentWebhook;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EcomBackend.API.Controllers;

[ApiController]
[Route("api/payments")]
public class PaymentsWebhookController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<PaymentsWebhookController> _logger;

    public PaymentsWebhookController(IMediator mediator, ILogger<PaymentsWebhookController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost("webhook")]
    public async Task<IActionResult> Webhook(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stripe webhook endpoint hit.");

        using var reader = new StreamReader(Request.Body);
        var payload = await reader.ReadToEndAsync(cancellationToken);
        var signature = Request.Headers["Stripe-Signature"].ToString();

        if (string.IsNullOrEmpty(signature))
            _logger.LogWarning("Stripe webhook called without a Stripe-Signature header.");

        try
        {
            await _mediator.Send(
                new ProcessPaymentWebhookCommand(payload, signature),
                cancellationToken);

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Stripe webhook processing failed.");
            return BadRequest();
        }
    }
}
