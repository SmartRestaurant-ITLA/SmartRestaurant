using System.ComponentModel.DataAnnotations;
using SmartRestaurant.Domain.Enums;

namespace SmartRestaurant.Application.DTOs.Inventory;

public class CreateInventoryItemDto
{
    [Display(Name = "Nombre")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Descripción")]
    public string? Description { get; set; }

    [Display(Name = "Categoría")]
    public InventoryCategory Category { get; set; }

    [Display(Name = "Unidad de medida")]
    public UnitOfMeasure Unit { get; set; }

    [Display(Name = "Stock actual")]
    public decimal CurrentStock { get; set; }

    [Display(Name = "Stock mínimo")]
    public decimal MinimumStock { get; set; }

    [Display(Name = "Costo unitario (RD$)")]
    public decimal UnitCost { get; set; }

    [Display(Name = "Fecha de vencimiento")]
    [DataType(DataType.Date)]
    public DateTime? ExpirationDate { get; set; }
}