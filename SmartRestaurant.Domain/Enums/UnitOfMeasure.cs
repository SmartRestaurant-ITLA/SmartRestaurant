using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SmartRestaurant.Domain.Enums;

public enum UnitOfMeasure
{
    [Display(Name = "Unidad")]
    Unit = 1,

    [Display(Name = "Kilogramo (kg)")]
    Kilogram = 2,

    [Display(Name = "Gramo (g)")]
    Gram = 3,

    [Display(Name = "Libra (lb)")]
    Pound = 4,

    [Display(Name = "Litro (L)")]
    Liter = 5,

    [Display(Name = "Mililitro (ml)")]
    Milliliter = 6,

    [Display(Name = "Galón")]
    Gallon = 7,

    [Display(Name = "Paquete")]
    Package = 8
}
