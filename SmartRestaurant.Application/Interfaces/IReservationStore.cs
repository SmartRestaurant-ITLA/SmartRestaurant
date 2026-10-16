using SmartRestaurant.Domain.Entities;

namespace SmartRestaurant.Application.Interfaces
{
    public interface IReservationStore
    {
        IReadOnlyCollection<Table> GetTables();

        IReadOnlyCollection<Reservation> GetReservations();

        Reservation? GetReservation(int id);

        Reservation SaveReservation(Reservation reservation);
    }
}
