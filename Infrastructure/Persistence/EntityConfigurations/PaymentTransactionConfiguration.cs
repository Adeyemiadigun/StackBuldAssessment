using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.EntityConfigurations
{
    using Domain.Entities;
    using Domain.Enums;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class PaymentTransactionConfiguration : IEntityTypeConfiguration<PaymentTransaction>
    {
        public void Configure(EntityTypeBuilder<PaymentTransaction> builder)
        {
            // Table name
            builder.ToTable("PaymentTransactions");

            // Primary key
            builder.HasKey(pt => pt.Id);

            // Properties
            builder.Property(pt => pt.OrderId)
                .IsRequired();

            builder.Property(pt => pt.UserId)
                .IsRequired();

            builder.Property(pt => pt.Amount)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(pt => pt.Provider)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(pt => pt.Reference)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(pt => pt.Status)
                .IsRequired()
                .HasConversion<string>(); // store enum as string

            builder.Property(pt => pt.CreatedAt)
                .IsRequired();

            // Relationships
            builder.HasOne<Order>()
                .WithMany(o => o.Transactions)
                .HasForeignKey(pt => pt.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Optional: configure field access for strict domain encapsulation
            builder.Metadata
                .FindNavigation(nameof(PaymentTransaction.Status))?
                .SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
    class PaymentTransactionConfiguration
    {
    }
}
