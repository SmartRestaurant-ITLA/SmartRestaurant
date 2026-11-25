using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartRestaurant.Domain.Entities;

namespace SmartRestaurant.Infrastructure.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments", table =>
            table.HasCheckConstraint("CK_Payments_Amount", "[Amount] > 0"));

        builder.HasKey(payment => payment.Id);

        builder.Property(payment => payment.InvoiceId)
            .IsRequired();

        builder.Property(payment => payment.Amount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(payment => payment.PaymentDate)
            .IsRequired();

        builder.Property(payment => payment.Method)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(payment => payment.Reference)
            .HasMaxLength(100);
    }
}
