
using Microsoft.EntityFrameworkCore;
using SmartRestaurant.Application.Interfaces.Repositories;
using SmartRestaurant.Domain.Entities;
using SmartRestaurant.Domain.Enums;
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

        public Task<Reservation?> GetReservationAsync(int id, CancellationToken ct = default)
        {
            return _context.Reservations
                .AsNoTracking()
                .FirstOrDefaultAsync(reservation => reservation.Id == id, ct);
        }

        public async Task<Reservation> SaveReservationAsync(Reservation reservation, CancellationToken ct = default)
        {
            ArgumentNullException.ThrowIfNull(reservation);

            if (reservation.Id == 0)
            {
                _context.Reservations.Add(reservation);
            }
            else
            {
                var existingReservation = await _context.Reservations
                    .FirstOrDefaultAsync(item => item.Id == reservation.Id, ct);

                if (existingReservation is null)
                {
                    throw new KeyNotFoundException($"No existe la reserva {reservation.Id}.");
                }

                existingReservation.UpdateDetails(
                    reservation.CustomerName,
                    reservation.StartAt,
                    reservation.DurationMinutes,
                    reservation.PartySize,
                    reservation.TableId);
                existingReservation.ChangeStatus(reservation.Status);
                reservation = existingReservation;
            }

            await _context.SaveChangesAsync(ct);
            return reservation;
        }

        public async Task<IReadOnlyCollection<int>> GetBlockingReservationTableIdsAsync(
            DateTime startAt,
            DateTime endAt,
            int? excludedReservationId = null,
            CancellationToken ct = default)
        {
            return await _context.Reservations
                .AsNoTracking()
                .Where(reservation =>
                    (reservation.Status == ReservationStatus.Pending ||
                     reservation.Status == ReservationStatus.Confirmed) &&
                    (excludedReservationId == null || reservation.Id != excludedReservationId) &&
                    reservation.StartAt < endAt &&
                    reservation.StartAt.AddMinutes(reservation.DurationMinutes) > startAt)
                .Select(reservation => reservation.TableId)
                .Distinct()
                .ToArrayAsync(ct);
        }

        public async Task<IReadOnlyCollection<int>> GetBlockingReservationTableIdsAtAsync(
            DateTime at,
            CancellationToken ct = default)
        {
            return await _context.Reservations
                .AsNoTracking()
                .Where(reservation =>
                    (reservation.Status == ReservationStatus.Pending ||
                     reservation.Status == ReservationStatus.Confirmed) &&
                    reservation.StartAt <= at &&
                    reservation.StartAt.AddMinutes(reservation.DurationMinutes) > at)
                .Select(reservation => reservation.TableId)
                .Distinct()
                .ToArrayAsync(ct);
        }
    }
}
