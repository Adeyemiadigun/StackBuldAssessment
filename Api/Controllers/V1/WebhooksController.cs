using System.Security.Cryptography;
using System.Text;
using Application.Commands.Payment.Webhook;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Infrastructure.Services.Payment.Paystack;

namespace Api.Controllers.V1;

/// <summary>
/// Handles webhook callbacks from payment providers
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/webhooks")]
[ApiVersion("1.0")]
public class WebhooksController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly PaystackOptions _paystackOptions;
    private readonly ILogger<WebhooksController> _logger;

    public WebhooksController(
        IMediator mediator,
        IOptions<PaystackOptions> paystackOptions,
        ILogger<WebhooksController> logger)
    {
        _mediator = mediator;
        _paystackOptions = paystackOptions.Value;
        _logger = logger;
    }

    /// <summary>
    /// Receives webhook events from Paystack
    /// </summary>
    /// <param name="payload">Webhook event payload</param>
    /// <param name="signature">Paystack signature from x-paystack-signature header</param>
    /// <returns>200 OK if webhook was processed successfully</returns>
    [HttpPost("paystack")]
    [AllowAnonymous]
    public async Task<IActionResult> PaystackWebhook(
        [FromBody] PaystackWebhookEvent payload,
        [FromHeader(Name = "x-paystack-signature")] string signature)
    {
        _logger.LogInformation("Received Paystack webhook: {EventType} for reference {Reference}",
            payload.Event, payload.Data?.Reference);

        // Verify signature
        if (!VerifyPaystackSignature(payload, signature))
        {
            _logger.LogWarning("Invalid Paystack signature for webhook event");
            return Unauthorized(new { message = "Invalid signature" });
        }

        try
        {
            // Process the webhook
            var result = await _mediator.Send(new HandlePaystackWebhookCommand(
                EventType: payload.Event,
                Reference: payload.Data!.Reference,
                Amount: payload.Data.Amount / 100m, // Convert from kobo to naira
                Status: payload.Data.Status,
                PaidAt: payload.Data.PaidAt
            ));

            _logger.LogInformation("Webhook processed successfully: {Message}", result.Message);
            return Ok(new { message = result.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing Paystack webhook for reference {Reference}",
                payload.Data?.Reference);

            // Return 200 to Paystack even on error to avoid retries
            // (we've already logged the error for manual review)
            return Ok(new { message = "Webhook received" });
        }
    }

    /// <summary>
    /// Verifies that the webhook payload matches the signature from Paystack
    /// </summary>
    private bool VerifyPaystackSignature(PaystackWebhookEvent payload, string signature)
    {
        if (string.IsNullOrEmpty(signature) || string.IsNullOrEmpty(_paystackOptions.SecretKey))
        {
            return false;
        }

        try
        {
            // Convert the payload to JSON and compute HMAC
            var json = System.Text.Json.JsonSerializer.Serialize(payload);
            var keyBytes = Encoding.UTF8.GetBytes(_paystackOptions.SecretKey);
            var payloadBytes = Encoding.UTF8.GetBytes(json);

            using var hmac = new HMACSHA512(keyBytes);
            var computedHash = hmac.ComputeHash(payloadBytes);
            var computedSignature = BitConverter.ToString(computedHash).Replace("-", "").ToLowerInvariant();

            // Constant-time comparison to prevent timing attacks
            return CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(computedSignature),
                Encoding.UTF8.GetBytes(signature)
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying Paystack signature");
            return false;
        }
    }
}
