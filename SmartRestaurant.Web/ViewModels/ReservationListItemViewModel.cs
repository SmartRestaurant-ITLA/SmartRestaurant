using System;

namespace SmartRestaurant.Web.ViewModels
{
    public class ReservationListItemViewModel
    {
        public int Id { get; set; }

        public string CustomerName { get; set; } = string.Empty;

        public DateTime StartAt { get; set; }

        public int PartySize { get; set; }

        public int DurationMinutes { get; set; }

        public int TableNumber { get; set; }

        public string Status { get; set; } = string.Empty;

        public DateTime EndAt =>
            StartAt.AddMinutes(DurationMinutes);
    }
}