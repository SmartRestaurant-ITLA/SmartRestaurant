using SmartRestaurant.Domain.Enums;

namespace SmartRestaurant.Application.DTOs.Orders;

public class OrderListItemDto
{
    public int Id { get; set; }

    public int TableId { get; set; }

    public int TableNumber { get; set; }

    public DateTime CreatedAt { get; set; }

    public OrderStatus Status { get; set; }

    public int DetailCount { get; set; }

    public decimal Total { get; set; }
}
