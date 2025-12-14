using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Common.Pagination;
using Domain.Entities;
using Domain.Enums;

namespace Application.Repositories
{
    public interface IPaymentTransactionRepository
    : IRepository<PaymentTransaction>
    {
        Task<IReadOnlyList<PaymentTransaction>> GetByOrderIdAsync(
            Guid orderId,
            CancellationToken ct);
       Task<PagedResult<PaymentTransaction>> GetByUserIdAsync(
    Guid userId,
    PaymentStatus? status,
    PaginationRequest request,
    CancellationToken ct);
    }

}
