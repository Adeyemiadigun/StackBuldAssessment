using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Models;
using Application.Queries.Products;
using MediatR;

namespace Application.Commands.Products.UpdateProductCommand
{
    public record UpdateProductCommand(UpdateProductRequestDto request) : IRequest<DataResponse<ProductDto>>;
}
