using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Exceptions;
using Application.Queries.Products;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Commands.Products.UpdateProductCommand
{
    public class UpdateProductHandler(IRepository<Product> _repo, ILogger<Product> logger,IUnitOfWork unitOfWork) : IRequestHandler<UpdateProductCommand, DataResponse<ProductDto>>
    {
        public async Task<DataResponse<ProductDto>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {

            logger.LogInformation("Updating Product with Id {0}", request.Id);

            var product = await _repo.GetByIdAsync(request.Id,cancellationToken) ?? throw new ApiException("Product Not Found",404,"ProductNotFound");

            var updateRequest = request.request;

            product.UpdateProduct(updateRequest.ProductName,updateRequest.Description, updateRequest.Price, updateRequest.StockQuantity);

           await _repo.UpdateAsync(product,cancellationToken);

            await unitOfWork.SaveChangesAsync(cancellationToken);
            return new DataResponse<ProductDto>
            {
                Data = product.ToProductDto(),
                Message = "Product Updated Successfully",
                Success = true
            };
            
        }
    }
}
