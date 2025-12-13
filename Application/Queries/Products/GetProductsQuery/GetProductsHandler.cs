using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Common.Pagination;
using Application.Queries.Products.GetProductQuery;
using Domain.Entities;
using MediatR;

namespace Application.Queries.Products.GetProductsQuery
{
    public class GetProductsHandler : IRequestHandler<GetProducts, DataResponse<PagedResult<ProductDto>>>
    {
        private readonly IRepository<Product> _repo;

        public GetProductsHandler(IRepository<Product> repo) => _repo = repo;

        public async Task<DataResponse<PagedResult<ProductDto>>> Handle(GetProducts request, CancellationToken cancellationToken)
        {
            var products = await _repo.Query()
                .OrderBy(x => x.Name)
                .ToPagedResultAsync(request.request.page, request.request.pageSize);

            var paged = new PagedResult<ProductDto>
            {
                Items = products.Items.Select(x => x.ToProductDto()).ToList(),
                PageNumber = products.PageNumber,
                PageSize = products.PageSize,
                TotalCount = products.TotalCount,
            };


            return new DataResponse<PagedResult<ProductDto>>
            {
                Data = paged,
                Message = "Products Retrieved Successfully",
                Success = true
            };
        }
    }

}
