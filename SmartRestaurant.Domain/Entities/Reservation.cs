using SmartRestaurant.Domain.Enums;

namespace SmartRestaurant.Domain.Entities
{
    public class Reservation
    {
        public int Id { get; private set; }

        public string CustomerName { get; private set; }

        public DateTime StartAt { get; private set; }

        public int DurationMinutes { get; private set; }

        public int PartySize { get; private set; }

        public int TableId { get; private set; }

        public ReservationStatus Status { get; private set; }

        public Reservation(
            string customerName,
            DateTime startAt,
            int durationMinutes,
            int partySize,
            int tableId,
            ReservationStatus status = ReservationStatus.Pending)
        {
            ArgumentNullException.ThrowIfNull(customerName);

            if (durationMinutes <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(durationMinutes),
                    "La duración debe ser positiva.");
            }

            if (partySize <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(partySize),
                    "El tamaño del grupo debe ser positivo.");
            }

            if (!Enum.IsDefined(status))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(status),
                    "El estado no es válido.");
            }

            CustomerName = customerName;
            StartAt = startAt;
            DurationMinutes = durationMinutes;
            PartySize = partySize;
            TableId = tableId;
            Status = status;
        }

        public void UpdateDetails(
            string customerName,
            DateTime startAt,
            int durationMinutes,
            int partySize,
            int tableId)
        {
            ArgumentNullException.ThrowIfNull(customerName);

            if (durationMinutes <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(durationMinutes),
                    "La duración debe ser positiva.");
            }

            if (partySize <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(partySize),
                    "El tamaño del grupo debe ser positivo.");
            }

            if (tableId <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(tableId),
                    "La mesa debe ser válida.");
            }

            CustomerName = customerName;
            StartAt = startAt;
            DurationMinutes = durationMinutes;
            PartySize = partySize;
            TableId = tableId;
        }

        public void AssignId(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            if (Id != 0)
            {
                throw new InvalidOperationException("La reserva ya tiene un identificador.");
            }

            Id = id;
        }

        public void ChangeStatus(ReservationStatus status)
        {
            if (!Enum.IsDefined(status))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(status),
                    "El estado no es válido.");
            }

            Status = status;
        }
    }
}