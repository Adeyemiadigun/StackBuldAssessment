using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.Persistence.Repositories
{
    internal class UnitOfWork : IUnitOfWork
    {
        private readonly StoreDbContext _context;
        private IDbContextTransaction? dbContextTransaction;

        public UnitOfWork(StoreDbContext context)
        {
            _context = context;
        }
        public async Task BeginTransactionAsync()
        {
            dbContextTransaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                await dbContextTransaction.CommitAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new ConcurrencyException("Resource was modified by another request.");
            }
        }

        public async Task RollbackAsync()
        {
            await dbContextTransaction!.RollbackAsync();
        }

        public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await _context.SaveChangesAsync(cancellationToken) > 0;
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new Exception("Concurrency conflict occurred.");
            }
        }
    }
}
