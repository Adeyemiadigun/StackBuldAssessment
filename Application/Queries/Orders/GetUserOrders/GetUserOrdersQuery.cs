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

namespace Application.Queries.Orders.GetUserOrders
{
    public record GetUserOrdersQuery(
    OrderStatus? Status, DateTime? FromDate,
    DateTime? ToDate, PaginationRequest PaginationRequest
) : IRequest<DataResponse<PagedResult<OrderListDto>>>;

}
