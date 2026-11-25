using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartRestaurant.Domain.Entities;

namespace SmartRestaurant.Infrastructure.Configurations;

public class OrderDetailConfiguration : IEntityTypeConfiguration<OrderDetail>
{
    public void Configure(EntityTypeBuilder<OrderDetail> builder)
    {
        builder.ToTable("OrderDetails", table =>
        {
            table.HasCheckConstraint("CK_OrderDetails_UnitPrice", "[UnitPrice] > 0");
            table.HasCheckConstraint("CK_OrderDetails_Quantity", "[Quantity] > 0");
        });

        builder.HasKey(detail => detail.Id);

        builder.Property(detail => detail.ProductId)
            .IsRequired();

        builder.Property(detail => detail.ProductName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(detail => detail.UnitPrice)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(detail => detail.Quantity)
            .IsRequired();

        builder.Ignore(detail => detail.Subtotal);

        builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(detail => detail.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
