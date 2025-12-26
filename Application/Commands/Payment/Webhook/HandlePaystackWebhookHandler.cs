using Application.Common.Interfaces;
using Application.Exceptions;
using Application.Services;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Commands.Payment.Webhook;

/// <summary>
/// Handles Paystack webhook events
/// </summary>
public class HandlePaystackWebhookHandler : IRequestHandler<HandlePaystackWebhookCommand, WebhookResponse>
{
    private readonly IRepository<PaymentTransaction> _paymentRepository;
    private readonly IRepository<Order> _orderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<HandlePaystackWebhookHandler> _logger;

    public HandlePaystackWebhookHandler(
        IRepository<PaymentTransaction> paymentRepository,
        IRepository<Order> orderRepository,
        IUnitOfWork unitOfWork,
        ILogger<HandlePaystackWebhookHandler> logger)
    {
        _paymentRepository = paymentRepository;
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<WebhookResponse> Handle(
        HandlePaystackWebhookCommand request,
        CancellationToken ct)
    {
        _logger.LogInformation("Processing webhook event {EventType} for reference {Reference}",
            request.EventType, request.Reference);

        // Only handle payment success events
        if (request.EventType != "charge.success")
        {
            _logger.LogDebug("Ignoring event type {EventType}", request.EventType);
            return new WebhookResponse(true, "Event received but not processed");
        }

        // Find payment transaction by reference
        var payment = await _paymentRepository.FirstOrDefaultAsync(
            p => p.Reference == request.Reference, ct);

        if (payment == null)
        {
            _logger.LogWarning("Payment transaction not found for reference {Reference}", request.Reference);
            throw new ApiException("Payment transaction not found", 404, "PaymentNotFound");
        }

        // If already successful, no need to process again (idempotent)
        if (payment.Status == PaymentStatus.Successful)
        {
            _logger.LogInformation("Payment {Reference} already processed as successful", request.Reference);
            return new WebhookResponse(true, "Payment already processed");
        }

        // Get the order
        var order = await _orderRepository.GetByIdAsync(payment.OrderId, ct);
        if (order == null)
        {
            _logger.LogError("Order not found for payment {PaymentId}", payment.Id);
            throw new ApiException("Order not found", 404, "OrderNotFound");
        }

        // Verify amount matches
        var expectedAmountInKobo = (int)(payment.Amount * 100);
        var receivedAmountInKobo = (int)(request.Amount * 100);

        if (expectedAmountInKobo != receivedAmountInKobo)
        {
            _logger.LogError(
                "Amount mismatch for payment {Reference}. Expected: {Expected}, Received: {Received}",
                request.Reference, expectedAmountInKobo, receivedAmountInKobo);
            throw new ApiException("Amount mismatch", 400, "AmountMismatch");
        }

        // Update payment status
        if (request.Status == "success")
        {
            payment.MarkSuccessful();
            order.MarkAsPaid();

            await _unitOfWork.SaveChangesAsync(ct);

            _logger.LogInformation(
                "Payment {Reference} processed successfully. Order {OrderId} marked as paid",
                request.Reference, order.Id);

            return new WebhookResponse(true, "Payment processed successfully");
        }
        else
        {
            payment.MarkFailed();
            order.MarkAsFailed();

            await _unitOfWork.SaveChangesAsync(ct);

            _logger.LogWarning("Payment {Reference} failed", request.Reference);
            return new WebhookResponse(true, "Payment marked as failed");
        }
    }
}
