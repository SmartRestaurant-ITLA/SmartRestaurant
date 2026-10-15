using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartRestaurant.Domain.Entities;

namespace SmartRestaurant.Infrastructure.EntityConfigurations;

public class DailySaleConfigurations : IEntityTypeConfiguration<DailySale>
{
    public void Configure(EntityTypeBuilder<DailySale> builder)
    {
        builder.ToTable("DailySales");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.SaleDate)
            .HasColumnType("date")
            .IsRequired();

        builder.Property(s => s.ProductName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.Quantity).IsRequired();

        builder.Property(s => s.CreatedAt).IsRequired();

        // Un producto no puede repetirse el mismo dia
        builder.HasIndex(s => new { s.SaleDate, s.ProductName })
            .IsUnique();
    }
}