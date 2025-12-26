using MediatR;

namespace Application.Commands.Payment.Webhook;

/// <summary>
/// Command to handle Paystack webhook event
/// </summary>
public record HandlePaystackWebhookCommand(
    string EventType,
    string Reference,
    decimal Amount,
    string Status,
    DateTime? PaidAt
) : IRequest<WebhookResponse>;
