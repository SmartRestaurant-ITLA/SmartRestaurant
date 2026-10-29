using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SmartRestaurant.Web.ViewModels
{
    public class CreateReservationViewModel
    {
        [Required(ErrorMessage = "El nombre del cliente es obligatorio.")]
        [StringLength(120)]
        [Display(Name = "Nombre del cliente")]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Selecciona la fecha y hora.")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Fecha y hora")]
        public DateTime StartAt { get; set; }

        [Range(1, int.MaxValue,
            ErrorMessage = "Debe haber al menos una persona.")]
        [Display(Name = "Cantidad de personas")]
        public int PartySize { get; set; } = 2;

        [Range(1, int.MaxValue,
            ErrorMessage = "La duración debe ser positiva.")]
        [Display(Name = "Duración en minutos")]
        public int DurationMinutes { get; set; } = 90;

        [Display(Name = "Mesa seleccionada")]
        public int? TableId { get; set; }

        // Datos para presentar la sugerencia del backend.
        public int? SuggestedTableId { get; set; }

        public string? SuggestedTableDescription { get; set; }

        // Opciones para el selector de mesas.
        public List<SelectListItem> AvailableTables { get; set; }
            = new List<SelectListItem>();
    }
}