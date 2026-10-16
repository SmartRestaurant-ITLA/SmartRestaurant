using SmartRestaurant.Application.Interfaces;
using SmartRestaurant.Domain.Entities;

namespace SmartRestaurant.Infrastructure.Persistence
{
    public class InMemoryReservationStore : IReservationStore
    {
        private readonly object _sync = new();
        private readonly List<Table> _tables =
        [
            new Table(1, 2),
            new Table(2, 4),
            new Table(3, 4),
            new Table(4, 6),
            new Table(5, 8)
        ];
        private readonly List<Reservation> _reservations = [];
        private int _nextReservationId = 1;

        public IReadOnlyCollection<Table> GetTables()
        {
            lock (_sync)
            {
                return _tables.ToArray();
            }
        }

        public IReadOnlyCollection<Reservation> GetReservations()
        {
            lock (_sync)
            {
                return _reservations.ToArray();
            }
        }

        public Reservation? GetReservation(int id)
        {
            lock (_sync)
            {
                return _reservations.FirstOrDefault(reservation => reservation.Id == id);
            }
        }

        public Reservation SaveReservation(Reservation reservation)
        {
            ArgumentNullException.ThrowIfNull(reservation);

            lock (_sync)
            {
                if (reservation.Id == 0)
                {
                    reservation.AssignId(_nextReservationId++);
                    _reservations.Add(reservation);
                    return reservation;
                }

                var index = _reservations.FindIndex(existing => existing.Id == reservation.Id);
                if (index < 0)
                {
                    throw new KeyNotFoundException($"No existe la reserva {reservation.Id}.");
                }

                _reservations[index] = reservation;
                return reservation;
            }
        }
    }
}
