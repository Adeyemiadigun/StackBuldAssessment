using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Common.Pagination;
using Application.Models;
using Domain.Entities;
using MediatR;

namespace Application.Queries.Orders.GetAllOrders
{
    public class GetAllOrdersHandler
    : IRequestHandler<GetAllOrdersQuery, DataResponse<PagedResult<OrderListDto>>>
    {
        private readonly IRepository<Order> _orderRepo;

        public GetAllOrdersHandler(IRepository<Order> orderRepo)
        {
            _orderRepo = orderRepo;
        }

        public async Task<DataResponse<PagedResult<OrderListDto>>> Handle(
     GetAllOrdersQuery request,
     CancellationToken ct)
        {
            var query = _orderRepo.Query();

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
                Data = await projected.ToPagedResultAsync(
                    request.Request.page,
                    request.Request.pageSize),
                Success = true,
                Message = "Orders retrieved successfully"
            };
        }
    }

}
