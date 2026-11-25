using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartRestaurant.Application.DTOs.Products;

namespace SmartRestaurant.Web.ViewModels;

public class OrderCreateViewModel
{
    [Range(1, int.MaxValue, ErrorMessage = "Selecciona una mesa.")]
    [Display(Name = "Mesa")]
    public int TableId { get; set; }

    [MinLength(1, ErrorMessage = "Agrega al menos un producto.")]
    public List<OrderDetailInputViewModel> Details { get; set; } = [new()];

    public List<SelectListItem> AvailableTables { get; set; } = [];

    public IReadOnlyList<ProductDto> AvailableProducts { get; set; } = [];
}
