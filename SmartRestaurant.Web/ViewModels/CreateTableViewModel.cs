
using System;
using System.ComponentModel.DataAnnotations;

namespace SmartRestaurant.Web.ViewModels
{
    public class CreateTableViewModel
    {
        [Display(Name = "Número de mesa")]
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "El número de mesa debe ser mayor que cero."
        )]
        public int Number { get; set; }

        [Display(Name = "Capacidad de personas")]
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "La capacidad debe ser mayor que cero."
        )]
        public int Capacity { get; set; } = 2;

        [Display(Name = "Mesa activa")]
        public bool IsActive { get; set; } = true;
    }
}
