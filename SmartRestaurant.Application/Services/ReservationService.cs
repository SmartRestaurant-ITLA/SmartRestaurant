using SmartRestaurant.Application.Interfaces.Repositories;
using SmartRestaurant.Domain.Entities;
using SmartRestaurant.Domain.Enums;

namespace SmartRestaurant.Application.Services
{
    public class ReservationService
    {
        private readonly IReservationRepository _repository;

        public ReservationService(IReservationRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public IReadOnlyCollection<Table> GetTables()
        {
            return _repository.GetTables();
        }

        public IReadOnlyCollection<Reservation> GetReservations()
        {
            return _repository.GetReservations();
        }

        public Reservation? GetReservation(int id)
        {
            return _repository.GetReservation(id);
        }

        public IReadOnlyCollection<Table> GetAvailableTables(
            DateTime startAt,
            int durationMinutes,
            int partySize,
            int? excludedReservationId = null)
        {
            ValidateReservationDetails(durationMinutes, partySize);
            var endAt = startAt.AddMinutes(durationMinutes);
            var reservations = _repository.GetReservations();

            return _repository.GetTables()
                .Where(table => table.IsActive && table.Capacity >= partySize)
                .Where(table => !reservations.Any(reservation =>
                    reservation.Id != excludedReservationId &&
                    reservation.TableId == table.Id &&
                    reservation.Status != ReservationStatus.Cancelled &&
                    Overlaps(startAt, endAt, reservation)))
                .ToArray();
        }

        public Reservation SaveReservation(
            string customerName,
            DateTime startAt,
            int durationMinutes,
            int partySize,
            int tableId,
            int? reservationId = null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(customerName);
            ValidateReservationDetails(durationMinutes, partySize);

            var existing = reservationId.HasValue
                ? _repository.GetReservation(reservationId.Value)
                    ?? throw new KeyNotFoundException($"No existe la reserva {reservationId.Value}.")
                : null;

            ValidateTableAvailability(tableId, startAt, durationMinutes, partySize, existing?.Id);

            if (existing is null)
            {
                return _repository.SaveReservation(new Reservation(
                    customerName,
                    startAt,
                    durationMinutes,
                    partySize,
                    tableId));
            }

            existing.UpdateDetails(customerName, startAt, durationMinutes, partySize, tableId);
            return _repository.SaveReservation(existing);
        }

        public Reservation ConfirmReservation(int reservationId)
        {
            var reservation = _repository.GetReservation(reservationId)
                ?? throw new KeyNotFoundException($"No existe la reserva {reservationId}.");

            ValidateTableAvailability(
                reservation.TableId,
                reservation.StartAt,
                reservation.DurationMinutes,
                reservation.PartySize,
                reservation.Id);

            reservation.ChangeStatus(ReservationStatus.Confirmed);
            return _repository.SaveReservation(reservation);
        }

        private void ValidateTableAvailability(
            int tableId,
            DateTime startAt,
            int durationMinutes,
            int partySize,
            int? excludedReservationId)
        {
            var table = _repository.GetTables().FirstOrDefault(candidate => candidate.Id == tableId)
                ?? throw new KeyNotFoundException($"No existe la mesa {tableId}.");

            if (!table.IsActive)
            {
                throw new InvalidOperationException("La mesa no está activa.");
            }

            if (table.Capacity < partySize)
            {
                throw new InvalidOperationException("La capacidad de la mesa es insuficiente.");
            }

            if (!GetAvailableTables(startAt, durationMinutes, partySize, excludedReservationId)
                .Any(availableTable => availableTable.Id == tableId))
            {
                throw new InvalidOperationException("La mesa no está disponible en el horario solicitado.");
            }
        }

        private static void ValidateReservationDetails(int durationMinutes, int partySize)
        {
            if (durationMinutes <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(durationMinutes), "La duración debe ser positiva.");
            }

            if (partySize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(partySize), "El tamaño del grupo debe ser positivo.");
            }
        }

        private static bool Overlaps(DateTime startAt, DateTime endAt, Reservation reservation)
        {
            var reservationEndAt = reservation.StartAt.AddMinutes(reservation.DurationMinutes);
            return startAt < reservationEndAt && reservation.StartAt < endAt;
        }
    }
}
