using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Context;

public class StoreDbContext : DbContext
{
    public StoreDbContext(DbContextOptions<StoreDbContext> options) : base(options) { }
    
            public DbSet<Product> Products { get; set; } = null!;
            public DbSet<Order> Orders { get; set; } = null!;
            public DbSet<OrderItem> OrderItems { get; set; } = null!;
        
}
    

