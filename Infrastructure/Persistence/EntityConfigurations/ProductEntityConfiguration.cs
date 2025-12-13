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
    public class ProductEntityConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> b)
        {
            b.HasKey(p => p.Id);

            b.Property(p => p.Name).HasMaxLength(200).IsRequired();

            b.Property(p => p.Description).HasMaxLength(1000);

            b.Property(p => p.Price).HasPrecision(18, 2).IsRequired();

            b.Property(p => p.StockQuantity).IsRequired();

            b.Property(p => p.RowVersion).IsRowVersion(); //
        }
    }
}
