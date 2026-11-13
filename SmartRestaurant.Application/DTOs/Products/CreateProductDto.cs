using System.ComponentModel.DataAnnotations;

namespace SmartRestaurant.Application.DTOs.Products;

public class CreateProductDto
{
    [Display(Name = "Nombre del producto")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Precio (RD$)")]
    public decimal Price { get; set; }

    // Catálogo (César)
    [Display(Name = "Descripción")]
    public string? Description { get; set; }

    [Display(Name = "Categoría")]
    public int? CategoryId { get; set; }
}