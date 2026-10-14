using System.ComponentModel.DataAnnotations;

namespace SmartRestaurant.Web.ViewModels
{
    public class EditReservationViewModel : CreateReservationViewModel
    {
        [Range(1, int.MaxValue,
            ErrorMessage = "El identificador de la reserva no es válido.")]
        public int Id { get; set; }
    }
}