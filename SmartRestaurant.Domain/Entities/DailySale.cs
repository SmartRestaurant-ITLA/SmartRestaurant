using SmartRestaurant.Domain.Common;

namespace SmartRestaurant.Domain.Entities;

// Resumen diario de ventas: una fila por producto por dia
public class DailySale : BaseEntity
{
    public DateTime SaleDate { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public int Quantity { get; set; }
}