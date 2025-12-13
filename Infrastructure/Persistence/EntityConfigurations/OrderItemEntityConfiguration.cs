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
    public class OrderItemEntityConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> b)
        {
            b.HasKey(oi => oi.Id);

            b.Property<Guid>("OrderId");

            b.Property(oi => oi.ProductId).IsRequired();

            b.Property(oi => oi.Quantity).IsRequired();

            b.Property(oi => oi.UnitPrice).HasPrecision(18, 2).IsRequired();
        }
    }
}
