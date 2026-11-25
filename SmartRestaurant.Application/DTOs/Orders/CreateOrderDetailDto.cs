using System.ComponentModel.DataAnnotations;

namespace SmartRestaurant.Application.DTOs.Orders;

public class CreateOrderDetailDto
{
    [Display(Name = "Producto")]
    public int ProductId { get; set; }

    [Display(Name = "Cantidad")]
    public int Quantity { get; set; }
}
