using System.ComponentModel.DataAnnotations;

namespace SmartRestaurant.Application.DTOs.Orders;

public class AddOrderItemDto
{
    public int OrderId { get; set; }

    [Display(Name = "Producto")]
    public int ProductId { get; set; }

    [Display(Name = "Cantidad")]
    public int Quantity { get; set; }
}

/*Descripción: 
 * datos de entrada para agregar un producto a un pedido existente. 
 * Lleva solo el pedido, el producto y la cantidad. 
 * El precio y el nombre no viajan aquí: el servidor los toma del catálogo, así el navegador no puede alterarlos.*/