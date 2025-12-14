using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Models;
using Application.Common.Pagination;
using Application.Models;
using Application.Repositories;
using Application.Services;
using MediatR;

namespace Application.Queries.Orders.GetUserTransactions
{
    public class GetUserTransactionsHandler
     : IRequestHandler<GetUserTransactionsQuery, DataResponse<PagedResult<OrderTransactionDto>>>
    {
        private readonly IPaymentTransactionRepository _transactionRepo;
        private readonly ICurrentUser _currentUser;

        public GetUserTransactionsHandler(
            IPaymentTransactionRepository transactionRepo,ICurrentUser currentUser)
        {
            _transactionRepo = transactionRepo;
            _currentUser = currentUser; 
        }

        public async Task<DataResponse<PagedResult<OrderTransactionDto>>> Handle(
            GetUserTransactionsQuery request,
            CancellationToken ct)
        {
            var userId = await _currentUser.GetUserAsync();
            var paged = await _transactionRepo.GetByUserIdAsync(userId,
                request.Status,request.FromDate,request.ToDate,request.Request,
                ct);

            var mapped = paged.Items.Select(t => new OrderTransactionDto(
                t.Id,
                t.Amount,
                t.Status,
                t.Provider,
                t.Reference,
                t.CreatedAt
            )).ToList();

            return new DataResponse<PagedResult<OrderTransactionDto>>
            {
                Message = "Data Retrieved",
                Success = true,
                Data = new PagedResult<OrderTransactionDto>(
                mapped,
                paged.TotalCount,
                paged.TotalPages,
                paged.PageSize)
            };
        }
    }

}
