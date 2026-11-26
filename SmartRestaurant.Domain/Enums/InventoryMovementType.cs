using System.ComponentModel.DataAnnotations;

namespace SmartRestaurant.Domain.Enums;

// Tipo de movimiento de inventario: indica si la cantidad suma o resta al stock
public enum InventoryMovementType
{
    [Display(Name = "Entrada")]
    Entry = 1,

    [Display(Name = "Salida")]
    Exit = 2
}
