using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Application.Models
{
    public record OrderListDto(
    Guid Id,
    OrderStatus Status,
    decimal TotalAmount,
    int ItemCount,
    DateTime CreatedAt
);
    public record OrderItemDto(
    Guid ProductId,
    int Quantity,
    decimal UnitPrice,
    decimal LineTotal
);
    public record OrderDetailsDto(
    Guid Id,
    OrderStatus Status,
    decimal TotalAmount,
    DateTime CreatedAt,
    IReadOnlyList<OrderItemDto> Items,
    IReadOnlyList<OrderTransactionDto> Transactions
);
    public record OrderTransactionDto(
    Guid Id,
    decimal Amount,
    PaymentStatus Status,
    string Provider,
    string Reference,
    DateTime CreatedAt
);


}
