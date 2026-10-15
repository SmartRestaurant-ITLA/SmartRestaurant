using System.ComponentModel.DataAnnotations;

namespace SmartRestaurant.Application.DTOs.Sales;

public class CreateDailySaleDto
{
    [Display(Name = "Fecha de la venta")]
    [DataType(DataType.Date)]
    public DateTime SaleDate { get; set; } = DateTime.Today;

    [Display(Name = "Producto o platillo")]
    public string ProductName { get; set; } = string.Empty;

    [Display(Name = "Cantidad vendida")]
    public int Quantity { get; set; }
}