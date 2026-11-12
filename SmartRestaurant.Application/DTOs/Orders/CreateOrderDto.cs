using System.ComponentModel.DataAnnotations;

namespace SmartRestaurant.Application.DTOs.Orders;

public class CreateOrderDto
{
    [Display(Name = "Mesa")]
    public int TableId { get; set; }

    public List<CreateOrderDetailDto> Details { get; set; } = [];
}
