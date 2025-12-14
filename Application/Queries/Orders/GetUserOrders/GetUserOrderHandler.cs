using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Common.Pagination;
using Application.Models;
using Application.Services;
using Domain.Entities;
using MediatR;

namespace Application.Queries.Orders.GetUserOrders
{
    public class GetMyOrdersHandler
    : IRequestHandler<GetUserOrdersQuery, DataResponse<PagedResult<OrderListDto>>>
    {
        private readonly IRepository<Order> _orderRepo;
        private readonly ICurrentUser _currentUser;

        public GetMyOrdersHandler(IRepository<Order> orderRepo, ICurrentUser currentUser)
        {
            _orderRepo = orderRepo;
            _currentUser = currentUser;
        }

        public async Task<DataResponse<PagedResult<OrderListDto>>> Handle(
            GetUserOrdersQuery request,
            CancellationToken ct)
        {
            var userId = await _currentUser.GetUserAsync();
            var query = _orderRepo.Query()
                .Where(o => o.UserId == userId);

            if (request.Status.HasValue)
                query = query.Where(o => o.Status == request.Status);
            if (request.FromDate.HasValue)
                query = query.Where(o => o.CreatedAt >= request.FromDate.Value);

            if (request.ToDate.HasValue)
                query = query.Where(o => o.CreatedAt <= request.ToDate.Value);

            var projected = query
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new OrderListDto(
                    o.Id,
                    o.Status,
                    o.TotalAmount,
                    o.Items.Count,
                    o.CreatedAt));

            return new DataResponse<PagedResult<OrderListDto>>
            {
                Success = true,
                Message = "Order Retreived",
                Data = await projected.ToPagedResultAsync(
                request.PaginationRequest.page,
                request.PaginationRequest.pageSize)
            };
        }
    }

}
