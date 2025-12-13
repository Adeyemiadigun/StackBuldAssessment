using Application.Commands.Payment.InitailizePayment;
using Application.Commands.Payment.VerifyPayment;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/v1/payments")]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PaymentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Initiate payment for an order
    /// </summary>
    [HttpPost("initiate")]

    [Authorize(Roles =  "Customer")]
    public async Task<IActionResult> InitiatePayment(
        [FromBody] InitiatePaymentCommand request,
        CancellationToken ct)
    {
        var command = new InitiatePaymentCommand(
            request.OrderId,
            request.Email);

        var result = await _mediator.Send(command, ct);

        return Ok(result);
    }

    /// <summary>
    /// Verify payment (called after redirect or webhook)
    /// </summary>
    [HttpGet("verify")]

    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> VerifyPayment(
        [FromQuery] string reference,
        CancellationToken ct)
    {
        var result = await _mediator.Send(
            new VerifyPaymentCommand(reference),
            ct);

        return Ok(result);
    }
}
