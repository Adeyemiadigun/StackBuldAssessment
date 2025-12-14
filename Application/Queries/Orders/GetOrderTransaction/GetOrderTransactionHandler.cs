using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Exceptions;
using Application.Models;
using Application.Repositories;
using Application.Services;
using Domain.Entities;
using MediatR;

namespace Application.Queries.Orders.GetOrderTransaction
{
    
    public class GetOrderTransactionsHandler
    : IRequestHandler<GetOrderTransactionsQuery, DataResponse<IReadOnlyList<OrderTransactionDto>>>
    {
        private readonly IPaymentTransactionRepository _transactionRepo;
        private readonly IOrderRepository _orderRepo;
        private readonly ICurrentUser _currentUser;


        public GetOrderTransactionsHandler(
            IPaymentTransactionRepository transactionRepo,
            IOrderRepository orderRepo, ICurrentUser currentUser)
        {
            _transactionRepo = transactionRepo;
            _orderRepo = orderRepo;
            _currentUser = currentUser;
        }

        public async Task<DataResponse<IReadOnlyList<OrderTransactionDto>>> Handle(
            GetOrderTransactionsQuery request,
            CancellationToken ct)
        {
            var IsAuthenticated =  _currentUser.IsAuthenticated();

            var order = await _orderRepo.GetByIdAsync(request.OrderId, ct)
                ?? throw new ApiException("Order not found", 404, "OrderNotFound");

            if (!IsAuthenticated)
                throw new ApiException("Forbidden", 403, "Forbidden");

            var transactions = await _transactionRepo
            .GetByOrderIdAsync(request.OrderId, ct);

            return new DataResponse<IReadOnlyList<OrderTransactionDto>>
            {
                Message = "Data Retrieved",
                Success = true,
                Data = transactions.Select(t => new OrderTransactionDto(
                t.Id,
                t.Amount,
                t.Status,
                t.Provider,
                t.Reference,
                t.CreatedAt
            )).ToList()
            };
        }
    }

}
