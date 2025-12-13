using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {

        public OrderRepository(StoreDbContext context) : base(context) 
        {
        }

        public async Task<Order?> GetOrderWithDetailsAsync(
            Guid orderId,
            CancellationToken ct)
        {
            return await _context.Orders
                .Include(o => o.Items)
                .Include(o => o.Transactions)
                .FirstOrDefaultAsync(o => o.Id == orderId, ct);
        }
    }

}
