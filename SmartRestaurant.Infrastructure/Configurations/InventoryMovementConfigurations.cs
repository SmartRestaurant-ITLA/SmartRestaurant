using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartRestaurant.Domain.Entities;

namespace SmartRestaurant.Infrastructure.Configurations;

public class InventoryMovementConfigurations : IEntityTypeConfiguration<InventoryMovement>
{
    public void Configure(EntityTypeBuilder<InventoryMovement> builder)
    {
        builder.ToTable("InventoryMovements");

        builder.HasKey(m => m.Id);

        // Relación: cada movimiento pertenece a un insumo; un insumo tiene muchos movimientos.
        // Restrict: no se puede borrar un insumo que tenga historial (los insumos se desactivan).
        builder.HasOne(m => m.InventoryItem)
            .WithMany()
            .HasForeignKey(m => m.InventoryItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(m => m.Type)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(m => m.Quantity).HasPrecision(18, 3);
        builder.Property(m => m.StockAfter).HasPrecision(18, 3);

        builder.Property(m => m.MovementDate).IsRequired();

        builder.Property(m => m.Reference)
            .HasMaxLength(100);

        builder.Property(m => m.Observation)
            .HasMaxLength(500);

        builder.Property(m => m.CreatedAt).IsRequired();

        // Índice para que el historial por insumo y fecha se consulte rápido
        builder.HasIndex(m => new { m.InventoryItemId, m.MovementDate });
    }
}
