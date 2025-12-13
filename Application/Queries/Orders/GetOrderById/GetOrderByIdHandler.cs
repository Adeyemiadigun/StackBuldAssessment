using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Exceptions;
using Application.Models;
using Application.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.Queries.Orders.GetOrderById
{
    public class GetOrderByIdHandler
    : IRequestHandler<GetOrderByIdQuery, DataResponse<OrderDetailsDto>>
    {
        private readonly IOrderRepository _orderRepo;

        public GetOrderByIdHandler(IOrderRepository orderRepo)
        {
            _orderRepo = orderRepo;
        }

        public async Task<DataResponse<OrderDetailsDto>> Handle(
            GetOrderByIdQuery request,
            CancellationToken ct)
        {
            var order = await _orderRepo.GetOrderWithDetailsAsync(request.OrderId,ct);

            if (order is null)
                throw new ApiException("Order not found", 404, "OrderNotFound");


            return new DataResponse<OrderDetailsDto>
            {
                Data = new OrderDetailsDto(
                order.Id,
                order.Status,
                order.TotalAmount,
                order.CreatedAt,
                order.Items.Select(i => new OrderItemDto(
                    i.ProductId,
                    i.Quantity,
                    i.UnitPrice,
                    i.Quantity * i.UnitPrice
                )).ToList(),
                order.Transactions.Select(t => new OrderTransactionDto(
                    t.Id,
                    t.Amount,
                    t.Status,
                    t.Provider,
                    t.Reference,
                    t.CreatedAt
                )).ToList() ?? []
            ),
                Success = true,
                Message = $"Order Retreived {request.OrderId} "
            };
        }
    }

}
