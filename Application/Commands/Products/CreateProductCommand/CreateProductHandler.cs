using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Commands.Products.CreateProductCommand
{
    public class CreateProductHandler(IRepository<Product> repo, IUnitOfWork unitOfWork,ILogger<CreateProductHandler> logger) : IRequestHandler<CreateProductCommand, DataResponse<Guid>>
    {

        public async Task<DataResponse<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("About to create Product");

            var res = await repo.ExistsAsync(x => x.Name == request.product.Name);
            if(res)
            {
                return new DataResponse<Guid>
                {
                    Success = false,
                    Message = "Product Exists Already"
                };
                

            }

            var product = new Product(
                request.product.Name,
                request.product.Description,
                request.product.Price,
                request.product.StockQuantity);

            await repo.AddAsync(product);
            await unitOfWork.SaveChangesAsync();
            logger.LogInformation("Product Created Successfully");

            return new DataResponse<Guid>
            {
                Success = true,
                Message = "Product Created Successfully",
                Data = product.Id
            };
        }
    }
}

