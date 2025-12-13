using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Exceptions;
using Domain.Entities;
using MediatR;

namespace Application.Queries.Products.GetProduct
{
    public class GetProductByIdHandler : IRequestHandler<GetProductQuery, DataResponse<ProductDto>>
    {
        private readonly IRepository<Product> _repo;

        public GetProductByIdHandler(IRepository<Product> repo)
        {
            _repo = repo;
        }
        public async Task<DataResponse<ProductDto>> Handle(GetProductQuery request, CancellationToken cancellationToken)
        {
            var product = await _repo.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new ApiException(
                "Product not found",
                404,
                "ProductNotFound");

            return new DataResponse<ProductDto>
            {
               Data = product.ToProductDto(),
               Success = true,
               Message = "Product retrieved Successfully"
            };
        }
    }
}

