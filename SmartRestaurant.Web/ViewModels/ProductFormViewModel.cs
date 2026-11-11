using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SmartRestaurant.Web.ViewModels;

public class ProductFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(150, ErrorMessage = "El nombre no puede pasar de 150 caracteres.")]
    [Display(Name = "Nombre")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "La descripción no puede pasar de 500 caracteres.")]
    [Display(Name = "Descripción")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "El precio es obligatorio.")]
    [Range(0, 999999.99, ErrorMessage = "El precio no puede ser negativo.")]
    [Display(Name = "Precio")]
    public decimal Price { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una categoría.")]
    [Display(Name = "Categoría")]
    public int CategoryId { get; set; }

    // Lista de categorías activas para el desplegable
    public List<SelectListItem> Categories { get; set; } = new();
}