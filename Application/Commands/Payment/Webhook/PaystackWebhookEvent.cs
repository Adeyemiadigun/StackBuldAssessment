namespace Application.Commands.Payment.Webhook;

/// <summary>
/// Paystack webhook event payload
/// </summary>
public record PaystackWebhookEvent(
    string Event,
    PaystackWebhookData Data
);

/// <summary>
/// Paystack webhook data
/// </summary>
public record PaystackWebhookData(
    int Id,
    string Domain,
    DateTime CreatedAt,
    string Status,
    string Reference,
    decimal Amount,
    string Message,
    string GatewayResponse,
    DateTime? PaidAt,
    DateTime Created_At,
    string Channel,
    string Currency,
    string IpAddress,
    PaystackCustomer Customer
);

/// <summary>
/// Paystack customer information from webhook
/// </summary>
public record PaystackCustomer(
    int Id,
    string? FirstName,
    string? LastName,
    string Email
);

/// <summary>
/// Response returned to Paystack after webhook processing
/// </summary>
public record WebhookResponse(
    bool Success,
    string Message
);
