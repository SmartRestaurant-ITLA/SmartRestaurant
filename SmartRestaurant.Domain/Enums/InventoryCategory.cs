using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SmartRestaurant.Domain.Enums;

public enum InventoryCategory
{
    [Display(Name = "Carnes")]
    Meats = 1,

    [Display(Name = "Pescados y mariscos")]
    Seafood = 2,

    [Display(Name = "Vegetales")]
    Vegetables = 3,

    [Display(Name = "Frutas")]
    Fruits = 4,

    [Display(Name = "Lácteos y huevos")]
    Dairy = 5,

    [Display(Name = "Granos y harinas")]
    Grains = 6,

    [Display(Name = "Bebidas")]
    Beverages = 7,

    [Display(Name = "Condimentos y especias")]
    Spices = 8,

    [Display(Name = "Limpieza y desechables")]
    Supplies = 9,

    [Display(Name = "Otros")]
    Other = 10
}