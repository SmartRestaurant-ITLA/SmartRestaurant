
using System;
using System.ComponentModel.DataAnnotations;

namespace SmartRestaurant.Web.ViewModels
{
    public class EditTableViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Número de mesa")]
        public int Number { get; set; }

        [Display(Name = "Capacidad de personas")]
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "La capacidad debe ser mayor que cero."
        )]
        public int Capacity { get; set; }

        [Display(Name = "Mesa habilitada")]
        public bool IsActive { get; set; }
    }
}
