using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Models;
using Application.Common.Pagination;
using Domain.Entities;
using MediatR;

namespace Application.Queries.Products.GetProductQuery
{
    public record GetProducts(PaginationRequest request)
    : IRequest<DataResponse<PagedResult<ProductDto>>>;

}
