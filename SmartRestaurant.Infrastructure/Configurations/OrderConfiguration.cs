using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartRestaurant.Domain.Entities;

namespace SmartRestaurant.Infrastructure.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(order => order.Id);

        builder.Property(order => order.TableId)
            .IsRequired();

        builder.Property(order => order.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(order => order.CreatedAt)
            .IsRequired();

        builder.Property(order => order.UpdatedAt)
            .IsRequired();

        builder.Ignore(order => order.Total);

        builder.HasOne<Table>()
            .WithMany()
            .HasForeignKey(order => order.TableId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(order => order.Details)
            .WithOne()
            .HasForeignKey(detail => detail.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
