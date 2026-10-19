
using Microsoft.EntityFrameworkCore;
using SmartRestaurant.Application.Interfaces;
using SmartRestaurant.Domain.Entities;
using SmartRestaurant.Infrastructure.Contexts;

namespace SmartRestaurant.Infrastructure.Repositories
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly SmartRestaurantDbContext _context;

        public ReservationRepository(
            SmartRestaurantDbContext context)
        {
            _context = context;
        }

        public IReadOnlyCollection<Table> GetTables()
        {
            return _context.Tables
                .AsNoTracking()
                .OrderBy(table => table.Number)
                .ToArray();
        }

        public IReadOnlyCollection<Reservation> GetReservations()
        {
            return _context.Reservations
                .AsNoTracking()
                .OrderBy(reservation => reservation.StartAt)
                .ToArray();
        }

        public Reservation? GetReservation(int id)
        {
            return _context.Reservations
                .AsNoTracking()
                .FirstOrDefault(reservation => reservation.Id == id);
        }

        public Reservation SaveReservation(Reservation reservation)
        {
            ArgumentNullException.ThrowIfNull(reservation);

            if (reservation.Id == 0)
            {
                _context.Reservations.Add(reservation);
            }
            else
            {
                var existingReservation = _context.Reservations
                    .FirstOrDefault(
                        item => item.Id == reservation.Id);

                if (existingReservation is null)
                {
                    throw new KeyNotFoundException(
                        $"No existe la reserva {reservation.Id}.");
                }

                existingReservation.UpdateDetails(
                    reservation.CustomerName,
                    reservation.StartAt,
                    reservation.DurationMinutes,
                    reservation.PartySize,
                    reservation.TableId);

                existingReservation.ChangeStatus(
                    reservation.Status);

                reservation = existingReservation;
            }

            _context.SaveChanges();

            return reservation;
        }
    }
}
