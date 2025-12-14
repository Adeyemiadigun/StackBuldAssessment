using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Models;
using Application.Models;
using MediatR;

namespace Application.Queries.Orders.GetOrderTransaction
{
    public record GetOrderTransactionsQuery(
     Guid OrderId
 ) : IRequest<DataResponse<IReadOnlyList<OrderTransactionDto>>>;

}
