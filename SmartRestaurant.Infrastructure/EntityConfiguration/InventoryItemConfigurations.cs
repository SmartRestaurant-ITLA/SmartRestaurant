using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartRestaurant.Domain.Entities;

namespace SmartRestaurant.Infrastructure.EntityConfigurations;

public class InventoryItemConfigurations : IEntityTypeConfiguration<InventoryItem>
{
    public void Configure(EntityTypeBuilder<InventoryItem> builder)
    {
        builder.ToTable("InventoryItems");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(i => i.Name)
            .IsUnique();

        builder.Property(i => i.Description)
            .HasMaxLength(500);

        builder.Property(i => i.Category)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(i => i.Unit)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(i => i.CurrentStock).HasPrecision(18, 3);
        builder.Property(i => i.MinimumStock).HasPrecision(18, 3);
        builder.Property(i => i.UnitCost).HasPrecision(18, 2);

        builder.Property(i => i.ExpirationDate).HasColumnType("date");

        builder.Property(i => i.CreatedAt).IsRequired();
    }
}