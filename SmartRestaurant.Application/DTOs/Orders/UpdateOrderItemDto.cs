using System.ComponentModel.DataAnnotations;

namespace SmartRestaurant.Application.DTOs.Orders;

public class UpdateOrderItemDto
{
    public int OrderId { get; set; }

    [Display(Name = "Producto")]
    public int ProductId { get; set; }

    [Display(Name = "Cantidad")]
    public int Quantity { get; set; }
}

/*Descripción: 
 * datos de entrada para cambiar la cantidad de un producto que ya está en el pedido. 
 * Se identifica la línea por OrderId y ProductId, porque el pedido tiene una sola línea por producto.*/