using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.EntityConfigurations
{
    public class OrderEntityConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> b)
        {
            b.HasKey(o => o.Id);

            b.Property(o => o.CreatedAt).IsRequired();

            b.HasMany<OrderItem>().WithOne().HasForeignKey("OrderId").OnDelete(DeleteBehavior.Cascade);
        }
    }
}
