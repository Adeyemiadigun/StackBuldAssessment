using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Models;
using Application.Common.Pagination;
using Application.Models;
using Domain.Enums;
using MediatR;

namespace Application.Queries.Orders.GetUserTransactions
{
    public record GetUserTransactionsQuery(
    PaymentStatus? Status, DateTime? FromDate,
    DateTime? ToDate, PaginationRequest Request
) : IRequest<DataResponse<PagedResult<OrderTransactionDto>>>;

}
