
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartRestaurant.Domain.Entities;

namespace SmartRestaurant.Infrastructure.Configurations
{
    public class ReservationConfiguration
        : IEntityTypeConfiguration<Reservation>
    {
        public void Configure(
            EntityTypeBuilder<Reservation> builder)
        {
            builder.ToTable("Reservations");

            builder.HasKey(reservation => reservation.Id);

            builder.Property(reservation => reservation.Id)
                .ValueGeneratedOnAdd();

            builder.Property(reservation => reservation.CustomerName)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(reservation => reservation.StartAt)
                .IsRequired();

            builder.Property(reservation => reservation.DurationMinutes)
                .IsRequired();

            builder.Property(reservation => reservation.PartySize)
                .IsRequired();

            builder.Property(reservation => reservation.TableId)
                .IsRequired();

            builder.Property(reservation => reservation.Status)
                .HasConversion<int>()
                .IsRequired();
        }
    }
}
