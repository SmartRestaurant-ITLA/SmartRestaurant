using SmartRestaurant.Application.DTOs.Orders;

namespace SmartRestaurant.Web.ViewModels;

public class OrderIndexViewModel
{
    public IReadOnlyList<OrderListItemDto> Orders { get; set; } = [];
}
