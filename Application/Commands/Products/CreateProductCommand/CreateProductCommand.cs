using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Models;
using MediatR;

namespace Application.Commands.Products.CreateProductCommand
{
    public record CreateProductCommand(CreateProductDto product) : IRequest<DataResponse<Guid>>;
}
