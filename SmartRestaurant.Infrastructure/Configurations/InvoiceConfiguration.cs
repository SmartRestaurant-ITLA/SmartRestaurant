using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartRestaurant.Domain.Entities;

namespace SmartRestaurant.Infrastructure.Configurations;

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("Invoices");

        builder.HasKey(invoice => invoice.Id);

        builder.Property(invoice => invoice.OrderId)
            .IsRequired();

        builder.HasIndex(invoice => invoice.OrderId)
            .IsUnique();

        builder.Property(invoice => invoice.InvoiceNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(invoice => invoice.InvoiceNumber)
            .IsUnique();

        builder.Property(invoice => invoice.IssuedAt)
            .IsRequired();

        builder.Property(invoice => invoice.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(invoice => invoice.CreatedAt)
            .IsRequired();

        builder.Property(invoice => invoice.UpdatedAt)
            .IsRequired();

        builder.Property<byte[]>("RowVersion")
            .IsRowVersion()
            .IsRequired();

        builder.Ignore(invoice => invoice.Total);
        builder.Ignore(invoice => invoice.AmountPaid);
        builder.Ignore(invoice => invoice.Balance);

        builder.HasOne<Order>()
            .WithOne()
            .HasForeignKey<Invoice>(invoice => invoice.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(invoice => invoice.Details)
            .WithOne()
            .HasForeignKey(detail => detail.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(invoice => invoice.Details)
            .HasField("_details")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(invoice => invoice.Payments)
            .WithOne()
            .HasForeignKey(payment => payment.InvoiceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Navigation(invoice => invoice.Payments)
            .HasField("_payments")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
