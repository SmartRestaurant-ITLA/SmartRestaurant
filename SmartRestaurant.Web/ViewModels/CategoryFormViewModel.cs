using System.ComponentModel.DataAnnotations;

namespace SmartRestaurant.Web.ViewModels;

public class CategoryFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100, ErrorMessage = "El nombre no puede pasar de 100 caracteres.")]
    [Display(Name = "Nombre")]
    public string Name { get; set; } = string.Empty;
}