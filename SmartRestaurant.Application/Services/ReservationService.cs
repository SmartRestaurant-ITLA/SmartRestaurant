using SmartRestaurant.Application.Interfaces.Repositories;
using SmartRestaurant.Application.Interfaces.Services;
using SmartRestaurant.Application.Interfaces.Repositories;
using SmartRestaurant.Domain.Entities;
using SmartRestaurant.Domain.Enums;

namespace SmartRestaurant.Application.Services
{
    public class ReservationService
    {
        private readonly IReservationRepository _repository;
        private readonly ITableAvailabilityService _tableAvailabilityService;

        public ReservationService(
            IReservationRepository repository,
            ITableAvailabilityService tableAvailabilityService)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _tableAvailabilityService = tableAvailabilityService ?? throw new ArgumentNullException(nameof(tableAvailabilityService));
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

        public Task<IReadOnlyList<Table>> GetAvailableTables(
            DateTime startAt,
            int durationMinutes,
            int partySize,
            int? excludedReservationId = null,
            CancellationToken ct = default) =>
            _tableAvailabilityService.GetAvailableTablesForReservationAsync(
                startAt,
                durationMinutes,
                partySize,
                excludedReservationId,
                ct);

        public async Task<Reservation> SaveReservation(
            string customerName,
            DateTime startAt,
            int durationMinutes,
            int partySize,
            int tableId,
            int? reservationId = null,
            CancellationToken ct = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(customerName);
            ValidateReservationDetails(durationMinutes, partySize);

            var existing = reservationId.HasValue
                ? await _repository.GetReservationAsync(reservationId.Value, ct)
                    ?? throw new KeyNotFoundException($"No existe la reserva {reservationId.Value}.")
                : null;

            await ValidateTableAvailability(tableId, startAt, durationMinutes, partySize, existing?.Id, ct);

            if (existing is null)
            {
                return await _repository.SaveReservationAsync(new Reservation(
                    customerName,
                    startAt,
                    durationMinutes,
                    partySize,
                    tableId), ct);
            }

            existing.UpdateDetails(customerName, startAt, durationMinutes, partySize, tableId);
            return await _repository.SaveReservationAsync(existing, ct);
        }

        public async Task<Reservation> ConfirmReservation(int reservationId, CancellationToken ct = default)
        {
            var reservation = await _repository.GetReservationAsync(reservationId, ct)
                ?? throw new KeyNotFoundException($"No existe la reserva {reservationId}.");

            await ValidateTableAvailability(
                reservation.TableId,
                reservation.StartAt,
                reservation.DurationMinutes,
                reservation.PartySize,
                reservation.Id,
                ct);

            reservation.ChangeStatus(ReservationStatus.Confirmed);
            return await _repository.SaveReservationAsync(reservation, ct);
        }

        private async Task ValidateTableAvailability(
            int tableId,
            DateTime startAt,
            int durationMinutes,
            int partySize,
            int? excludedReservationId,
            CancellationToken ct)
        {
            if (!await _tableAvailabilityService.CanAssignReservationAsync(
                    tableId,
                    startAt,
                    durationMinutes,
                    partySize,
                    excludedReservationId,
                    ct))
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
