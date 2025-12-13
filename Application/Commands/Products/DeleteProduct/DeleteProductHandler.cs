using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Exceptions;
using Domain.Entities;
using MediatR;

namespace Application.Commands.Products.DeleteProduct
{
    public class DeleteProductHandler : IRequestHandler<DeleteProductCommand>
    {
        private readonly IRepository<Product> _repo;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteProductHandler(IRepository<Product> repo, IUnitOfWork unitOfWork)
        {
            _repo = repo;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(DeleteProductCommand request, CancellationToken ct)
        {
            var product = await _repo.GetByIdAsync(request.Id, ct)
                ?? throw new ApiException("Product not found", 404, "ProductNotFound");

            await _repo.DeleteAsync(product, ct);
            await _unitOfWork.SaveChangesAsync(ct);

        }

        
    }

}
