using SmartRestaurant.Application.DTOs.Products;

namespace SmartRestaurant.Web.ViewModels;

public class ProductIndexViewModel
{
    public IReadOnlyList<ProductDto> Products { get; set; } = [];
}
