
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartRestaurant.Domain.Entities;

namespace SmartRestaurant.Infrastructure.Configurations
{
    public class TableConfiguration
        : IEntityTypeConfiguration<Table>
    {
        public void Configure(EntityTypeBuilder<Table> builder)
        {
            builder.ToTable("Tables");

            builder.HasKey(table => table.Id);

            builder.Property(table => table.Id)
                .ValueGeneratedNever();

            builder.Property(table => table.Number)
                .IsRequired();

            builder.Property(table => table.Capacity)
                .IsRequired();

            builder.Property(table => table.IsActive)
                .IsRequired();

            builder.HasIndex(table => table.Number)
                .IsUnique();
        }
    }
}
