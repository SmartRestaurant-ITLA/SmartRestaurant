using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartRestaurant.Domain.Entities;

namespace SmartRestaurant.Infrastructure.Configurations;

public class InvoiceDetailConfiguration : IEntityTypeConfiguration<InvoiceDetail>
{
    public void Configure(EntityTypeBuilder<InvoiceDetail> builder)
    {
        builder.ToTable("InvoiceDetails", table =>
        {
            table.HasCheckConstraint("CK_InvoiceDetails_UnitPrice", "[UnitPrice] > 0");
            table.HasCheckConstraint("CK_InvoiceDetails_Quantity", "[Quantity] > 0");
        });

        builder.HasKey(detail => detail.Id);

        builder.Property(detail => detail.InvoiceId)
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
    }
}
