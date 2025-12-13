using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.EntityConfigurations;
public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        // Table name
        builder.ToTable("Orders");

        // Primary key
        builder.HasKey(o => o.Id);

        // Properties
        builder.Property(o => o.Status)
            .IsRequired()
            .HasConversion<string>(); // store enum as string, optional: use int if preferred

        builder.Property(o => o.CreatedAt)
            .IsRequired();

        builder.Property(o => o.UserId)
            .IsRequired();

        builder.Property(o => o.TotalAmount)
            .HasPrecision(18, 2) // optional, EF ignores computed by default
            .HasComputedColumnSql("0"); // EF won't track _items sum automatically; leave for domain

        // Relations
        builder.HasMany(o => o.Items)
            .WithOne()
            .HasForeignKey("OrderId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(o => o.Transactions)
            .WithOne()
            .HasForeignKey("OrderId")
            .OnDelete(DeleteBehavior.Cascade);

        // Ignore private fields if needed
        builder.Ignore("_items"); // EF will map Items via navigation anyway
        builder.Ignore("TotalAmount"); // computed in domain
        builder.Ignore("paymentTransactions");
    }
}
