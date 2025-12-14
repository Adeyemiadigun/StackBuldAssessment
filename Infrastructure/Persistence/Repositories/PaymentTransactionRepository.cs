using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Models;
using Application.Common.Pagination;
using Application.Repositories;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class PaymentTransactionRepository : Repository<PaymentTransaction>, IPaymentTransactionRepository
    {
        public PaymentTransactionRepository(StoreDbContext context) : base(context)
        {
        }
        public async Task<IReadOnlyList<PaymentTransaction>> GetByOrderIdAsync(
        Guid orderId,
    CancellationToken ct)
        {
            return await _context.PaymentTransactions
                .Where(t => t.OrderId == orderId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync(ct);
        }
        public async Task<PagedResult<PaymentTransaction>> GetByUserIdAsync(
        Guid userId,
        PaymentStatus? status,
        PaginationRequest request,
        CancellationToken ct)
        {
            var query = _context.PaymentTransactions
                .AsNoTracking()
                .Where(t => t.UserId == userId);

            if (status.HasValue)
                query = query.Where(t => t.Status == status.Value);

            var total = await query.CountAsync(ct);

            var data = await query
                .OrderByDescending(t => t.CreatedAt)
                .Skip((request.page - 1) * request.pageSize)
                .Take(request.pageSize)
                .ToListAsync(ct);

            return new PagedResult<PaymentTransaction>(
                data,
                total,
                request.page,
                request.pageSize);
        }



    }
}
