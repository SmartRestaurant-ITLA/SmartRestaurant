using System.ComponentModel.DataAnnotations;
using SmartRestaurant.Domain.Enums;

namespace SmartRestaurant.Application.DTOs.Inventory;

// Datos que llegan del formulario para registrar una entrada o salida
public class RegisterInventoryMovementDto
{
    [Display(Name = "Insumo")]
    public int InventoryItemId { get; set; }

    [Display(Name = "Tipo de movimiento")]
    public InventoryMovementType Type { get; set; }

    [Display(Name = "Cantidad")]
    public decimal Quantity { get; set; }

    [Display(Name = "Fecha del movimiento")]
    [DataType(DataType.DateTime)]
    public DateTime MovementDate { get; set; } = DateTime.Now;

    [Display(Name = "Referencia")]
    public string? Reference { get; set; }

    [Display(Name = "Observación")]
    public string? Observation { get; set; }
}
