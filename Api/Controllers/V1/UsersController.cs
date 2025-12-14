using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Application.Common.Models;
using Application.Queries.Orders.GetUserTransactions;

namespace Api.Controllers.V1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [Authorize(Roles = nameof(UserRole.Customer))]
        [HttpGet("transactions")]
        public async Task<IActionResult> GetMyTransactions(
        [FromQuery] PaymentStatus? status,
        [FromQuery] DateTime? fromDate,
         [FromQuery] DateTime? toDate,
        [FromQuery] PaginationRequest request)
        {

            var result = await _mediator.Send(
                new GetUserTransactionsQuery( status,fromDate,toDate, request));

            return Ok(result);
        }

    }
}
