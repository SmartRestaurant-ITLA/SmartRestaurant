using System.ComponentModel.DataAnnotations;

namespace SmartRestaurant.Web.ViewModels;

public class OrderDetailInputViewModel
{
    [Range(1, int.MaxValue, ErrorMessage = "Selecciona un producto.")]
    [Display(Name = "Producto")]
    public int ProductId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor que cero.")]
    [Display(Name = "Cantidad")]
    public int Quantity { get; set; } = 1;
}
