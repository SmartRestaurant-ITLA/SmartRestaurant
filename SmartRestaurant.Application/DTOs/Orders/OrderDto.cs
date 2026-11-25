using SmartRestaurant.Domain.Enums;

namespace SmartRestaurant.Application.DTOs.Orders;

public class OrderDto
{
    public int Id { get; set; }

    public int TableId { get; set; }

    public int TableNumber { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public OrderStatus Status { get; set; }

    public decimal Total { get; set; }

    public IReadOnlyList<OrderDetailDto> Details { get; set; } = [];

    public IReadOnlyList<OrderStatus> AvailableStatusTransitions { get; set; } = [];
}
