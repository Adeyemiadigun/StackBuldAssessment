using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Application.Commands.Orders.PlaceOrderCommand
{
    public record PlaceOrderCommand(List<PlaceOrderItem> Items)
    : IRequest<Guid>;

    public record PlaceOrderItem(Guid ProductId, int Quantity);

}
