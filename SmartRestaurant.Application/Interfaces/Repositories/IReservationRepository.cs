using SmartRestaurant.Domain.Entities;

namespace SmartRestaurant.Application.Interfaces.Repositories
{
    public interface IReservationRepository
    {
        IReadOnlyCollection<Table> GetTables();

        IReadOnlyCollection<Reservation> GetReservations();

        Reservation? GetReservation(int id);

        Reservation SaveReservation(Reservation reservation);

        Task<Reservation?> GetReservationAsync(int id, CancellationToken ct = default);

        Task<Reservation> SaveReservationAsync(Reservation reservation, CancellationToken ct = default);

        Task<IReadOnlyCollection<int>> GetBlockingReservationTableIdsAsync(
            DateTime startAt,
            DateTime endAt,
            int? excludedReservationId = null,
            CancellationToken ct = default);

        Task<IReadOnlyCollection<int>> GetBlockingReservationTableIdsAtAsync(
            DateTime at,
            CancellationToken ct = default);
    }
}
