using Application.Common.Models;
using MediatR;

namespace Application.Queries.Products.GetProduct
{
    public record GetProductQuery(Guid Id): IRequest<DataResponse<ProductDto>>;
}
