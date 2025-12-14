using Domain.Enums;
using System.Data;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Application.Queries.Orders.GetUserOrders;
using Application.Common.Models;
using Application.Queries.Orders.GetOrderById;
using Application.Queries.Orders.GetAllOrders;
using Application.Queries.Orders.GetOrderTransaction;

namespace Api.Controllers.V1
{

    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize(Roles = nameof(UserRole.Customer))]
        [HttpGet]
        public async Task<IActionResult> GetMyOrders(
            [FromQuery] OrderStatus? status,
            [FromQuery] PaginationRequest request)
        {

            var result = await _mediator.Send(
                new GetUserOrdersQuery(status, request));

            return Ok(result);
        }
        [Authorize]
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetOrderById(Guid id)
        {

            var result = await _mediator.Send(
                new GetOrderByIdQuery(id));

            return Ok(result);
        }

        [Authorize(Roles = nameof(UserRole.Admin))]
        [HttpGet("admin/orders")]
        public async Task<IActionResult> GetAllOrders(
        [FromQuery] OrderStatus? status,
        [FromQuery] PaginationRequest request)
        {
            var result = await _mediator.Send(
                new GetAllOrdersQuery(status,request));

            return Ok(result);
        }

        [Authorize]
        [HttpGet("{orderId:guid}/transactions")]
        public async Task<IActionResult> GetOrderTransactions(Guid orderId)
        {

            var result = await _mediator.Send(
                new GetOrderTransactionsQuery(orderId));

            return Ok(result);
        }



    }
}
